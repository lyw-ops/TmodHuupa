using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Buffs;

namespace TmodHuupa.Content.Projectiles;

public class HoopaMinionProjectile : ModProjectile
{
	private const float IdleDistanceFromPlayer = 68f;
	private const float IdleHeight = 62f;

	public override string Texture => "Terraria/Images/Projectile_1";

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
		Projectile.width = 58;
		Projectile.height = 58;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.minion = true;
		Projectile.minionSlots = 1f;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 18000;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Summon;
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

	public override bool PreDraw(ref Color lightColor)
	{
		HoopaDraw.Draw(Projectile.Center, Projectile.rotation, Projectile.spriteDirection);
		return false;
	}
}

internal static class HoopaDraw
{
	public static void Draw(Vector2 worldCenter, float rotation, int spriteDirection)
	{
		float bob = MathF.Sin(Main.GlobalTimeWrappedHourly * 4f) * 1.5f;
		Vector2 center = worldCenter + new Vector2(0f, bob);
		SpriteEffects effects = spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		DrawRing(center + Flip(new Vector2(-25f, -4f), spriteDirection), 15f, 3.5f, new Color(248, 215, 36, 230));
		DrawRing(center + Flip(new Vector2(25f, -4f), spriteDirection), 15f, 3.5f, new Color(248, 215, 36, 230));
		DrawRing(center + new Vector2(0f, 16f), 27f, 4f, new Color(248, 215, 36, 235));

		DrawFilledEllipse(center + new Vector2(0f, 4f), 21f, 24f, new Color(224, 65, 136), rotation, effects);
		DrawFilledEllipse(center + Flip(new Vector2(-25f, 8f), spriteDirection), 12f, 15f, new Color(214, 62, 128), rotation * 0.5f, effects);
		DrawFilledEllipse(center + Flip(new Vector2(25f, 8f), spriteDirection), 12f, 15f, new Color(214, 62, 128), -rotation * 0.5f, effects);

		DrawLine(center + Flip(new Vector2(-22f, -20f), spriteDirection), center + Flip(new Vector2(-34f, -36f), spriteDirection), 5f, new Color(170, 166, 166));
		DrawLine(center + Flip(new Vector2(22f, -20f), spriteDirection), center + Flip(new Vector2(34f, -36f), spriteDirection), 5f, new Color(170, 166, 166));

		DrawFilledEllipse(center + new Vector2(-4f * spriteDirection, -18f), 19f, 15f, new Color(188, 177, 214), rotation * 0.35f, effects);
		DrawFilledEllipse(center + Flip(new Vector2(11f, -42f), spriteDirection), 14f, 14f, new Color(232, 81, 145), rotation, effects);
		DrawFilledEllipse(center + Flip(new Vector2(25f, -36f), spriteDirection), 13f, 9f, new Color(232, 81, 145), rotation * 0.3f, effects);

		DrawFilledEllipse(center + Flip(new Vector2(-11f, -18f), spriteDirection), 4f, 8f, new Color(244, 224, 30), 0f, effects);
		DrawFilledEllipse(center + Flip(new Vector2(7f, -17f), spriteDirection), 4f, 8f, new Color(122, 232, 75), 0f, effects);
		DrawFilledEllipse(center + Flip(new Vector2(-11f, -18f), spriteDirection), 1.7f, 5f, new Color(35, 35, 38), 0f, effects);
		DrawFilledEllipse(center + Flip(new Vector2(7f, -17f), spriteDirection), 1.7f, 5f, new Color(35, 35, 38), 0f, effects);

		DrawRing(center + Flip(new Vector2(-21f, 11f), spriteDirection), 10f, 3f, new Color(248, 215, 36, 230));
		DrawRing(center + Flip(new Vector2(21f, 11f), spriteDirection), 10f, 3f, new Color(248, 215, 36, 230));
	}

	private static Vector2 Flip(Vector2 value, int spriteDirection)
	{
		return new Vector2(value.X * spriteDirection, value.Y);
	}

	private static void DrawFilledEllipse(Vector2 worldCenter, float halfWidth, float halfHeight, Color color, float rotation, SpriteEffects effects)
	{
		Texture2D pixel = TextureAssets.MagicPixel.Value;
		Vector2 origin = new(0.5f, 0.5f);

		for (float y = -halfHeight; y <= halfHeight; y += 2f) {
			float normalizedY = y / halfHeight;
			float width = halfWidth * MathF.Sqrt(MathF.Max(0f, 1f - normalizedY * normalizedY));
			Vector2 local = new Vector2(0f, y).RotatedBy(rotation);
			Main.EntitySpriteDraw(pixel, worldCenter + local - Main.screenPosition, null, color, rotation, origin, new Vector2(width * 2f, 2.5f), effects);
		}
	}

	private static void DrawRing(Vector2 worldCenter, float radius, float thickness, Color color)
	{
		const int segments = 36;
		Texture2D pixel = TextureAssets.MagicPixel.Value;
		Vector2 origin = new(0.5f, 0.5f);

		for (int i = 0; i < segments; i++) {
			float angle = MathHelper.TwoPi * i / segments;
			Vector2 radial = new(MathF.Cos(angle), MathF.Sin(angle));
			Vector2 position = worldCenter + radial * radius - Main.screenPosition;
			Main.EntitySpriteDraw(pixel, position, null, color, angle + MathHelper.PiOver2, origin, new Vector2(radius * 0.22f, thickness), SpriteEffects.None);
		}
	}

	private static void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
	{
		Texture2D pixel = TextureAssets.MagicPixel.Value;
		Vector2 delta = end - start;
		Vector2 center = (start + end) * 0.5f - Main.screenPosition;
		Main.EntitySpriteDraw(pixel, center, null, color, delta.ToRotation(), new Vector2(0.5f, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None);
	}
}
