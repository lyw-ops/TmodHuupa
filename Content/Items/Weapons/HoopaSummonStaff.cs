using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Buffs;
using TmodHuupa.Content.Projectiles;

namespace TmodHuupa.Content.Items.Weapons;

public class HoopaSummonStaff : ModItem
{
	public override string Texture => "Terraria/Images/Item_495";

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.damage = 1;
		Item.DamageType = DamageClass.Summon;
		Item.mana = 10;
		Item.useTime = 36;
		Item.useAnimation = 36;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.knockBack = 0f;
		Item.value = Item.buyPrice(gold: 4);
		Item.rare = ItemRarityID.Pink;
		Item.UseSound = SoundID.Item44;
		Item.buffType = ModContent.BuffType<HoopaMinionBuff>();
		Item.shoot = ModContent.ProjectileType<HoopaMinionProjectile>();
		Item.shootSpeed = 0f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		player.AddBuff(Item.buffType, 2);

		if (player.whoAmI != Main.myPlayer) {
			return false;
		}

		for (int i = 0; i < Main.maxProjectiles; i++) {
			Projectile projectile = Main.projectile[i];
			if (projectile.active && projectile.owner == player.whoAmI && projectile.type == type) {
				projectile.Kill();
			}
		}

		Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
		return false;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.GoldBar, 8)
			.AddIngredient(ItemID.FallenStar, 10)
			.AddIngredient(ItemID.Amethyst, 5)
			.AddTile(TileID.Anvils)
			.Register();

		CreateRecipe()
			.AddIngredient(ItemID.PlatinumBar, 8)
			.AddIngredient(ItemID.FallenStar, 10)
			.AddIngredient(ItemID.Amethyst, 5)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
