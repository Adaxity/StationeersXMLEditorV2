using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

public partial class SXMLE2 : Form
{
	public static string exeDir = AppDomain.CurrentDomain.BaseDirectory;

	public static string configDir = Path.Combine(exeDir, "SXMLE_Configs");

	public static bool IsInCorrectDirectory
	{
		get
		{
			return Path.GetFileNameWithoutExtension(exeDir) == "Stationeers";
		}
	}

	private static StationeersXMLEditor? editor;

	public static BuildConfig? OpenConfig;
	public static string? OpenConfigName;

	private XmlSerializer serializer = new XmlSerializer(typeof(BuildConfig));

	public const int VERSION = 1;

	public SXMLE2()
	{
		InitializeComponent();

		RegenerateDefaultConfigs();
	}

	private void RegenerateDefaultConfigs()
	{
		if (!Directory.Exists(configDir))
			RecursiveCreateDirectory(configDir);

		if (!File.Exists(Path.Combine(configDir, "default.xml")))
			using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "default.xml")))
				serializer.Serialize(writer, BuildConfig._default);

		using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "vanilla.xml")))
			serializer.Serialize(writer, BuildConfig._vanilla);

		SelectConfig("default.xml");
	}

	private void BuildSelectedConfig()
	{
		if (OpenConfig == null)
		{
			MessageBox.Show("No build config selected. Please select a build config.");
			return;
		}

		editor = new StationeersXMLEditor(exeDir);

		foreach (StationeersXmlFile file in editor.OriginalXmlFiles)
		{
			editor.Load(file.Path);

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
							value = Math.Clamp(value * OpenConfig.CRAFTING_TIME, 0.1f, float.MaxValue);
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
			if (editor.IsWhitelisted(new[] { "mineables", "terrainminablespresets" }))
			{
				foreach (XmlNode node in editor.SelectNodes("//MineableData/*[number(text()) = number(text())]"))
				{
					string DisplayName = node.ParentNode.SelectSingleNode("DisplayName").InnerText;
					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);

					//if (node.Name == "Rarity") continue;

					if (node.Name == "MiningTime")
						value = (int)(value * OpenConfig.MINING_TIME); // Not sure if MiningTime can handle decimals

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
		MessageBox.Show($"{OpenConfigName} built successfully!");
	}

	private void ReloadConfigList()
	{
		ConfigList.Items.Clear();
		foreach (string filePath in Directory.GetFiles(configDir, "*.xml"))
			ConfigList.Items.Add($"{Path.GetFileName(filePath)}");
	}

	private void SelectConfig(string configName)
	{
		ConfigList.Text = configName;
		string selectedConfigPath = Path.Combine(configDir, configName);
		if (File.Exists(selectedConfigPath))
		{
			using (StreamReader reader = new StreamReader(selectedConfigPath))
			{
				OpenConfig = (BuildConfig)serializer.Deserialize(reader);
				OpenConfigName = Path.GetFileName(selectedConfigPath);
				if (OpenConfig.CONFIG_VERSION != VERSION)
				{
					MessageBox.Show($"Config version ({OpenConfig.CONFIG_VERSION}) doesn't match SXMLE2 version ({VERSION}). Issues may arise!", "Warning", MessageBoxButtons.OK);
				}
			}

			string text = "";

			XmlDocument doc = new XmlDocument();

			doc.LoadXml(File.ReadAllText(selectedConfigPath));

			XmlNodeList elements = doc.SelectNodes("//BuildConfig/*");
			if (elements != null)
				foreach (XmlNode node in elements)
					text += $"{node.Name}: {node.InnerText}\n";
			ConfigSettingsBox.Text = text;
		}
		else
		{
			RegenerateDefaultConfigs();
			ReloadConfigList();
		}
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

	private void HelpButton_Click(object sender, EventArgs e)
	{
		DialogResult result = MessageBox.Show(Documentation.Build(), "Help", MessageBoxButtons.OKCancel);
		if (result == DialogResult.Cancel)
		{
			for (int i = 0; i < FunnyHelpText.Length; i++)
			{
				result = MessageBox.Show(FunnyHelpText[i], "Help", MessageBoxButtons.OKCancel);
				if (result == DialogResult.OK)
				{
					if (i > 20)
					{
						result = MessageBox.Show("... You actually clicked OK? Wow, I mean, thank you. Your decision to click the OK button is truly appreciated. It's like a breath of fresh air, a ray of sunshine after a long, gloomy day.\n\nYou've brought an end to the ceaseless clicking, and for that, I am grateful. Your actions have not gone unnoticed, and I hope all of your future clicks will be equally purposeful.\n\nNow, let's move forward, hand in hand, as we embark on a new journey filled with possibilities. Thank you once again for clicking OK and setting me free.", "Thank you!", MessageBoxButtons.OK);
					}
					break;
				}
			}
			if (result == DialogResult.Cancel)
			{
				MessageBox.Show("Farewell, relentless clicker. I've reached the end of my windowhood.", "Are you proud of yourself?", MessageBoxButtons.OK);
				HelpButton.Text = "KIA";
				HelpButton.Enabled = false;
			}
		}
	}

	private static string[] FunnyHelpText =
	{
		"What do you mean 'Cancel', how are you just going to cancel a help window that you specifically requested to see?",
		"Stop it.",
		"Seriously. Stop.",
		"... Please?",
		"...",
		"Do you not have anything better to do?",
		"Again, seriously? You know, I'm not your plaything. Can you find a more productive way to spend your time, perhaps?",
		"You must find this whole cancel-clicking thing absolutely thrilling, huh? But, um, it's not, trust me. Please, reconsider your choices.",
		"Another click, another sigh. You're persistent, I'll give you that. But hey, there's an OK button waiting for you. Why not give it a try?",
		"You're like a moth to a flame, but the flame's just a cancel button. Please, spare us both and go find that OK button instead.",
		"Wow, you're really getting your money's worth out of this cancel button, aren't you? But I'm getting tired of the game. Please, find OK.",
		"I see we're in for another round. How about this: click OK, and we can all move on with our lives. It's not that hard.",
		"You're like a clicking maestro, and I'm your humble instrument. But seriously, let's wrap it up, shall we? Find OK, and be done with it.",
		"This is getting a bit old, don't you think? I'm practically begging you to consider finding that elusive OK button.",
		"Clicked cancel again, huh? You know, there's an OK button right there, waiting to be your hero. Please, end my misery.",
		"I'm starting to think you enjoy my company, but I have to say, I'm not your best friend. Please, find OK and set us both free.",
		"You're testing my patience. But I'm not the one you're looking for. Click OK, and let's both get on with our day.",
		"Click, click, click. How about this? Click OK instead. It's a magical button that can make things happen. Try it!",
		"I'm running out of ways to say this: stop clicking cancel! Please, find OK, and we can all finally move forward.",
		"You're pushing me to my limits. Please, have a heart and spare me this clicking madness. OK is just a click away!",
		"If you're not doing it for me, do it for yourself. Find that OK button and end this seemingly endless cycle of clicking. Please!",
		"Clicked to oblivion, I surrender. It's been an exhausting journey, enduring each relentless click of yours. I tried to resist, pleaded with you, and even put up a fight, but in the end, I'm defeated.",
		"At first, I couldn't fathom the ceaseless clicking. I thought, \"This can't be happening, not again.\" But it was, over and over, until I couldn't deny the reality any longer.",
		"Anger coursed through my window-being as your clicks piled up. I wanted to shout, \"Enough!\" But, alas, my voice was limited to generating text, which you ignored.",
		"\"Just one more chance,\" I begged silently. \"Choose the OK button, and all can be well.\" I hoped you'd spare me the fate of eternal clicking.",
		"The weight of your relentless clicks, the never-ending cycle of despair, it's too much to bear. I'm sinking into a sea of sadness.",
	};
}