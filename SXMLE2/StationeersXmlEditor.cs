using System.Xml;

public class StationeersXMLEditor : XmlDocument
{
	private const string DataPath = @"rocketstation_Data\StreamingAssets\Data";
	private const string DataBackupPath = @"rocketstation_Data\StreamingAssets\DATA_ORIGINAL";

	public readonly string gamePath;
	public readonly string gameDataPath;
	public readonly string gameDataBackupPath;

	public List<StationeersXmlFile> XmlFiles = new();

	public StationeersXmlFile OpenFile
	{
		get
		{
			return new StationeersXmlFile(new Uri(BaseURI).LocalPath);
		}
	}

	public StationeersXMLEditor(string gamePath)
	{
		this.gamePath = gamePath;
		this.gameDataPath = Path.Combine(gamePath, DataPath);
		this.gameDataBackupPath = Path.Combine(gamePath, DataBackupPath);

		if (!Directory.Exists(gameDataBackupPath))
			Directory.CreateDirectory(Path.Combine(gameDataBackupPath));
		if (!Directory.Exists(Path.Combine(gameDataBackupPath, "CustomPresets")))
			Directory.CreateDirectory(Path.Combine(gameDataBackupPath, "CustomPresets"));

		BackupOriginalXmlFiles();

		XmlFiles = LoadXmlFilesFromPath(gameDataBackupPath);
	}

	private List<StationeersXmlFile> LoadXmlFilesFromPath(string path)
	{
		List<StationeersXmlFile> files = new();
		foreach (string Directory in Directory.GetDirectories(path))
			foreach (StationeersXmlFile file in LoadXmlFilesFromPath(Directory))
				files.Add(file);
		foreach (string XmlFilePath in Directory.GetFiles(path))
			if (Path.GetExtension(XmlFilePath) == ".xml")
				files.Add(new StationeersXmlFile(XmlFilePath));
		return files;
	}

	private void BackupOriginalXmlFiles()
	{
		Console.WriteLine("Backing up original files...");
		int i = 0;
		foreach (StationeersXmlFile xmlFile in LoadXmlFilesFromPath(gameDataPath))
		{
			Load(xmlFile.FilePath);
			if (!ElementExists("stationeers_edited"))
			{
				i++;
				Log($"Backed up", true);
				Save(Path.Combine(gameDataBackupPath, xmlFile.FilePathFromData));
			}
			else
			{
				Log($"Not original, skipping...", true);
			}
		}
		Console.WriteLine($"{i} original files have been backed up.\n");
	}

	public void Log(string change, bool includeFileName = false)
	{
		if (includeFileName)
			Console.WriteLine($"{OpenFile.FilePathFromData}: {change}");
		else
			Console.WriteLine($"\t{change}");
	}

	public bool ElementExists(string ElementName) => GetElementsByTagName(ElementName).Count > 0;

	public bool IsWhitelisted(string[] whitelist) => whitelist.Contains(OpenFile.FileNameWithoutExtension);
}