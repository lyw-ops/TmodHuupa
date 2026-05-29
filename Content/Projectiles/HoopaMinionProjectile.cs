using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Buffs;
using TmodHuupa.Content.Players;

namespace TmodHuupa.Content.Projectiles;

public class HoopaMinionProjectile : ModProjectile
{
	private const float IdleDistanceFromPlayer = 68f;
	private const float IdleHeight = 62f;
	private const int AnimationFrameTicks = 8;
	private const float AttackRange = 520f;
	private const float RingBoltSpeed = 8f;
	private const int AttackCooldownTicks = 72;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 4;
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
		Animate();
		TryUseSmallRingSkill(owner);
		Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.X * 0.015f, 0.08f);
		Lighting.AddLight(Projectile.Center, 0.45f, 0.25f, 0.75f);
	}

	private void Animate()
	{
		Projectile.frameCounter++;
		if (Projectile.frameCounter < AnimationFrameTicks) {
			return;
		}

		Projectile.frameCounter = 0;
		Projectile.frame++;
		if (Projectile.frame >= Main.projFrames[Projectile.type]) {
			Projectile.frame = 0;
		}
	}

	private void TryUseSmallRingSkill(Player owner)
	{
		if (Projectile.owner != Main.myPlayer) {
			return;
		}

		if (Projectile.ai[0] > 0f) {
			Projectile.ai[0]--;
			return;
		}

		NPC target = FindTarget();
		if (target == null) {
			return;
		}

		Vector2 direction = target.Center - Projectile.Center;
		if (direction.LengthSquared() <= 0f) {
			return;
		}

		HoopaPlayer hoopaPlayer = owner.GetModPlayer<HoopaPlayer>();
		int damage = 6 + hoopaPlayer.RingSyncLevel / 2;
		Vector2 velocity = direction.SafeNormalize(Vector2.UnitY) * RingBoltSpeed;
		Projectile.NewProjectile(
			Projectile.GetSource_FromAI(),
			Projectile.Center,
			velocity,
			ModContent.ProjectileType<HoopaRingBoltProjectile>(),
			damage,
			1.2f,
			owner.whoAmI);

		SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.35f, Pitch = 0.25f }, Projectile.Center);
		Projectile.ai[0] = AttackCooldownTicks;
		Projectile.netUpdate = true;
	}

	private NPC FindTarget()
	{
		NPC closestTarget = null;
		float closestDistanceSquared = AttackRange * AttackRange;

		for (int i = 0; i < Main.maxNPCs; i++) {
			NPC npc = Main.npc[i];
			if (!npc.CanBeChasedBy(Projectile)) {
				continue;
			}

			float distanceSquared = Vector2.DistanceSquared(Projectile.Center, npc.Center);
			if (distanceSquared >= closestDistanceSquared) {
				continue;
			}

			if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height)) {
				continue;
			}

			closestTarget = npc;
			closestDistanceSquared = distanceSquared;
		}

		return closestTarget;
	}
}
