using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

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

	private void ConfigList_IndexChanged(object sender, EventArgs e)

	{
		SelectConfig(ConfigList.Text);
	}

	private void ConfigList_Click(object sender, EventArgs e)
	{
		if (!Directory.Exists(configDir))
			SXMLE2.RecursiveCreateDirectory(configDir);

		if (!File.Exists(Path.Combine(configDir, "default.xml")))
			using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "default.xml")))
				configSerializer.Serialize(writer, BuildConfig._default);

		using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "vanilla.xml")))
			configSerializer.Serialize(writer, BuildConfig._vanilla);

		SelectConfig("default.xml");
		ReloadConfigList();

	}

	private void BuildButton_Click(object sender, EventArgs e)
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
		BuildSelectedConfig();

	}

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
						case "Time":
							value = Math.Clamp(value * OpenConfig.CRAFTING_TIME, 0.001f, float.MaxValue);
							break;

						case "Energy":
							value *= OpenConfig.CRAFTING_ENERGY;
							break;

						default: // Materials
							value *= OpenConfig.CRAFTING_INGREDIENTS;
							break;
              
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
  
	private void ConfigList_IndexChanged(object sender, EventArgs e) => SelectConfig(ConfigList.Text);

	private void ConfigList_Click(object sender, EventArgs e) => ReloadConfigList();

	private void BuildButton_Click(object sender, EventArgs e) => BuildSelectedConfig();

	private void HelpButton_Click(object sender, EventArgs e) => DumbJoke.Make(HelpButton);
}