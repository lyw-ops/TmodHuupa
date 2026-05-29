using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TmodHuupa.Content.Projectiles;

public class HoopaRingBoltProjectile : ModProjectile
{
	public override string Texture => "Terraria/Images/Projectile_1";

	public override void SetDefaults()
	{
		Projectile.width = 12;
		Projectile.height = 12;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 90;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
	}

	public override void AI()
	{
		Projectile.rotation += 0.08f * Projectile.direction;
		Lighting.AddLight(Projectile.Center, 0.22f, 0.16f, 0.02f);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D pixel = TextureAssets.MagicPixel.Value;
		Vector2 center = Projectile.Center - Main.screenPosition;
		Vector2 origin = new(0.5f, 0.5f);
		Color ringColor = new Color(245, 205, 65, 210);

		DrawRing(pixel, center, origin, 5.5f, 1.4f, ringColor);
		return false;
	}

	private void DrawRing(Texture2D pixel, Vector2 center, Vector2 origin, float radius, float thickness, Color color)
	{
		const int segments = 12;
		for (int i = 0; i < segments; i++) {
			float angle = MathHelper.TwoPi * i / segments + Projectile.rotation;
			Vector2 radial = new(MathF.Cos(angle), MathF.Sin(angle));
			Vector2 position = center + radial * radius;
			Main.EntitySpriteDraw(pixel, position, null, color, angle + MathHelper.PiOver2, origin, new Vector2(radius * 0.18f, thickness), SpriteEffects.None);
		}
	}
}
