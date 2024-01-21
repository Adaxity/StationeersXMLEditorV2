internal static class Program
{
	[STAThread]
	private static void Main()
	{
		ApplicationConfiguration.Initialize();

#if DEBUG
		SXMLE2.exeDir = @"C:\Program Files (x86)\Steam\steamapps\common\Stationeers";
		SXMLE2.configDir = SXMLE2.exeDir + @"\SXMLE_Configs";
#endif

		if (SXMLE2.IsInCorrectDirectory)
			Application.Run(new SXMLE2());
		else
			MessageBox.Show($"Please put the .exe file into your Stationeers folder.\n\nCurrently in: {SXMLE2.exeDir}", "Incorrect file location", MessageBoxButtons.OK);
	}
}