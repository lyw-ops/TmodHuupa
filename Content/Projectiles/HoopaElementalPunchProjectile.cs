using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Buffs;

namespace TmodHuupa.Content.Projectiles;

public class HoopaElementalPunchProjectile : ModProjectile
{
	public const int FirePunch = 0;
	public const int IcePunch = 1;
	public const int ThunderPunch = 2;

	private const float HitRadius = 46f;

	public override string Texture => "TmodHuupa/Content/Projectiles/HoopaRingBoltProjectile";

	private int PunchType => (int)Projectile.ai[0];

	public override void SetDefaults()
	{
		Projectile.width = (int)(HitRadius * 2f);
		Projectile.height = (int)(HitRadius * 2f);
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.penetrate = 3;
		Projectile.timeLeft = 18;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override bool? CanHitNPC(NPC target)
	{
		return target.CanBeChasedBy(Projectile);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float closestX = MathHelper.Clamp(Projectile.Center.X, targetHitbox.Left, targetHitbox.Right);
		float closestY = MathHelper.Clamp(Projectile.Center.Y, targetHitbox.Top, targetHitbox.Bottom);
		Vector2 closestPoint = new(closestX, closestY);
		return Vector2.DistanceSquared(closestPoint, Projectile.Center) <= HitRadius * HitRadius;
	}

	public override void AI()
	{
		Projectile.localAI[0]++;
		if (Projectile.localAI[0] == 1f) {
			SoundEngine.PlaySound(GetPunchSound(), Projectile.Center);
			SpawnImpactDust(14);
		}

		if (Main.netMode != NetmodeID.Server && Projectile.localAI[0] % 4f == 0f) {
			SpawnImpactDust(3);
		}

		Color lightColor = GetPunchColor();
		Lighting.AddLight(Projectile.Center, lightColor.R / 255f * 0.45f, lightColor.G / 255f * 0.35f, lightColor.B / 255f * 0.35f);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		switch (PunchType) {
			case FirePunch:
				target.AddBuff(BuffID.OnFire, 240);
				break;
			case IcePunch:
				target.AddBuff(BuffID.Frostburn, 180);
				if (!target.boss && Main.rand.NextFloat() < 0.35f) {
					target.AddBuff(ModContent.BuffType<HoopaFrostBindDebuff>(), 75);
				}
				break;
			case ThunderPunch:
				if (Main.rand.NextFloat() < (target.boss ? 0.18f : 0.4f)) {
					target.AddBuff(ModContent.BuffType<HoopaParalysisDebuff>(), target.boss ? 90 : 150);
				}
				break;
		}
	}

	private SoundStyle GetPunchSound()
	{
		return PunchType switch {
			FirePunch => SoundID.Item20 with { Volume = 0.45f, Pitch = 0.15f },
			IcePunch => SoundID.Item30 with { Volume = 0.45f, Pitch = 0.25f },
			ThunderPunch => SoundID.Item93 with { Volume = 0.42f, Pitch = 0.25f },
			_ => SoundID.Item1
		};
	}

	private Color GetPunchColor()
	{
		return PunchType switch {
			FirePunch => new Color(255, 95, 45),
			IcePunch => new Color(125, 225, 255),
			ThunderPunch => new Color(255, 230, 65),
			_ => Color.White
		};
	}

	private int GetDustType()
	{
		return PunchType switch {
			FirePunch => DustID.Torch,
			IcePunch => DustID.Snow,
			ThunderPunch => DustID.GoldFlame,
			_ => DustID.GoldFlame
		};
	}

	private void SpawnImpactDust(int count)
	{
		if (Main.netMode == NetmodeID.Server) {
			return;
		}

		Color color = GetPunchColor();
		int dustType = GetDustType();
		for (int i = 0; i < count; i++) {
			float speed = Main.rand.NextFloat(0.35f, 1.25f);
			Vector2 velocity = Main.rand.NextVector2CircularEdge(speed, speed);
			Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
			Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 170, color, Main.rand.NextFloat(0.48f, 0.75f));
			dust.noGravity = true;
			dust.noLight = true;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
