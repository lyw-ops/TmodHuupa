using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
	public const int InitialBondLevel = 1;
	public const int MaxBondLevel = 5;
	public const int SpatialDodgeUnlockLevel = 20;
	public const int SpatialDodgeCooldownSeconds = 90;

	private const int SpatialDodgeCooldownTicks = SpatialDodgeCooldownSeconds * 60;
	private const int SpatialDodgeInvulnerabilityTicks = 60;

	private int capReminderCooldown;
	private int spatialDodgeCooldown;
	private readonly HashSet<string> witnessedBosses = new();

	public int RingSyncLevel { get; private set; } = InitialRingSyncLevel;
	public int RingSyncExperience { get; private set; }
	public int RingSyncLevelCap { get; private set; } = InitialRingSyncLevelCap;
	public int BondLevel { get; private set; } = InitialBondLevel;

	public bool IsAtRingSyncCap => RingSyncLevel >= RingSyncLevelCap;
	public int ExperienceToNextRingSyncLevel => IsAtRingSyncCap ? 0 : GetExperienceForNextRingSyncLevel(RingSyncLevel);
	public bool HasSpatialDodgeUnlocked => RingSyncLevel >= SpatialDodgeUnlockLevel;
	public bool IsSpatialDodgeReady => HasSpatialDodgeUnlocked && spatialDodgeCooldown <= 0;
	public int SpatialDodgeCooldownSecondsRemaining => (spatialDodgeCooldown + 59) / 60;

	public override void Initialize()
	{
		RingSyncLevel = InitialRingSyncLevel;
		RingSyncExperience = 0;
		RingSyncLevelCap = InitialRingSyncLevelCap;
		BondLevel = InitialBondLevel;
		witnessedBosses.Clear();
		capReminderCooldown = 0;
		spatialDodgeCooldown = 0;
	}

	public override void ResetEffects()
	{
		if (capReminderCooldown > 0) {
			capReminderCooldown--;
		}

		if (spatialDodgeCooldown > 0) {
			spatialDodgeCooldown--;
		}
	}

	public override bool ConsumableDodge(Player.HurtInfo info)
	{
		if (!CanUseSpatialDodge(info)) {
			return false;
		}

		spatialDodgeCooldown = SpatialDodgeCooldownTicks;
		Player.immune = true;
		Player.AddImmuneTime(ImmunityCooldownID.General, SpatialDodgeInvulnerabilityTicks);
		ShowSpatialDodgeFeedback();
		return true;
	}

	public override void SaveData(TagCompound tag)
	{
		tag["ringSyncLevel"] = RingSyncLevel;
		tag["ringSyncExperience"] = RingSyncExperience;
		tag["ringSyncLevelCap"] = RingSyncLevelCap;
		tag["bondLevel"] = BondLevel;
		tag["witnessedBosses"] = new List<string>(witnessedBosses);
	}

	public override void LoadData(TagCompound tag)
	{
		RingSyncLevel = Math.Clamp(tag.GetInt("ringSyncLevel"), InitialRingSyncLevel, MaxRingSyncLevel);
		RingSyncExperience = Math.Max(0, tag.GetInt("ringSyncExperience"));
		RingSyncLevelCap = Math.Clamp(tag.GetInt("ringSyncLevelCap"), InitialRingSyncLevelCap, MaxRingSyncLevel);
		BondLevel = Math.Clamp(tag.GetInt("bondLevel"), InitialBondLevel, MaxBondLevel);
		witnessedBosses.Clear();

		foreach (string bossKey in tag.GetList<string>("witnessedBosses")) {
			witnessedBosses.Add(bossKey);
		}

		if (RingSyncLevel > RingSyncLevelCap) {
			RingSyncLevel = RingSyncLevelCap;
		}
	}

	public bool HasHoopaSummoned()
	{
		return Player.HasBuff(ModContent.BuffType<HoopaMinionBuff>())
			|| Player.ownedProjectileCounts[ModContent.ProjectileType<HoopaMinionProjectile>()] > 0;
	}

	public bool CanUnlockRingSyncLevelCap(int requiredLevelCap, int newLevelCap)
	{
		return RingSyncLevelCap == requiredLevelCap
			&& RingSyncLevel >= requiredLevelCap
			&& newLevelCap > RingSyncLevelCap
			&& newLevelCap <= MaxRingSyncLevel;
	}

	public bool TryUnlockRingSyncLevelCap(int requiredLevelCap, int newLevelCap)
	{
		if (!CanUnlockRingSyncLevelCap(requiredLevelCap, newLevelCap)) {
			return false;
		}

		RingSyncLevelCap = newLevelCap;
		return true;
	}

	public bool TryGrantRingSyncExperience(NPC source, int amount)
	{
		if (amount <= 0 || !HasHoopaSummoned()) {
			return false;
		}

		return TryAddRingSyncExperience(source, amount, showCombatText: true);
	}

	public bool TryGrantDebugRingSyncExperience(int amount)
	{
		if (amount <= 0) {
			return false;
		}

		return TryAddRingSyncExperience(null, amount, showCombatText: false);
	}

	private bool TryAddRingSyncExperience(NPC source, int amount, bool showCombatText)
	{
		if (IsAtRingSyncCap) {
			ShowCapReminder();
			return false;
		}

		RingSyncExperience += amount;
		if (showCombatText && source != null) {
			ShowExperienceCombatText(source, amount);
		}

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

	public bool TryClaimBossWitnessReward(NPC source, string bossKey, string bossName, int ringSyncExperience, int bondLevelTarget)
	{
		if (witnessedBosses.Contains(bossKey)) {
			return false;
		}

		witnessedBosses.Add(bossKey);
		TryGrantRingSyncExperience(source, ringSyncExperience);

		if (Player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server) {
			string rewardText = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.BossWitnessed", bossName, ringSyncExperience);
			Main.NewText(rewardText, 255, 215, 80);
		}

		TryRaiseBondLevel(bondLevelTarget);
		return true;
	}

	private bool CanUseSpatialDodge(Player.HurtInfo info)
	{
		return info.Dodgeable
			&& !Player.dead
			&& HasHoopaSummoned()
			&& HasSpatialDodgeUnlocked
			&& spatialDodgeCooldown <= 0
			&& Player.statLife >= Player.statLifeMax2;
	}

	public static int GetExperienceForNextRingSyncLevel(int level)
	{
		return 25 + level * 15 + level * level * 3;
	}

	private bool TryRaiseBondLevel(int targetLevel)
	{
		targetLevel = Math.Clamp(targetLevel, InitialBondLevel, MaxBondLevel);
		if (targetLevel <= BondLevel) {
			return false;
		}

		BondLevel = targetLevel;
		if (Player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server) {
			string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.BondLevelUp", BondLevel);
			Main.NewText(text, 255, 150, 220);
		}

		return true;
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

	private void ShowSpatialDodgeFeedback()
	{
		if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) {
			return;
		}

		string text = Language.GetTextValue("Mods.TmodHuupa.HoopaExperience.SpatialDodge");
		CombatText.NewText(Player.getRect(), new Color(255, 215, 80), text, true);
		SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.6f, Pitch = 0.45f }, Player.Center);

		for (int i = 0; i < 18; i++) {
			Vector2 velocity = (MathHelper.TwoPi * i / 18f).ToRotationVector2() * 2.5f;
			Dust dust = Dust.NewDustPerfect(Player.Center, DustID.GoldFlame, velocity, 120, new Color(255, 210, 70), 1.1f);
			dust.noGravity = true;
		}
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
