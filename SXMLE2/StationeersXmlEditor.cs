using System.Xml;

public class StationeersXMLEditor : XmlDocument
{
	public const string DataPath = @"rocketstation_Data\StreamingAssets\Data";
	public const string DataBackupPath = @"rocketstation_Data\StreamingAssets\DATA_ORIGINAL";

	public readonly string gamePath;
	public readonly string gameDataPath;
	public readonly string gameDataBackupPath;

	public List<StationeersXmlFile> OriginalXmlFiles = new();

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
			SXMLE2.RecursiveCreateDirectory(Path.Combine(gameDataBackupPath));

		BackupOriginalXmlFiles();

		OriginalXmlFiles = LoadXmlFilesFromPath(gameDataBackupPath);
	}

	private List<StationeersXmlFile> LoadXmlFilesFromPath(string path)
	{
		List<string> paths = FindAllPaths(path);
		List<StationeersXmlFile> files = new();

		foreach (string dir in paths)
			foreach (string XmlFilePath in Directory.GetFiles(dir))
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
			string combinedPath = Path.Combine(gameDataBackupPath + $"\\{xmlFile.PathFromData}");

			SXMLE2.RecursiveCreateDirectory(combinedPath);
		}

		foreach (StationeersXmlFile xmlFile in LoadXmlFilesFromPath(gameDataPath))
		{
			string combinedPath;
			if (xmlFile.PathFromData == "")
				combinedPath = gameDataBackupPath;
			else
				combinedPath = Path.Combine(gameDataBackupPath, xmlFile.PathFromData);

			Load(xmlFile.Path);
			if (!ElementExists("stationeers_edited"))
			{
				Save($"{combinedPath}\\{xmlFile.NameExt}");
				i++;
				Log($"Backed up", true);
			}
			else
			{
				Log($"Not original, skipping...", true);
			}
			Console.WriteLine($"{i} original files have been backed up.\n");
		}
	}

	public void Log(string change, bool includeFileName = false)
	{
		if (includeFileName)
			Console.WriteLine($"{OpenFile.PathFromData}: {change}");
		else
			Console.WriteLine($"\t{change}");
	}

	private static List<string> FindAllPaths(string directory)
	{
		List<string> allDirectories = new List<string>();
		allDirectories.Add(directory);
		foreach (string subdirectory in Directory.GetDirectories(directory))
			allDirectories.AddRange(FindAllPaths(subdirectory));
		return allDirectories;
	}

	public bool ElementExists(string ElementName) => GetElementsByTagName(ElementName).Count > 0;

	public bool IsWhitelisted(string[] whitelist) => whitelist.Contains(OpenFile.Name);
}