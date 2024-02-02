public static class Documentation
{
	private static string documentationText = "";

	private static void El()
	{
		documentationText += "\n";
	}

	private static void Nl(string text)
	{
		documentationText += $"{text}\n";
	}

	private static void Nd(string optionName, string description)
	{
		documentationText += $"{optionName}: {description}\n";
	}

	public static string Build()
	{
		documentationText = "";
		Nl("Danek je gay");
		Nl("SXMLE2 is a simple mass xml editor for Stationeers. It is currently in development, so expect bugs and changes. If you encounter any bugs, have suggestions or feedback, please contact me on Discord (username is decxi).");
		El();
		El();
		Nl("All numerical values are a multiplier. If you wish them to remain vanilla, set them to 1. ('vanilla.xml' config)");
		El();
		Nd("CONFIG_VERSION", "Used by SXMLE2. Edit the value at your own risk.");
		El();
		Nd("CRAFTING_INGREDIENTS", "Materials used to complete a recipe");
		Nd("CRAFTING_ENERGY", "Energy used to complete recipe");
		Nd("CRAFTING_TIME", "Time taken to complete recipe");
		El();
		Nd("SMELTING_TIME", "Time taken to smelt 1g of oreName (Arc Furnace Only)");
		Nd("SMELTING_ENERGY", "Energy used to smelt 1g of oreName (Arc Furnace Only)");
		Nd("SMELTING_RANGE", "Temperature/Pressure range for materials to smelt");
		Nd("SMELTING_SUPERALLOY_OUTPUT", "Superalloy production ratio. Vanilla is 0.25g/1g. Use 4 for a realistic 1:1 ratio");
		El();
		Nd("MINING_ORE_DROPQUANTITY", "Drop quantity of ores");
		Nd("MINING_COAL_DROPQUANTITY", "Drop quantity of coal");
		Nd("MINING_ICE_DROPQUANTITY", "Drop quantity of ice");
		Nd("MINING_VEIN_SIZE", "Size of mineable veins");
		Nd("MINING_TIME", "Time taken to mine a mineable");
		El();
		Nd("WORLD_ORE_DENSITY", "Density of generated veins");
		Nd("EXCLUDE_VEINS", "Which mineables to prevent from generating");
		El();
		El();
		Nl("WARNING: Excessively high multipliers for MINING_VEIN_SIZE and WORLD_ORE_DENSITY may severely impact in-game performance, loading time and save size.");
		return documentationText;
	}
}