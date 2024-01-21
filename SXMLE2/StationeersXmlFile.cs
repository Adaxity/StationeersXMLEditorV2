using System.IO;

public readonly struct StationeersXmlFile
{
	public readonly string Path;
	public readonly string Name;
	public readonly string NameExt;
	public readonly string PathFromData;

	public StationeersXmlFile(string FilePath)
	{
		Path = FilePath;
		Name = System.IO.Path.GetFileNameWithoutExtension(Path);
		NameExt = System.IO.Path.GetFileName(Path);
		PathFromData = ExtractDataPath(Path);
	}

	private string ExtractDataPath(string fullPath)
	{
		string path = System.IO.Path.GetRelativePath(SXMLE2.exeDir + "\\rocketstation_Data\\StreamingAssets\\", fullPath);
		if (path.StartsWith("Data"))
			path = path.Replace("Data\\", "");
		if (path.StartsWith("DATA_ORIGINAL"))
			path = path.Replace("DATA_ORIGINAL\\", "");
		path = path.Replace(NameExt, "");
		return path;
	}
}