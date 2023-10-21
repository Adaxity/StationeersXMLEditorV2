internal static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();
		if (SXMLE2.IsInCorrectDirectory)
			Application.Run(new SXMLE2());
		else
			MessageBox.Show($"Please put the .exe file into your Stationeers folder.\n\nCurrently in: {SXMLE2.exeDir}", "Incorrect file location", MessageBoxButtons.OK);
	}
}