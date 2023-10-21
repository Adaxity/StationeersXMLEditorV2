[Serializable]
public class BuildConfig
{
	public int CONFIG_VERSION;

	public float CRAFTING_INGREDIENTS;
	public float CRAFTING_ENERGY;
	public float CRAFTING_TIME;

	public float SMELTING_TIME;
	public float SMELTING_ENERGY;
	public float SMELTING_RANGE;
	public float SMELTING_SUPERALLOY_OUTPUT;

	public float MINING_ORE_DROPQUANTITY;
	public float MINING_COAL_DROPQUANTITY;
	public float MINING_ICE_DROPQUANTITY;
	public float MINING_VEIN_SIZE;
	public float MINING_TIME;

	public float WORLD_ORE_DENSITY;

	public string[]? EXCLUDE_VEINS;

	public static readonly BuildConfig _vanilla = new BuildConfig()
	{
		CONFIG_VERSION = SXMLE2.VERSION,
		CRAFTING_INGREDIENTS = 1f,
		CRAFTING_ENERGY = 1f,
		CRAFTING_TIME = 1f,
		SMELTING_TIME = 1f,
		SMELTING_ENERGY = 1f,
		SMELTING_RANGE = 1f,
		SMELTING_SUPERALLOY_OUTPUT = 1f,
		MINING_ORE_DROPQUANTITY = 1f,
		MINING_COAL_DROPQUANTITY = 1f,
		MINING_ICE_DROPQUANTITY = 1f,
		MINING_VEIN_SIZE = 1f,
		MINING_TIME = 1f,
		WORLD_ORE_DENSITY = 1f,
		EXCLUDE_VEINS = new string[] { "Geyser" }
	};

	public static readonly BuildConfig _default = new BuildConfig()
	{
		CONFIG_VERSION = SXMLE2.VERSION,
		CRAFTING_INGREDIENTS = 1f,
		CRAFTING_ENERGY = 1f,
		CRAFTING_TIME = 0.1f,
		SMELTING_TIME = 0.1f,
		SMELTING_ENERGY = 0.1f,
		SMELTING_RANGE = 2f,
		SMELTING_SUPERALLOY_OUTPUT = 4f,
		MINING_ORE_DROPQUANTITY = 2f,
		MINING_COAL_DROPQUANTITY = 2f,
		MINING_ICE_DROPQUANTITY = 5f,
		MINING_VEIN_SIZE = 2f,
		MINING_TIME = 0f,
		WORLD_ORE_DENSITY = 2f,
		EXCLUDE_VEINS = new string[] { "Geyser", "Uranium" }
	};
}