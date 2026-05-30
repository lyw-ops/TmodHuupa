using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TmodHuupa.Content.Buffs;

public class HoopaRingShieldBuff : ModBuff
{
	private const float DamageReduction = 0.12f;

	public override string Texture => "TmodHuupa/Content/Buffs/HoopaMinionBuff";

	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.endurance += DamageReduction;

		if (Main.netMode == NetmodeID.Server || !Main.rand.NextBool(5)) {
			return;
		}

		float angle = Main.rand.NextFloat(MathHelper.TwoPi);
		Vector2 direction = angle.ToRotationVector2();
		Vector2 position = player.Center + direction * Main.rand.NextFloat(32f, 48f);
		Vector2 velocity = direction.RotatedBy(MathHelper.PiOver2) * 0.22f;
		bool goldDust = Main.rand.NextBool();
		int dustType = goldDust ? DustID.GoldFlame : DustID.Snow;
		Color color = goldDust ? new Color(255, 215, 90) : new Color(145, 225, 255);
		Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 180, color, 0.5f);
		dust.noGravity = true;
		dust.noLight = true;
	}
}
