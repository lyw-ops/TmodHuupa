using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TmodHuupa.Content.Projectiles;

public class HoopaRiftPullProjectile : ModProjectile
{
	private const int LifetimeTicks = 90;
	private const float PullRadius = 150f;
	private const int DustIntervalTicks = 6;

	public override string Texture => "TmodHuupa/Content/Projectiles/HoopaRingBoltProjectile";

	public override void SetDefaults()
	{
		Projectile.width = (int)(PullRadius * 2f);
		Projectile.height = (int)(PullRadius * 2f);
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.penetrate = -1;
		Projectile.timeLeft = LifetimeTicks;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
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
		return Vector2.DistanceSquared(closestPoint, Projectile.Center) <= PullRadius * PullRadius;
	}

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		if (!owner.active || owner.dead) {
			Projectile.Kill();
			return;
		}

		Projectile.localAI[0]++;
		if (Projectile.localAI[0] == 1f) {
			SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.45f, Pitch = -0.15f }, Projectile.Center);
		}

		if (Main.netMode != NetmodeID.MultiplayerClient) {
			PullNearbyNPCs();
		}

		if (Main.netMode != NetmodeID.Server && Projectile.localAI[0] % DustIntervalTicks == 0f) {
			SpawnRingDust();
		}

		Lighting.AddLight(Projectile.Center, 0.12f, 0.36f, 0.55f);
	}

	private void PullNearbyNPCs()
	{
		float radiusSquared = PullRadius * PullRadius;
		for (int i = 0; i < Main.maxNPCs; i++) {
			NPC npc = Main.npc[i];
			if (!npc.CanBeChasedBy(Projectile)) {
				continue;
			}

			Vector2 toCenter = Projectile.Center - npc.Center;
			float distanceSquared = toCenter.LengthSquared();
			if (distanceSquared <= 16f || distanceSquared > radiusSquared) {
				continue;
			}

			float distance = MathF.Sqrt(distanceSquared);
			float falloff = 1f - distance / PullRadius;
			float resistance = npc.boss ? 0.25f : MathHelper.Clamp(npc.knockBackResist, 0.35f, 1f);
			float pullStrength = 0.055f * resistance * (0.4f + falloff);
			npc.velocity += toCenter / distance * pullStrength;
			npc.velocity *= npc.boss ? 0.997f : 0.99f;
		}
	}

	private void SpawnRingDust()
	{
		int dustCount = 10;
		float spin = Projectile.localAI[0] * 0.045f;
		for (int i = 0; i < dustCount; i++) {
			float angle = MathHelper.TwoPi * i / dustCount + spin;
			Vector2 direction = angle.ToRotationVector2();
			Vector2 position = Projectile.Center + direction * PullRadius;
			Vector2 velocity = direction.RotatedBy(MathHelper.PiOver2) * 0.25f - direction * 0.12f;
			Dust dust = Dust.NewDustPerfect(position, DustID.Snow, velocity, 180, new Color(145, 225, 255), 0.55f);
			dust.noGravity = true;
			dust.noLight = true;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<HoopaRingBoltProjectile>()].Value;
		int frameHeight = texture.Height / 4;
		int frame = (int)(Main.GameUpdateCount / 6 % 4);
		Rectangle sourceRectangle = new(0, frameHeight * frame, texture.Width, frameHeight);
		Vector2 origin = new(sourceRectangle.Width, sourceRectangle.Height);
		origin *= 0.5f;
		Vector2 center = Projectile.Center - Main.screenPosition;

		float fadeIn = MathHelper.Clamp(Projectile.localAI[0] / 12f, 0f, 1f);
		float fadeOut = MathHelper.Clamp(Projectile.timeLeft / 18f, 0f, 1f);
		float opacity = fadeIn * fadeOut * 0.48f;
		float spin = Main.GlobalTimeWrappedHourly * 1.8f;

		for (int i = 0; i < 18; i++) {
			float angle = MathHelper.TwoPi * i / 18f + spin;
			Vector2 offset = angle.ToRotationVector2() * PullRadius;
			float scale = 0.58f + 0.08f * MathF.Sin(spin * 2f + i);
			Main.EntitySpriteDraw(
				texture,
				center + offset,
				sourceRectangle,
				new Color(145, 225, 255, 0) * opacity,
				angle + MathHelper.PiOver2,
				origin,
				scale,
				SpriteEffects.None,
				0);
		}

		return false;
	}
}
