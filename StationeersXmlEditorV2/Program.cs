using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

internal class Program
{
	public static Version version = Assembly.GetExecutingAssembly().GetName().Version;

	public static string exeDir = AppDomain.CurrentDomain.BaseDirectory;

	public static string configDir = Path.Combine(exeDir, "StationeersXmlEditorV2Configs");

	private static StationeersXMLEditor? editor;

	public static BuildConfig? bc;

	private static void Main(string[] args)
	{
		if (Directory.GetParent(exeDir).Name != "Stationeers")
		{
			Console.WriteLine("Please put the .exe file into the Stationeers folder.");
			Console.WriteLine($@"Current location: {exeDir}");
			Console.WriteLine($@"Press any key to exit...");
			Console.ReadKey();
			//return;
		}

		Console.WriteLine("Stationeers data found!\n");
		Console.WriteLine($"StationeersXmlEditor {version} by decxi - Use 'help' for a list of commands.");

		if (!Directory.Exists(configDir))
			Directory.CreateDirectory(configDir);

		XmlSerializer serializer = new XmlSerializer(typeof(BuildConfig));

		if (!Directory.Exists(Path.Combine(configDir, "default.xml"))) // regenerates file even when file exists, fix
			using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "default.xml")))
				serializer.Serialize(writer, BuildConfig._default);

		using (StreamWriter writer = new StreamWriter(Path.Combine(configDir, "vanilla.xml")))
			serializer.Serialize(writer, BuildConfig._vanilla);

		using (StreamReader reader = new StreamReader(Path.Combine(configDir, "default.xml")))
			bc = (BuildConfig)serializer.Deserialize(reader);

		string[] userInput = new string[16];

		while (true)
			try
			{
				for (int i = 0; i < userInput.Length; i++)
					userInput[i] = string.Empty;
				Console.Write("\n>> ");
				userInput[0] = Console.ReadLine().Trim();

				if (Regex.IsMatch(userInput[0], @"[\w-]+"))
				{
					MatchCollection matches = Regex.Matches(userInput[0], @"\w+");
					for (int i = 0; i < matches.Count; i++) userInput[i + 1] = matches[i].Value;
					userInput[1] = userInput[1].ToLower();
				}
				else
					continue;

				Console.WriteLine();

				switch (userInput[1])
				{
					case "quit":
					case "shutdown":
					case "stop":
					case "exit":
						return;

					case "help":
						Console.WriteLine("StationeersXmlEditor commands:");
						Console.WriteLine("\thelp: Show all commands");
						Console.WriteLine("\tinfo: Show info about StationeersXmlEditor");
						Console.WriteLine("\tbuild: Build xml files from config");
						Console.WriteLine("\tconfig: Show loaded build config.");
						Console.WriteLine("\tconfig [save/load] [filename]: Shows build config values.");
						Console.WriteLine("\tclear: Clears the console");
						Console.WriteLine("\texit: Close the program");
						break;

					case "build":
						Build();
						break;

					case "clear":
						Console.Clear();
						Console.WriteLine($"StationeersXmlEditor {version} by decxi - Use 'help' for a list of commands.");
						break;

					case "config":
						if (!string.IsNullOrEmpty(userInput[3]))
						{
							string xmlFilePath = Path.Combine(configDir, $"{userInput[3]}.xml");

							switch (userInput[2])
							{
								case "save":
									using (StreamWriter writer = new StreamWriter(xmlFilePath))
									{
										bc.CONFIG_NAME = userInput[3];
										serializer.Serialize(writer, bc);
										Console.WriteLine($"config '{userInput[3]}' created successfully!");
									}
									break;

								case "load":
									if (File.Exists(xmlFilePath))
										using (StreamReader reader = new StreamReader(xmlFilePath))
										{
											bc = (BuildConfig)serializer.Deserialize(reader);
											Console.WriteLine($"config '{userInput[3]}' loaded successfully!");
										}
									else
										Console.WriteLine($"config '{userInput[3]}' doesn't exist.");
									break;
							}
						}
						else
						{
							Console.WriteLine($"Current config: '{bc.CONFIG_NAME}'");
							Console.Write($"Available configs: ");
							List<string> files = new List<string>();
							foreach (string filePath in Directory.GetFiles(configDir, "*.xml"))
								files.Add($"'{Path.GetFileNameWithoutExtension(filePath)}'");
							Console.Write(string.Join(", ", files));
						}
						break;

					default:
						Console.WriteLine($"'{userInput[1]}' isn't a valid command. Use 'help' for a list of commands.");
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"ERROR: {e.Message}");
			}
	}

	public static void Build()
	{
		editor = new StationeersXMLEditor(exeDir);

		foreach (StationeersXmlFile file in editor.XmlFiles)
		{
			editor.Load(file.FilePath);
			editor.Log("Loading...", true);

			if (editor.ElementExists("stationeers_edited"))
			{
				editor.Log("already edited, skipping...");
				continue;
			}
			else
			{
				editor.DocumentElement.PrependChild(editor.CreateElement("stationeers_edited"));
				editor.Log($"Adding <stationeers_edited> tag...");
			}

			// Assemblers changes
			if (editor.IsWhitelisted(new[] { "autolathe", "DynamicObjectsFabricator", "electronics", "fabricator", "gascanisters", "organicsprinter", "paints", "PipeBender", "rocketmanufactory", "security", "toolmanufacturer" }))
			{
				// Remove all empty nodes
				foreach (XmlNode node in editor.SelectNodes("//*[text()='0']"))
				{
					string parentName = node.ParentNode.ParentNode.SelectSingleNode("PrefabName").InnerText;
					node.ParentNode.RemoveChild(node);
					editor.Log($"Removed empty {node.Name} tag of {parentName}");
				}

				// Change crafting values
				foreach (XmlNode node in editor.SelectNodes("//Recipe/*"))
				{
					float value = float.Parse(node.InnerText, CultureInfo.InvariantCulture);
					switch (node.Name)
					{
						case "Time":
							value = Math.Clamp(value * bc.CRAFTING_TIME_MULTIPLIER, 0.25f, 999f);
							break;

						case "Energy":
							value *= bc.CRAFTING_ENERGY_MULTIPLIER;
							break;

						default: // Materials
							value *= bc.CRAFTING_MATERIAL_MULTIPLIER;
							break;
					}
					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
					string parentName = node.ParentNode.ParentNode.SelectSingleNode("PrefabName").InnerText;
					editor.Log($"Reduced the {node.Name} of {parentName} to {value}");
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
						value /= bc.SMELTING_TOLERANCE_MULTIPLIER;
					else if (valueType == "Stop")
						value *= bc.SMELTING_TOLERANCE_MULTIPLIER;
					else if (valueType == "Output" && bc.SMELTING_NORMALIZE_SUPERALLOY_OUTPUT)
						value = 1f;

					node.InnerText = Math.Clamp(value, 0f, 99999f).ToString(CultureInfo.InvariantCulture);

					editor.Log($"Changed {type} {valueType} of {parentName} to {node.InnerText}");
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
						value *= bc.SMELTING_TIME_MULTIPLIER;
					else if (node.Name == "Energy")
						value *= bc.SMELTING_ENERGY_MULTIPLIER;

					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
					string parentName = node.ParentNode.ParentNode.SelectSingleNode("PrefabName").InnerText;
					editor.Log($"Reduced required {node.Name} of {parentName} to {node.InnerText}");
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
						value = (int)(value * bc.MINING_MULTIPLIER_TIME); // Not sure if MiningTime can handle decimals

					if (node.Name.Contains("DropQuantity"))
						value *= DisplayName switch
						{
							"Ice" or "Oxite" or "Volatiles" or "Nitrice" => bc.MINING_MULTIPLIER_ICE_DROPQUANTITY,
							"Coal" => bc.MINING_MULTIPLIER_COAL_DROPQUANTITY,
							_ => bc.MINING_MULTIPLIER_ORE_DROPQUANTITY
						};

					if (node.Name.Contains("Vein"))
						value *= bc.MINING_MULTIPLIER_VEIN_SIZE;

					// Which ores to remove from world
					foreach (string oreName in bc.EXCLUDE_VEINS)
						if (DisplayName == oreName) value = 0;

					node.InnerText = value.ToString(CultureInfo.InvariantCulture);
					editor.Log($"Set {node.Name} of {DisplayName} to {value}");
				}
			}
			editor.Log("Closing...\n", true);
			editor.Save(Path.Combine(editor.gameDataPath, file.FilePathFromData));
		}
		Console.WriteLine("Finished!");
	}
}