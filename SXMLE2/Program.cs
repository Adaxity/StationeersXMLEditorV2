internal static class Program
{
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();
		if (SXMLE2.IsInCorrectDirectory)
			Application.Run(new SXMLE2());
		else
			MessageBox.Show($"Please put 'SXMLE2.exe' into your Stationeers folder.\n(Into the same folder as 'rocketstation.exe')\n\nCurrently in: '{SXMLE2.exeDir}'", "Incorrect file location", MessageBoxButtons.OK);
	}
}