using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Players;
using TmodHuupa.Content.Projectiles;

namespace TmodHuupa.Content.Globals;

public class HoopaExperienceGlobalNPC : GlobalNPC
{
	private int hoopaExperienceOwner = -1;

	public override bool InstancePerEntity => true;

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
		hoopaExperienceOwner = -1;
	}

	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		if (projectile.type == ModContent.ProjectileType<HoopaRingBoltProjectile>() && IsValidPlayer(projectile.owner)) {
			hoopaExperienceOwner = projectile.owner;
		}
	}

	public override void OnKill(NPC npc)
	{
		if (!CanGrantExperience(npc)) {
			return;
		}

		Player player = GetExperiencePlayer(npc);
		if (!player.active || player.dead) {
			return;
		}

		HoopaPlayer hoopaPlayer = player.GetModPlayer<HoopaPlayer>();
		if (!hoopaPlayer.HasHoopaSummoned()) {
			return;
		}

		int experience = CalculateRingSyncExperience(npc, hoopaPlayer);
		hoopaPlayer.TryGrantRingSyncExperience(npc, experience);
		TryGrantBossWitnessReward(npc, hoopaPlayer);
	}

	private static bool CanGrantExperience(NPC npc)
	{
		return !npc.friendly
			&& !npc.townNPC
			&& !npc.dontTakeDamage
			&& npc.lifeMax > 5
			&& (npc.damage > 0 || npc.boss)
			&& !npc.SpawnedFromStatue;
	}

	private Player GetExperiencePlayer(NPC npc)
	{
		int playerIndex = IsValidPlayer(hoopaExperienceOwner) ? hoopaExperienceOwner : npc.lastInteraction;
		if (!IsValidPlayer(playerIndex)) {
			playerIndex = Player.FindClosest(npc.position, npc.width, npc.height);
		}

		return Main.player[playerIndex];
	}

	private static bool IsValidPlayer(int playerIndex)
	{
		return playerIndex >= 0 && playerIndex < Main.maxPlayers && Main.player[playerIndex].active;
	}

	private static int CalculateRingSyncExperience(NPC npc, HoopaPlayer hoopaPlayer)
	{
		int baseExperience = GetBaseExperience(npc);
		float stageMultiplier = GetStageMultiplier(npc, hoopaPlayer.RingSyncLevel);
		if (stageMultiplier <= 0f) {
			return 0;
		}

		return Math.Max(1, (int)MathF.Round(baseExperience * stageMultiplier));
	}

	private static void TryGrantBossWitnessReward(NPC npc, HoopaPlayer hoopaPlayer)
	{
		if (!TryGetBossWitnessReward(npc, out string bossKey, out int experience, out int bondLevelTarget)) {
			return;
		}

		hoopaPlayer.TryClaimBossWitnessReward(npc, bossKey, npc.FullName, experience, bondLevelTarget);
	}

	private static bool TryGetBossWitnessReward(NPC npc, out string bossKey, out int experience, out int bondLevelTarget)
	{
		bossKey = string.Empty;
		experience = 0;
		bondLevelTarget = HoopaPlayer.InitialBondLevel;

		switch (npc.type) {
			case NPCID.KingSlime:
				bossKey = "kingSlime";
				experience = 120;
				return true;
			case NPCID.EyeofCthulhu:
				bossKey = "eyeOfCthulhu";
				experience = 180;
				bondLevelTarget = 2;
				return true;
			case NPCID.EaterofWorldsHead:
			case NPCID.BrainofCthulhu:
				bossKey = "evilBoss";
				experience = 260;
				bondLevelTarget = 2;
				return true;
			case NPCID.QueenBee:
				bossKey = "queenBee";
				experience = 280;
				return true;
			case NPCID.SkeletronHead:
				bossKey = "skeletron";
				experience = 320;
				return true;
			case NPCID.WallofFlesh:
			case NPCID.WallofFleshEye:
				bossKey = "wallOfFlesh";
				experience = 650;
				bondLevelTarget = 3;
				return true;
			case NPCID.TheDestroyer:
				bossKey = "destroyer";
				experience = 800;
				return true;
			case NPCID.Retinazer:
			case NPCID.Spazmatism:
				bossKey = "twins";
				experience = 800;
				return true;
			case NPCID.SkeletronPrime:
				bossKey = "skeletronPrime";
				experience = 800;
				return true;
			case NPCID.Plantera:
				bossKey = "plantera";
				experience = 1200;
				bondLevelTarget = 4;
				return true;
			case NPCID.Golem:
				bossKey = "golem";
				experience = 1400;
				return true;
			case NPCID.MoonLordCore:
				bossKey = "moonLord";
				experience = 2500;
				bondLevelTarget = 5;
				return true;
			default:
				return false;
		}
	}

	private static int GetBaseExperience(NPC npc)
	{
		if (npc.boss) {
			return Main.hardMode ? 900 : 350;
		}

		int value = npc.lifeMax / 20 + npc.defense / 2 + npc.damage / 8;
		if (Main.hardMode) {
			value += 35;
		}

		return Math.Clamp(value, 2, 900);
	}

	private static float GetStageMultiplier(NPC npc, int ringSyncLevel)
	{
		int npcStage = GetNpcStage(npc);
		int hoopaStage = Math.Clamp((ringSyncLevel - 1) / 10, 0, 9);
		int stageDifference = npcStage - hoopaStage;

		if (stageDifference >= 2) {
			return 1.5f;
		}

		return stageDifference switch {
			1 => 1.2f,
			0 => 1f,
			-1 => 0.4f,
			-2 => 0.1f,
			_ => 0f
		};
	}

	private static int GetNpcStage(NPC npc)
	{
		if (npc.boss) {
			return Main.hardMode ? 5 : 2;
		}

		if (NPC.downedMoonlord) {
			return 8;
		}

		if (NPC.downedGolemBoss) {
			return 7;
		}

		if (NPC.downedPlantBoss) {
			return 6;
		}

		if (NPC.downedMechBossAny) {
			return 5;
		}

		if (Main.hardMode) {
			return 4;
		}

		if (NPC.downedBoss3) {
			return 3;
		}

		if (NPC.downedBoss2) {
			return 2;
		}

		if (NPC.downedBoss1) {
			return 1;
		}

		return 0;
	}
}
