using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TmodHuupa.Content.Players;

namespace TmodHuupa.Content.Items.Tools;

public class HoopaDebugCompass : ModItem
{
	private const int DebugExperienceAmount = 500;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.UseSound = SoundID.Item4;
		Item.rare = ItemRarityID.Gray;
		Item.value = 0;
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override bool? UseItem(Player player)
	{
		HoopaPlayer hoopaPlayer = player.GetModPlayer<HoopaPlayer>();
		if (player.altFunctionUse == 2) {
			ShowStatus(hoopaPlayer);
			return true;
		}

		if (hoopaPlayer.TryGrantDebugRingSyncExperience(DebugExperienceAmount)) {
			Main.NewText(Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.DebugGrant", DebugExperienceAmount), 255, 215, 80);
		}

		ShowStatus(hoopaPlayer);
		return true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.DirtBlock)
			.Register();
	}

	private static void ShowStatus(HoopaPlayer hoopaPlayer)
	{
		string key = hoopaPlayer.IsAtRingSyncCap
			? "Mods.TmodHuupa.HoopaExperience.DebugStatusAtCap"
			: "Mods.TmodHuupa.HoopaExperience.DebugStatus";

		string text = Language.GetTextValue(
			key,
			hoopaPlayer.RingSyncLevel,
			hoopaPlayer.RingSyncLevelCap,
			hoopaPlayer.RingSyncExperience,
			hoopaPlayer.ExperienceToNextRingSyncLevel,
			hoopaPlayer.BondLevel,
			HoopaPlayer.MaxBondLevel);

		Main.NewText(text, 180, 220, 255);
		Main.NewText(GetSpatialDodgeStatus(hoopaPlayer), 255, 215, 80);
	}

	private static string GetSpatialDodgeStatus(HoopaPlayer hoopaPlayer)
	{
		if (!hoopaPlayer.HasSpatialDodgeUnlocked) {
			return Language.GetTextValue(
				"Mods.TmodHuupa.HoopaExperience.DebugSpatialDodgeLocked",
				HoopaPlayer.SpatialDodgeUnlockLevel,
				HoopaPlayer.SpatialDodgeCooldownSeconds);
		}

		if (hoopaPlayer.IsSpatialDodgeReady) {
			return Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.DebugSpatialDodgeReady");
		}

		return Language.GetTextValue(
			"Mods.TmodHuupa.HoopaExperience.DebugSpatialDodgeCooldown",
			hoopaPlayer.SpatialDodgeCooldownSecondsRemaining);
	}
}
