using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public partial class SXMLE2 : Form
{
	public const int CONFIGVERSION = 1;

	public static string exeDir =
#if DEBUG
		"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Stationeers";
#else
		AppDomain.CurrentDomain.BaseDirectory;
#endif

	public static string configDir = Path.Combine(exeDir, "SXMLE_Configs");

	public static bool IsInCorrectDirectory
	{
		get
		{
			string endDirectory = Path.GetFileName(exeDir.TrimEnd(Path.DirectorySeparatorChar));
			string targetDirectory = "Stationeers";

			return string.Equals(endDirectory, targetDirectory, StringComparison.OrdinalIgnoreCase);
		}
	}

	private static StationeersXMLEditor? editor;
	public static BuildConfig? OpenConfig;
	public static string? OpenConfigFile; // xxxx.xml
	private XmlSerializer configSerializer = new XmlSerializer(typeof(BuildConfig));

	public SXMLE2()
	{
		InitializeComponent();

		Prepare();
	}

	private void Prepare()
	{
		if (!Directory.Exists(configDir))
			SXMLE2.RecursiveCreateDirectory(configDir);

		if (!File.Exists(Path.Combine(configDir, "default.xml")))
			using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "default.xml")))
				configSerializer.Serialize(writer, BuildConfig._default);

		using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "vanilla.xml")))
			configSerializer.Serialize(writer, BuildConfig._vanilla);


		SelectConfig("default.xml");
	}

	private void SelectConfig(string configName)
	{
		ConfigList.Text = configName;
		string selectedConfigPath = Path.Combine(configDir, configName);
		if (File.Exists(selectedConfigPath))
		{
			using (StreamReader reader = new StreamReader(selectedConfigPath))
			{
				OpenConfig = (BuildConfig)configSerializer.Deserialize(reader);
				OpenConfigFile = Path.GetFileName(selectedConfigPath);
				if (OpenConfig.CONFIG_VERSION != CONFIGVERSION)
				{
					MessageBox.Show($"Config version ({OpenConfig.CONFIG_VERSION}) doesn't match SXMLE2 version ({CONFIGVERSION}). Please check the vanilla.xml config for differences in your config.", "WARNING!", MessageBoxButtons.OK);
				}
			}

			string text = "";

			XmlDocument doc = new XmlDocument();

			doc.LoadXml(File.ReadAllText(selectedConfigPath));

			XmlNodeList elements = doc.SelectNodes("//BuildConfig/*");
			if (elements != null)
				foreach (XmlNode node in elements)
				{
					if (node.Name == "EXCLUDE_VEINS")
					{
						text += "EXCLUDE_VEINS: ";
						List<string> nodes = new();
						foreach (XmlNode node2 in node.ChildNodes)
							nodes.Add(node2.InnerText);
						text += $"{string.Join(", ", nodes)}";
						continue;
					}
					text += $"{node.Name}: {node.InnerText}\n";
				}


			ConfigSettingsBox.Text = text;
		}
		else
		{
			Prepare();
			ReloadConfigList();
		}
	}

	private void BuildSelectedConfig()
	{
		if (OpenConfig == null)
		{
			MessageBox.Show("No build config selected. Please select a build config.");
			return;
		}

		editor = new StationeersXMLEditor(exeDir);

		foreach (StationeersXmlFile file in editor.XmlFiles)
		{
			editor.Load(file.Path);

			if (editor.ElementExists("stationeers_edited"))
				continue;
			else
				editor.DocumentElement.PrependChild(editor.CreateElement("stationeers_edited"));

			// Assemblers changes
			if (editor.IsWhitelisted(new[] { "autolathe", "automatedoven", "cooking", "DynamicObjectsFabricator", "electronics", "fabricator", "gascanisters", "chemistry", "organicsprinter", "Packaging", "paintmixer", "paints", "PipeBender", "rocketmanufactory", "security", "toolmanufacturer" }))
			{
				// Remove all empty nodes
				foreach (XmlNode node in editor.SelectNodes("//*[text()='0']"))
				{
					string parentName = node.ParentNode.ParentNode.SelectSingleNode("PrefabName").InnerText;
					node.ParentNode.RemoveChild(node);
				}

				// Change crafting values
				foreach (XmlNode node in editor.SelectNodes("//Recipe/*"))
				{
					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);
					switch (node.Name)
					{
						case "Time":
							value = Math.Clamp(value * OpenConfig.CRAFTING_TIME, 0.01f, float.MaxValue);
							break;

						case "Energy":
							value *= OpenConfig.CRAFTING_ENERGY;
							break;

						default: // Materials
							value *= OpenConfig.CRAFTING_INGREDIENTS;
							break;
					}
					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
					string parentName = node.ParentNode.ParentNode.SelectSingleNode("PrefabName").InnerText;
				}
			}

			// Furnace and Advanced Furnace changes
			if (editor.IsWhitelisted(new[] { "furnace", "advancedfurnace" }))
			{
				// Increase required stop pressure/temperature, reduce required start pressure/temperature
				foreach (XmlNode node in editor.SelectNodes("//Start | //Stop | //Output"))
				{
					string parentName =
						(node.ParentNode.ParentNode.ParentNode.SelectSingleNode("PrefabName")?.InnerText) ??
						(node.ParentNode.ParentNode.SelectSingleNode("PrefabName")?.InnerText) ??
						(node.ParentNode.SelectSingleNode("PrefabName")?.InnerText);

					string type = node.ParentNode.Name;
					string valueType = node.Name;

					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);

					if (valueType == "Start")
						value /= OpenConfig.SMELTING_RANGE;
					else if (valueType == "Stop")
						value *= OpenConfig.SMELTING_RANGE;
					else if (valueType == "Output")
						value *= OpenConfig.SMELTING_SUPERALLOY_OUTPUT;

					node.InnerText = Math.Clamp(value, 0f, 99999f).ToString(CultureInfo.InvariantCulture);
				}
			}

			// Arc Furnace changes
			if (editor.IsWhitelisted(new[] { "arcfurnace" }))
			{
				// Reduce required Time and Energy for smelting
				foreach (XmlNode node in editor.SelectNodes("//Time | //Energy"))
				{
					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);

					if (node.Name == "Time")
						value *= OpenConfig.SMELTING_TIME;
					else if (node.Name == "Energy")
						value *= OpenConfig.SMELTING_ENERGY;

					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
					string parentName = node.ParentNode.ParentNode.SelectSingleNode("PrefabName").InnerText;
				}
			}

			// Mineables changes
			if (editor.IsWhitelisted(new[] { "mineables", "terrainminablespresets", "worldsettings" }))
			{
				foreach (XmlNode node in editor.SelectNodes("//MineableData/*[number(text()) = number(text())] | //DeepMineableData/*[number(text()) = number(text())]"))
				{
					string DisplayName = node.ParentNode.SelectSingleNode("DisplayName").InnerText;
					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);

					if (node.Name == "MiningTime")
						value = (int)(value * OpenConfig.MINING_TIME); // Not sure if MiningTime can handle decimals, too lazy to try

					if (node.Name.Contains("DropQuantity"))
						value *= DisplayName switch
						{
							"Ice" or "Oxite" or "Volatiles" or "Nitrice" => OpenConfig.MINING_ICE_DROPQUANTITY,
							"Coal" => OpenConfig.MINING_COAL_DROPQUANTITY,
							_ => OpenConfig.MINING_ORE_DROPQUANTITY
						};

					if (node.Name.Contains("Vein"))
						value *= OpenConfig.MINING_VEIN_SIZE;

					// Which ores to remove from world
					foreach (string oreName in OpenConfig.EXCLUDE_VEINS)
						if (DisplayName == oreName) value = 0;

					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
				}
			}

			if (editor.IsWhitelisted(new[] { "worldsettings" }))
			{
				foreach (XmlNode node in editor.SelectNodes("//WorldOreDensity"))
				{
					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);
					value *= OpenConfig.WORLD_ORE_DENSITY;
					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
				}
			}
			string combinedPath;
			if (file.PathFromData == "")
			{
				combinedPath = Path.Combine(editor.gameDataPath, $"{file.NameExt}");
			}
			else
			{
				combinedPath = Path.Combine(editor.gameDataPath, $"{file.PathFromData}{file.NameExt}");
			}

			editor.Save(combinedPath);
		}
		MessageBox.Show($"{OpenConfigFile} built successfully!");
	}

	private void ReloadConfigList()
	{
		ConfigList.Items.Clear();
		foreach (string filePath in Directory.GetFiles(configDir, "*.xml"))
			ConfigList.Items.Add($"{Path.GetFileName(filePath)}");
	}

	public static void RecursiveCreateDirectory(string path)
	{
		string[] directories = path.Split(Path.DirectorySeparatorChar);

		string currentDirectory = directories[0];

		for (int i = 1; i < directories.Length; i++)
		{
			currentDirectory = Path.Combine(currentDirectory, directories[i]);
			if (!Directory.Exists(currentDirectory))
			{
				Directory.CreateDirectory(currentDirectory);
				Console.WriteLine($"Created directory: {currentDirectory}");
			}
		}
	}

	private void ConfigList_IndexChanged(object sender, EventArgs e) => SelectConfig(ConfigList.Text);

	private void ConfigList_Click(object sender, EventArgs e) => ReloadConfigList();

	private void BuildButton_Click(object sender, EventArgs e) => BuildSelectedConfig();

	private void HelpButton_Click(object sender, EventArgs e) => DumbJoke.Make(HelpButton);
}


