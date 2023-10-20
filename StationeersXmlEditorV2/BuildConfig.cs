using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class BuildConfig
{
	public string CONFIG_NAME;

	public float CRAFTING_MATERIAL_MULTIPLIER;        // materials used to complete recipe

	public float CRAFTING_ENERGY_MULTIPLIER;          // energy used to complete recipe
	public float CRAFTING_TIME_MULTIPLIER;            // time taken to complete recipe

	public float SMELTING_TIME_MULTIPLIER;            // amount of time taken to smelt 1g of oreName. (Arc Furnace Only)
	public float SMELTING_ENERGY_MULTIPLIER;          // amount of energy used to smelt 1g of oreName. (Arc Furnace Only)

	public float SMELTING_TOLERANCE_MULTIPLIER;       // more/less temp/press error allowed for materials to smelt
	public bool SMELTING_NORMALIZE_SUPERALLOY_OUTPUT; // Corrects superalloy production ratios. 1g = 1g, not 0.25g

	public float MINING_MULTIPLIER_ORE_DROPQUANTITY;          // drop quantity of ores
	public float MINING_MULTIPLIER_COAL_DROPQUANTITY; // drop quantity of coal
	public float MINING_MULTIPLIER_ICE_DROPQUANTITY;  // drop quantity of ice

	public float MINING_MULTIPLIER_VEIN_SIZE;         // size of mineable veins
	public float MINING_MULTIPLIER_TIME;              // time taken to mine a mineable

	public string[]? EXCLUDE_VEINS;                   // Which veins of mineables to prevent from generating

	public static readonly BuildConfig _vanilla = new BuildConfig()
	{
		CONFIG_NAME = "vanilla",
		CRAFTING_MATERIAL_MULTIPLIER = 1f,
		CRAFTING_ENERGY_MULTIPLIER = 1f,
		CRAFTING_TIME_MULTIPLIER = 1f,
		SMELTING_TIME_MULTIPLIER = 1f,
		SMELTING_ENERGY_MULTIPLIER = 1f,
		SMELTING_TOLERANCE_MULTIPLIER = 1f,
		SMELTING_NORMALIZE_SUPERALLOY_OUTPUT = false,
		MINING_MULTIPLIER_ORE_DROPQUANTITY = 1f,
		MINING_MULTIPLIER_COAL_DROPQUANTITY = 1f,
		MINING_MULTIPLIER_ICE_DROPQUANTITY = 1f,
		MINING_MULTIPLIER_VEIN_SIZE = 1f,
		MINING_MULTIPLIER_TIME = 1f,
		EXCLUDE_VEINS = new string[] { "Geyser" }
	};

	public static readonly BuildConfig _default = new BuildConfig()
	{
		CONFIG_NAME = "default",
		CRAFTING_MATERIAL_MULTIPLIER = 1f,
		CRAFTING_ENERGY_MULTIPLIER = 1f,
		CRAFTING_TIME_MULTIPLIER = 0.1f,
		SMELTING_TIME_MULTIPLIER = 0.1f,
		SMELTING_ENERGY_MULTIPLIER = 0.1f,
		SMELTING_TOLERANCE_MULTIPLIER = 2f,
		SMELTING_NORMALIZE_SUPERALLOY_OUTPUT = true,
		MINING_MULTIPLIER_ORE_DROPQUANTITY = 2f,
		MINING_MULTIPLIER_COAL_DROPQUANTITY = 2f,
		MINING_MULTIPLIER_ICE_DROPQUANTITY = 5f,
		MINING_MULTIPLIER_VEIN_SIZE = 2f,
		MINING_MULTIPLIER_TIME = 0f,
		EXCLUDE_VEINS = new string[] { "Geyser", "Uranium" }
	};
}

