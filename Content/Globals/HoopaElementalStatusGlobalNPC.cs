using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Buffs;

namespace TmodHuupa.Content.Globals;

public class HoopaElementalStatusGlobalNPC : GlobalNPC
{
	public override void AI(NPC npc)
	{
		if (npc.HasBuff(ModContent.BuffType<HoopaFrostBindDebuff>())) {
			ApplyFrostBind(npc);
		}

		if (npc.HasBuff(ModContent.BuffType<HoopaParalysisDebuff>())) {
			ApplyParalysis(npc);
		}
	}

	private static void ApplyFrostBind(NPC npc)
	{
		npc.velocity *= npc.boss ? 0.92f : 0.58f;

		if (Main.netMode == NetmodeID.Server || !Main.rand.NextBool(8)) {
			return;
		}

		Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width * 0.45f, npc.height * 0.45f), DustID.Snow, -npc.velocity * 0.03f, 180, new Color(145, 225, 255), 0.5f);
		dust.noGravity = true;
		dust.noLight = true;
	}

	private static void ApplyParalysis(NPC npc)
	{
		npc.velocity *= npc.boss ? 0.95f : 0.78f;
		if (!npc.boss && Main.GameUpdateCount % 24 < 5) {
			npc.velocity *= 0.25f;
		}

		if (Main.netMode == NetmodeID.Server || !Main.rand.NextBool(8)) {
			return;
		}

		Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width * 0.35f, npc.height * 0.35f), DustID.GoldFlame, Vector2.Zero, 190, new Color(255, 235, 90), 0.45f);
		dust.noGravity = true;
		dust.noLight = true;
	}
}
