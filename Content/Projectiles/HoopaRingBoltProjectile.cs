using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TmodHuupa.Content.Projectiles;

public class HoopaRingBoltProjectile : ModProjectile
{
	private const int AnimationFrameTicks = 5;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 4;
	}

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
		Projectile.extraUpdates = 1;
		Projectile.scale = 0.75f;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		Animate();
		Lighting.AddLight(Projectile.Center, 0.24f, 0.18f, 0.03f);
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
}
