# TmodHuupa 开发记录

这个文件用于记录当前原型阶段的图片复用情况，以及后续需要补齐或重新设计的资源问题。

## 图片复用记录

| 使用处 | 当前复用资源 | 原因 | 后续处理 |
|---|---|---|---|
| `HoopaRingShieldBuff` | `Content/Buffs/HoopaMinionBuff.png` | 临时避免缺少 `HoopaRingShieldBuff.png` 导致模组加载失败 | 需要制作独立的圆环护盾 buff 图标 |
| `HoopaFrostBindDebuff` | `Content/Buffs/HoopaMinionBuff.png` | 临时避免新增 debuff 缺图 | 需要制作冰结束缚图标，建议蓝白冰晶或雪花 |
| `HoopaParalysisDebuff` | `Content/Buffs/HoopaMinionBuff.png` | 临时避免新增 debuff 缺图 | 需要制作麻痹图标，建议黄色电流或闪电 |
| `HoopaRiftPullProjectile` | `Content/Projectiles/HoopaRingBoltProjectile.png` | 先复用小圆环素材做浅蓝牵引圈视觉 | 后续可制作独立牵引圆环序列帧 |
| `HoopaElementalPunchProjectile` | `Content/Projectiles/HoopaRingBoltProjectile.png` | 临时复用小圆环帧作为拳击中心闪光 | 后续建议制作火焰拳、冰冻拳、雷电拳独立特效图 |

## 已有独立图片

| 资源 | 用途 | 状态 |
|---|---|---|
| `Content/Buffs/HoopaMinionBuff.png` | 胡帕召唤 buff 图标 | 可用 |
| `Content/Projectiles/HoopaMinionProjectile.png` | 胡帕本体 4 帧动画 | 可用，后续可继续优化动作 |
| `Content/Projectiles/HoopaRingBoltProjectile.png` | 小圆环弹幕 4 帧动画 | 可用，但被多个原型特效临时复用 |
| `Content/Items/Weapons/HoopaSummonStaff.png` | 胡帕召唤杖图标 | 可用 |
| `Content/Items/Tools/HoopaDebugCompass.png` | 调试罗盘图标 | 可用 |
| `Content/Items/Consumables/*.png` | 圆环核心道具图标 | 可用，后续可按阶段重画得更有辨识度 |

## 待处理问题

1. **Buff 图标缺失风险**
   - 每新增一个 `ModBuff`，tModLoader 默认会查找同名 PNG。
   - 如果暂时没有独立图片，需要显式写 `Texture` 复用现有资源。
   - 后续应补齐 `HoopaRingShieldBuff.png`、`HoopaFrostBindDebuff.png`、`HoopaParalysisDebuff.png`。

2. **属性拳特效仍是临时方案**
   - 目前不再使用 `MagicPixel` 画长线，避免出现全屏散射光束。
   - 现在只复用小圆环贴图做近距离闪光，视觉还不够像真正的拳击特效。
   - 后续建议制作三套独立 sprite sheet：
     - 火焰拳：红橙火焰爆点和火星。
     - 冰冻拳：浅蓝冰刺、冰晶碎片。
     - 雷电拳：黄白电光、小蓝色电弧。

3. **异界牵引圆环视觉还需要定稿**
   - 当前版本保留浅蓝透明牵引圈。
   - “胡帕丢出手环再展开”的方案暂时撤回，原因是实机视觉不够理想。
   - 后续如果重做，建议先做小尺寸 sprite sheet，而不是用代码画连续大圆环。

4. **技能强度还没有系统调平**
   - `Lv.30` 同时拥有异界牵引和属性拳，需要继续观察是否过强。
   - `Lv.40` 圆环护盾目前是短时间减伤，需要实机确认触发频率和存在感。
   - 属性拳的燃烧、冰结束缚、麻痹持续时间和触发概率都还是原型值。

5. **构建测试注意事项**
   - 如果 tModLoader 正在运行并加载 `TmodHuupa.tmod`，命令行构建可能在打包阶段失败。
   - 重新构建前最好关闭 tModLoader，或在游戏里禁用本 mod 后退出。

## 近期建议

- 先确认当前 `Lv.30` 属性拳不再出现全屏散射光束。
- 如果视觉稳定，再提交并推送当前 `Lv.30/Lv.40` 技能批次。
- 下一轮优先补独立 buff 图标，降低后续新增 buff 时的资源风险。
