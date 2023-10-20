using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public readonly struct StationeersXmlFile
{
	public readonly string FilePath;
	public readonly string FileName;
	public readonly string FileNameWithoutExtension;
	public readonly string FilePathFromData;

	public StationeersXmlFile(string FilePath)
	{
		this.FilePath = FilePath;
		this.FileName = Path.GetFileName(FilePath);
		this.FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
		this.FilePathFromData = ExtractPath(FilePath);
	}

	private string ExtractPath(string fullPath)
	{
		string directory = Path.GetDirectoryName(fullPath);
		string parentFolder = new DirectoryInfo(directory).Name;
		if (parentFolder.Equals("CustomPresets", StringComparison.OrdinalIgnoreCase))
			return Path.Combine(parentFolder, fullPath.Substring(directory.Length + 1));
		else
			return Path.GetFileName(fullPath);
	}
}

