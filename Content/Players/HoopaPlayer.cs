using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TmodHuupa.Content.Buffs;
using TmodHuupa.Content.Projectiles;

namespace TmodHuupa.Content.Players;

public class HoopaPlayer : ModPlayer
{
	public const int InitialRingSyncLevel = 1;
	public const int InitialRingSyncLevelCap = 10;
	public const int MaxRingSyncLevel = 100;

	private int capReminderCooldown;

	public int RingSyncLevel { get; private set; } = InitialRingSyncLevel;
	public int RingSyncExperience { get; private set; }
	public int RingSyncLevelCap { get; private set; } = InitialRingSyncLevelCap;

	public bool IsAtRingSyncCap => RingSyncLevel >= RingSyncLevelCap;
	public int ExperienceToNextRingSyncLevel => IsAtRingSyncCap ? 0 : GetExperienceForNextRingSyncLevel(RingSyncLevel);

	public override void Initialize()
	{
		RingSyncLevel = InitialRingSyncLevel;
		RingSyncExperience = 0;
		RingSyncLevelCap = InitialRingSyncLevelCap;
		capReminderCooldown = 0;
	}

	public override void ResetEffects()
	{
		if (capReminderCooldown > 0) {
			capReminderCooldown--;
		}
	}

	public override void SaveData(TagCompound tag)
	{
		tag["ringSyncLevel"] = RingSyncLevel;
		tag["ringSyncExperience"] = RingSyncExperience;
		tag["ringSyncLevelCap"] = RingSyncLevelCap;
	}

	public override void LoadData(TagCompound tag)
	{
		RingSyncLevel = Math.Clamp(tag.GetInt("ringSyncLevel"), InitialRingSyncLevel, MaxRingSyncLevel);
		RingSyncExperience = Math.Max(0, tag.GetInt("ringSyncExperience"));
		RingSyncLevelCap = Math.Clamp(tag.GetInt("ringSyncLevelCap"), InitialRingSyncLevelCap, MaxRingSyncLevel);

		if (RingSyncLevel > RingSyncLevelCap) {
			RingSyncLevel = RingSyncLevelCap;
		}
	}

	public bool HasHoopaSummoned()
	{
		return Player.HasBuff(ModContent.BuffType<HoopaMinionBuff>())
			|| Player.ownedProjectileCounts[ModContent.ProjectileType<HoopaMinionProjectile>()] > 0;
	}

	public bool TryGrantRingSyncExperience(NPC source, int amount)
	{
		if (amount <= 0 || !HasHoopaSummoned()) {
			return false;
		}

		if (IsAtRingSyncCap) {
			ShowCapReminder();
			return false;
		}

		RingSyncExperience += amount;
		ShowExperienceCombatText(source, amount);

		while (!IsAtRingSyncCap && RingSyncExperience >= ExperienceToNextRingSyncLevel) {
			RingSyncExperience -= ExperienceToNextRingSyncLevel;
			RingSyncLevel++;
			ShowLevelUpMessage();
		}

		if (IsAtRingSyncCap) {
			RingSyncExperience = 0;
			ShowCapReachedMessage();
		}

		return true;
	}

	public static int GetExperienceForNextRingSyncLevel(int level)
	{
		return 25 + level * 15 + level * level * 3;
	}

	private void ShowExperienceCombatText(NPC source, int amount)
	{
		if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) {
			return;
		}

		string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.ExperienceCombatText", amount);
		CombatText.NewText(source.getRect(), new Color(255, 215, 80), text);
	}

	private void ShowLevelUpMessage()
	{
		if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) {
			return;
		}

		string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.LevelUp", RingSyncLevel);
		Main.NewText(text, 255, 215, 80);
	}

	private void ShowCapReachedMessage()
	{
		if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) {
			return;
		}

		string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.LevelCapReached", RingSyncLevelCap);
		Main.NewText(text, 255, 150, 220);
		capReminderCooldown = 600;
	}

	private void ShowCapReminder()
	{
		if (capReminderCooldown > 0 || Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) {
			return;
		}

		string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.LevelCapReminder");
		Main.NewText(text, 255, 150, 220);
		capReminderCooldown = 600;
	}
}
