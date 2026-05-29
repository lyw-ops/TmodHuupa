using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TmodHuupa.Content.Players;

namespace TmodHuupa.Content.Items.Consumables;

public abstract class RingCoreItem : ModItem
{
	protected abstract int RequiredLevelCap { get; }
	protected abstract int NewLevelCap { get; }
	protected abstract int CoreRarity { get; }
	protected virtual int SellValue => Item.sellPrice(silver: 25);

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.UseSound = SoundID.Item4;
		Item.consumable = true;
		Item.value = SellValue;
		Item.rare = CoreRarity;
	}

	public override bool CanUseItem(Player player)
	{
		HoopaPlayer hoopaPlayer = player.GetModPlayer<HoopaPlayer>();
		if (hoopaPlayer.CanUnlockRingSyncLevelCap(RequiredLevelCap, NewLevelCap)) {
			return true;
		}

		if (player.whoAmI == Main.myPlayer) {
			ShowCannotUseMessage(hoopaPlayer);
		}

		return false;
	}

	public override bool? UseItem(Player player)
	{
		HoopaPlayer hoopaPlayer = player.GetModPlayer<HoopaPlayer>();
		if (!hoopaPlayer.TryUnlockRingSyncLevelCap(RequiredLevelCap, NewLevelCap)) {
			return false;
		}

		if (player.whoAmI == Main.myPlayer) {
			string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.RingCoreBreakthrough", NewLevelCap);
			Main.NewText(text, 255, 215, 80);
			CombatText.NewText(player.getRect(), new Color(255, 215, 80), $"Lv.{NewLevelCap}");
		}

		return true;
	}

	protected Recipe CreateCoreRecipe(int tileId)
	{
		Recipe recipe = CreateRecipe();
		recipe.AddTile(tileId);
		return recipe;
	}

	private void ShowCannotUseMessage(HoopaPlayer hoopaPlayer)
	{
		string key;
		if (hoopaPlayer.RingSyncLevelCap >= NewLevelCap) {
			key = "Mods.TmodHuupa.HoopaExperience.RingCoreAlreadyUsed";
		}
		else if (hoopaPlayer.RingSyncLevelCap == RequiredLevelCap) {
			key = "Mods.TmodHuupa.HoopaExperience.RingCoreNeedsLevel";
		}
		else {
			key = "Mods.TmodHuupa.HoopaExperience.RingCoreWrongStage";
		}

		Main.NewText(Language.GetTextValue(key, RequiredLevelCap), 255, 150, 220);
	}
}
