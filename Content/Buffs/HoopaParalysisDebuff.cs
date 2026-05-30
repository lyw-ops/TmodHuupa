using Terraria;
using Terraria.ModLoader;

namespace TmodHuupa.Content.Buffs;

public class HoopaParalysisDebuff : ModBuff
{
	public override string Texture => "TmodHuupa/Content/Buffs/HoopaMinionBuff";

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}
}
