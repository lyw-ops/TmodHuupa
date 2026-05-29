using Terraria.ID;

namespace TmodHuupa.Content.Items.Consumables;

public class RoughRingCore : RingCoreItem
{
	protected override int RequiredLevelCap => 10;
	protected override int NewLevelCap => 20;
	protected override int CoreRarity => ItemRarityID.Blue;

	public override void AddRecipes()
	{
		CreateCoreRecipe(TileID.Anvils)
			.AddIngredient(ItemID.IronBar, 8)
			.AddIngredient(ItemID.Gel, 20)
			.AddIngredient(ItemID.FallenStar, 3)
			.Register();

		CreateCoreRecipe(TileID.Anvils)
			.AddIngredient(ItemID.LeadBar, 8)
			.AddIngredient(ItemID.Gel, 20)
			.AddIngredient(ItemID.FallenStar, 3)
			.Register();
	}
}
