using Terraria;
using Terraria.ModLoader;
using TmodHuupa.Content.Projectiles;

namespace TmodHuupa.Content.Buffs;

public class HoopaMinionBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<HoopaMinionProjectile>()] > 0) {
			player.buffTime[buffIndex] = 18000;
			return;
		}

		player.DelBuff(buffIndex);
		buffIndex--;
	}
}
