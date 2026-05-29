using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TmodHuupa.Content.Projectiles;

namespace TmodHuupa.Content.Buffs;

public class HoopaMinionBuff : ModBuff
{
	public override string Texture => $"Terraria/Images/Buff_{BuffID.Summoning}";

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
