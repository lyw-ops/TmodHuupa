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
	private const int RingPickupUnlockLevel = 10;
	private const float RingPickupRange = 360f;
	private const float RingPickupSpeed = 9f;
	private const int RiftPullUnlockLevel = 30;
	private const int RiftPullCooldownTicks = 240;
	private const int ElementalPunchUnlockLevel = 30;
	private const int ElementalPunchCooldownTicks = 270;
	private const int ElementalPunchTotalTicks = 32;
	private const int ElementalPunchImpactTicks = 13;
	private const int RingShieldUnlockLevel = 40;
	private const int RingShieldCooldownTicks = 2700;
	private const int RingShieldDurationTicks = 480;

	private int elementalPunchCooldown;
	private int elementalPunchTimer;
	private int elementalPunchTargetIndex = -1;
	private int elementalPunchType;
	private bool elementalPunchHasHit;

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

		if (elementalPunchCooldown > 0) {
			elementalPunchCooldown--;
		}

		if (UpdateElementalPunch(owner)) {
			Animate();
			Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.X * 0.018f, 0.12f);
			Lighting.AddLight(Projectile.Center, 0.55f, 0.35f, 0.12f);
			return;
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
		TryUseRingPickupSkill(owner);
		bool startedElementalPunch = TryStartElementalPunchSkill(owner);
		if (!startedElementalPunch) {
			TryUseRiftPullSkill(owner);
		}
		TryUseRingShieldSkill(owner);
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

	private void TryUseRingPickupSkill(Player owner)
	{
		if (Projectile.owner != Main.myPlayer) {
			return;
		}

		HoopaPlayer hoopaPlayer = owner.GetModPlayer<HoopaPlayer>();
		if (hoopaPlayer.RingSyncLevel < RingPickupUnlockLevel) {
			return;
		}

		float rangeSquared = RingPickupRange * RingPickupRange;
		for (int i = 0; i < Main.maxItems; i++) {
			Item item = Main.item[i];
			if (!CanPullItem(item, owner, rangeSquared)) {
				continue;
			}

			Vector2 toOwner = owner.Center - item.Center;
			Vector2 desiredVelocity = toOwner.SafeNormalize(Vector2.Zero) * RingPickupSpeed;
			item.velocity = Vector2.Lerp(item.velocity, desiredVelocity, 0.18f);
			item.noGrabDelay = 0;
		}
	}

	private bool CanPullItem(Item item, Player owner, float rangeSquared)
	{
		if (!item.active || item.IsAir || item.beingGrabbed) {
			return false;
		}

		if (item.playerIndexTheItemIsReservedFor != 400 && item.playerIndexTheItemIsReservedFor != owner.whoAmI) {
			return false;
		}

		return Vector2.DistanceSquared(item.Center, Projectile.Center) <= rangeSquared;
	}

	private void TryUseRiftPullSkill(Player owner)
	{
		if (Projectile.owner != Main.myPlayer) {
			return;
		}

		if (Projectile.ai[1] > 0f) {
			Projectile.ai[1]--;
			return;
		}

		HoopaPlayer hoopaPlayer = owner.GetModPlayer<HoopaPlayer>();
		if (hoopaPlayer.RingSyncLevel < RiftPullUnlockLevel) {
			return;
		}

		NPC target = FindTarget();
		if (target == null) {
			return;
		}

		int damage = 5 + hoopaPlayer.RingSyncLevel / 5;
		Projectile.NewProjectile(
			Projectile.GetSource_FromAI(),
			target.Center,
			Vector2.Zero,
			ModContent.ProjectileType<HoopaRiftPullProjectile>(),
			damage,
			0.3f,
			owner.whoAmI);

		Projectile.ai[1] = RiftPullCooldownTicks;
		Projectile.netUpdate = true;
	}

	private bool TryStartElementalPunchSkill(Player owner)
	{
		if (Projectile.owner != Main.myPlayer || elementalPunchCooldown > 0 || elementalPunchTimer > 0) {
			return false;
		}

		HoopaPlayer hoopaPlayer = owner.GetModPlayer<HoopaPlayer>();
		if (hoopaPlayer.RingSyncLevel < ElementalPunchUnlockLevel) {
			return false;
		}

		NPC target = FindTarget();
		if (target == null) {
			return false;
		}

		elementalPunchTargetIndex = target.whoAmI;
		elementalPunchType = Main.rand.Next(3);
		elementalPunchTimer = 1;
		elementalPunchHasHit = false;
		elementalPunchCooldown = ElementalPunchCooldownTicks;
		SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.3f, Pitch = 0.55f }, Projectile.Center);
		return true;
	}

	private bool UpdateElementalPunch(Player owner)
	{
		if (elementalPunchTimer <= 0) {
			return false;
		}

		if (elementalPunchTargetIndex < 0 || elementalPunchTargetIndex >= Main.maxNPCs) {
			EndElementalPunch();
			return false;
		}

		NPC target = Main.npc[elementalPunchTargetIndex];
		if (!target.CanBeChasedBy(Projectile) || !owner.active || owner.dead) {
			EndElementalPunch();
			return false;
		}

		elementalPunchTimer++;
		Vector2 approachDirection = (Projectile.Center - target.Center).SafeNormalize(new Vector2(-owner.direction, 0f));
		Vector2 punchPosition = target.Center + approachDirection * 42f;
		Vector2 toPunchPosition = punchPosition - Projectile.Center;
		Vector2 desiredVelocity = toPunchPosition.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(toPunchPosition.Length() / 3f, 10f, 24f);
		Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.34f);
		Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

		if (Main.netMode != NetmodeID.Server && elementalPunchTimer % 3 == 0) {
			SpawnElementalPunchTrail();
		}

		if (!elementalPunchHasHit && (elementalPunchTimer >= ElementalPunchImpactTicks || Vector2.DistanceSquared(Projectile.Center, punchPosition) <= 28f * 28f)) {
			PerformElementalPunch(owner, target);
		}

		if (elementalPunchTimer >= ElementalPunchTotalTicks) {
			EndElementalPunch();
		}

		return true;
	}

	private void PerformElementalPunch(Player owner, NPC target)
	{
		HoopaPlayer hoopaPlayer = owner.GetModPlayer<HoopaPlayer>();
		int damage = 14 + hoopaPlayer.RingSyncLevel / 3;
		Projectile.NewProjectile(
			Projectile.GetSource_FromAI(),
			target.Center,
			Vector2.Zero,
			ModContent.ProjectileType<HoopaElementalPunchProjectile>(),
			damage,
			2f,
			owner.whoAmI,
			elementalPunchType);

		Vector2 recoil = (Projectile.Center - target.Center).SafeNormalize(Vector2.UnitY);
		Projectile.velocity = recoil * 8f;
		elementalPunchHasHit = true;
	}

	private void EndElementalPunch()
	{
		elementalPunchTimer = 0;
		elementalPunchTargetIndex = -1;
		elementalPunchHasHit = false;
	}

	private void SpawnElementalPunchTrail()
	{
		Color color = elementalPunchType switch {
			HoopaElementalPunchProjectile.FirePunch => new Color(255, 95, 45),
			HoopaElementalPunchProjectile.IcePunch => new Color(125, 225, 255),
			HoopaElementalPunchProjectile.ThunderPunch => new Color(255, 230, 65),
			_ => new Color(255, 215, 90)
		};
		int dustType = elementalPunchType switch {
			HoopaElementalPunchProjectile.FirePunch => DustID.Torch,
			HoopaElementalPunchProjectile.IcePunch => DustID.Snow,
			HoopaElementalPunchProjectile.ThunderPunch => DustID.GoldFlame,
			_ => DustID.GoldFlame
		};

		Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), dustType, -Projectile.velocity * 0.04f, 170, color, 0.55f);
		dust.noGravity = true;
		dust.noLight = true;
	}

	private void TryUseRingShieldSkill(Player owner)
	{
		if (Projectile.owner != Main.myPlayer) {
			return;
		}

		if (Projectile.localAI[0] > 0f) {
			Projectile.localAI[0]--;
			return;
		}

		HoopaPlayer hoopaPlayer = owner.GetModPlayer<HoopaPlayer>();
		if (hoopaPlayer.RingSyncLevel < RingShieldUnlockLevel || owner.HasBuff(ModContent.BuffType<HoopaRingShieldBuff>())) {
			return;
		}

		if (FindTarget() == null) {
			return;
		}

		owner.AddBuff(ModContent.BuffType<HoopaRingShieldBuff>(), RingShieldDurationTicks);
		SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.4f, Pitch = 0.35f }, owner.Center);
		SpawnRingShieldDust(owner);
		Projectile.localAI[0] = RingShieldCooldownTicks;
	}

	private void SpawnRingShieldDust(Player owner)
	{
		if (Main.netMode == NetmodeID.Server) {
			return;
		}

		for (int i = 0; i < 24; i++) {
			Vector2 direction = (MathHelper.TwoPi * i / 24f).ToRotationVector2();
			Vector2 position = owner.Center + direction * 42f;
			Vector2 velocity = direction.RotatedBy(MathHelper.PiOver2) * 0.45f;
			bool goldDust = i % 2 == 0;
			int dustType = goldDust ? DustID.GoldFlame : DustID.Snow;
			Color color = goldDust ? new Color(255, 215, 90) : new Color(145, 225, 255);
			Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 170, color, 0.58f);
			dust.noGravity = true;
			dust.noLight = true;
		}
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
