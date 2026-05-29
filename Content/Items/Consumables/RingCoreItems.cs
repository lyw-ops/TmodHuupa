using Terraria.ID;

namespace TmodHuupa.Content.Items.Consumables;

public class GleamingRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 20;
	protected override int NewLevelCap => 30;
	protected override int CoreRarity => ItemRarityID.Green;

	public override void AddRecipes()
	{
		foreach (int stabilizingBar in new[] { ItemID.SilverBar, ItemID.TungstenBar }) {
			foreach (int structureBar in new[] { ItemID.GoldBar, ItemID.PlatinumBar }) {
				CreateCoreRecipe(TileID.Anvils)
					.AddIngredient(stabilizingBar, 8)
					.AddIngredient(structureBar, 6)
					.AddIngredient(ItemID.Amethyst, 3)
					.AddIngredient(ItemID.FallenStar, 5)
					.Register();
			}
		}
	}
}

public class AlteredRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 30;
	protected override int NewLevelCap => 40;
	protected override int CoreRarity => ItemRarityID.Orange;

	public override void AddRecipes()
	{
		CreateCoreRecipe(TileID.Anvils)
			.AddIngredient(ItemID.DemoniteBar, 10)
			.AddIngredient(ItemID.ShadowScale, 8)
			.AddIngredient(ItemID.FallenStar, 5)
			.Register();

		CreateCoreRecipe(TileID.Anvils)
			.AddIngredient(ItemID.CrimtaneBar, 10)
			.AddIngredient(ItemID.TissueSample, 8)
			.AddIngredient(ItemID.FallenStar, 5)
			.Register();
	}
}

public class MoltenRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 40;
	protected override int NewLevelCap => 50;
	protected override int CoreRarity => ItemRarityID.LightRed;

	public override void AddRecipes()
	{
		CreateCoreRecipe(TileID.Anvils)
			.AddIngredient(ItemID.HellstoneBar, 12)
			.AddIngredient(ItemID.Obsidian, 20)
			.AddIngredient(ItemID.Fireblossom, 2)
			.Register();
	}
}

public class HardenedRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 50;
	protected override int NewLevelCap => 60;
	protected override int CoreRarity => ItemRarityID.Pink;

	public override void AddRecipes()
	{
		foreach (int bar in new[] { ItemID.CobaltBar, ItemID.PalladiumBar }) {
			CreateCoreRecipe(TileID.MythrilAnvil)
				.AddIngredient(bar, 12)
				.AddIngredient(ItemID.SoulofLight, 6)
				.AddIngredient(ItemID.SoulofNight, 6)
				.Register();
		}
	}
}

public class ArcaneRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 60;
	protected override int NewLevelCap => 70;
	protected override int CoreRarity => ItemRarityID.LightPurple;

	public override void AddRecipes()
	{
		foreach (int bar in new[] { ItemID.MythrilBar, ItemID.OrichalcumBar }) {
			CreateCoreRecipe(TileID.MythrilAnvil)
				.AddIngredient(bar, 12)
				.AddIngredient(ItemID.SoulofSight, 3)
				.AddIngredient(ItemID.SoulofMight, 3)
				.AddIngredient(ItemID.SoulofFright, 3)
				.Register();
		}
	}
}

public class TitanRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 70;
	protected override int NewLevelCap => 80;
	protected override int CoreRarity => ItemRarityID.Lime;

	public override void AddRecipes()
	{
		foreach (int bar in new[] { ItemID.AdamantiteBar, ItemID.TitaniumBar }) {
			CreateCoreRecipe(TileID.MythrilAnvil)
				.AddIngredient(bar, 12)
				.AddIngredient(ItemID.HallowedBar, 10)
				.Register();
		}
	}
}

public class VerdantRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 80;
	protected override int NewLevelCap => 90;
	protected override int CoreRarity => ItemRarityID.Yellow;

	public override void AddRecipes()
	{
		CreateCoreRecipe(TileID.MythrilAnvil)
			.AddIngredient(ItemID.ChlorophyteBar, 12)
			.AddIngredient(ItemID.LifeFruit)
			.AddIngredient(ItemID.LihzahrdBrick, 25)
			.Register();
	}
}

public class AstralRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 90;
	protected override int NewLevelCap => 100;
	protected override int CoreRarity => ItemRarityID.Red;

	public override void AddRecipes()
	{
		CreateCoreRecipe(TileID.LunarCraftingStation)
			.AddIngredient(ItemID.FragmentSolar, 8)
			.AddIngredient(ItemID.FragmentVortex, 8)
			.AddIngredient(ItemID.FragmentNebula, 8)
			.AddIngredient(ItemID.FragmentStardust, 8)
			.AddIngredient(ItemID.LunarBar, 6)
			.Register();
	}
}
