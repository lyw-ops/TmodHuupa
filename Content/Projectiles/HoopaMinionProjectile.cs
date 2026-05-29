using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Buffs;

namespace TmodHuupa.Content.Projectiles;

public class HoopaMinionProjectile : ModProjectile
{
	private const float IdleDistanceFromPlayer = 68f;
	private const float IdleHeight = 62f;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 1;
		Main.projPet[Projectile.type] = true;
		ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
		ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.minion = true;
		Projectile.minionSlots = 1f;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 18000;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.scale = 0.62f;
	}

	public override bool? CanDamage()
	{
		return false;
	}

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];

		if (!owner.active || owner.dead) {
			owner.ClearBuff(ModContent.BuffType<HoopaMinionBuff>());
			return;
		}

		if (owner.HasBuff(ModContent.BuffType<HoopaMinionBuff>())) {
			Projectile.timeLeft = 2;
		}

		Vector2 idlePosition = owner.Center + new Vector2(-owner.direction * IdleDistanceFromPlayer, -IdleHeight);
		idlePosition.Y += MathF.Sin(Main.GlobalTimeWrappedHourly * 3.5f + Projectile.identity) * 8f;

		float distanceToIdle = Vector2.Distance(Projectile.Center, idlePosition);
		if (distanceToIdle > 1600f) {
			Projectile.Center = owner.Center;
			Projectile.netUpdate = true;
		}

		Vector2 toIdle = idlePosition - Projectile.Center;
		if (toIdle.LengthSquared() < 18f * 18f) {
			Projectile.velocity *= 0.86f;
		}
		else {
			Vector2 desiredVelocity = toIdle.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(distanceToIdle / 18f, 4f, 14f);
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.12f);
		}

		Projectile.spriteDirection = Projectile.Center.X < owner.Center.X ? 1 : -1;
		Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.X * 0.015f, 0.08f);
		Lighting.AddLight(Projectile.Center, 0.45f, 0.25f, 0.75f);
	}
}
