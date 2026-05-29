# Hoopa Across Terraria

`TmodHuupa` is an early tModLoader prototype for a Hoopa-themed Terraria adventure.

The story premise is that Hoopa crosses space-time into Terraria, discovers the strange and unknowable creatures of this world, and decides to help the player survive, grow, and eventually face a larger Hoopa threat.

## Current Features

- `Hoopa Summon Staff`: craftable summon weapon.
- Swing once to summon one animated Hoopa minion.
- Hoopa follows the player and stays active while the buff is maintained.
- Hoopa automatically attacks nearby enemies with a small four-frame ring bolt projectile.
- Hoopa gains `Ring Sync` experience while summoned.
- Ring Sync starts at `Lv.1` and initially caps at `Lv.10`.
- Ring Core breakthrough items raise the Ring Sync cap every 10 levels, up to `Lv.100`.
- The summon staff tooltip shows current Ring Sync level and experience.
- The summon staff tooltip also shows Hoopa's current Bond Level.
- Summoning Hoopa prints the current Ring Sync progress in chat.
- Hoopa projectile kills are credited back to the owner for experience gain.
- Key boss kills witnessed by summoned Hoopa grant one-time Ring Sync rewards and can raise Bond Level.
- `Hoopa Debug Compass` is available as a temporary development helper.
- At Ring Sync `Lv.10`, Hoopa unlocks `Ring Pickup`, pulling nearby dropped items toward the player.
- At Ring Sync `Lv.20`, Hoopa unlocks `Spatial Dodge`: while summoned, a full-health player automatically dodges one hit, then the skill enters a `90s` cooldown.

## Ring Sync Prototype

The first growth system is implemented as a small playable MVP:

1. Summon Hoopa with the staff.
2. Fight enemies while Hoopa is active.
3. Enemy kills grant Ring Sync experience.
4. Hoopa levels up automatically when enough experience is gained.
5. At each 10-level cap, leveling stops and the player is told a Ring Core breakthrough is required.
6. Craft and use the matching Ring Core to raise the cap by 10 levels.

Pokemon summon pools, story-gated unlocks, and later-stage skills are planned but not implemented yet.

## Boss Witness Rewards

Hoopa now has a lightweight Bond Level prototype:

- Bond starts at `1` and currently caps at `5`.
- A boss reward is only recorded when Hoopa is summoned and witnesses the kill.
- Each tracked boss can grant its one-time reward once per character.
- Some milestone bosses raise Bond Level:
  - Eye of Cthulhu: Bond `2`
  - Wall of Flesh: Bond `3`
  - Plantera: Bond `4`
  - Moon Lord: Bond `5`

Tracked bosses currently include King Slime, Eye of Cthulhu, Eater of Worlds or Brain of Cthulhu, Queen Bee, Skeletron, Wall of Flesh, the mechanical bosses, Plantera, Golem, and Moon Lord.

## Ring Cores

The breakthrough chain currently supports the full `Lv.10 -> Lv.100` cap progression:

| Core | Cap Change | Station | Main Materials |
|---|---:|---|---|
| `Rough Ring Core` | `10 -> 20` | Anvil | Iron/Lead Bar, Gel, Fallen Star |
| `Gleaming Ring Core` | `20 -> 30` | Anvil | Silver/Tungsten Bar, Gold/Platinum Bar, Amethyst, Fallen Star |
| `Altered Ring Core` | `30 -> 40` | Anvil | Demonite/Crimtane Bar, Shadow Scale/Tissue Sample, Fallen Star |
| `Molten Ring Core` | `40 -> 50` | Anvil | Hellstone Bar, Obsidian, Fireblossom |
| `Hardened Ring Core` | `50 -> 60` | Mythril Anvil | Cobalt/Palladium Bar, Soul of Light, Soul of Night |
| `Arcane Ring Core` | `60 -> 70` | Mythril Anvil | Mythril/Orichalcum Bar, Souls of Sight/Might/Fright |
| `Titan Ring Core` | `70 -> 80` | Mythril Anvil | Adamantite/Titanium Bar, Hallowed Bar |
| `Verdant Ring Core` | `80 -> 90` | Mythril Anvil | Chlorophyte Bar, Life Fruit, Lihzahrd Brick |
| `Astral Ring Core` | `90 -> 100` | Ancient Manipulator | Lunar Fragments, Luminite Bar |

## Testing In Game

To test the current prototype:

1. Enable `TmodHuupa` in tModLoader.
2. Enter a world with a test character.
3. Craft or obtain the `Hoopa Summon Staff`.
4. Summon Hoopa.
5. Stand near simple enemies such as slimes.
6. Confirm Hoopa fires the small ring bolt and Ring Sync experience appears.
7. Hover the staff to check current level and experience.

For faster development testing, craft the `Hoopa Debug Compass` from `1 Dirt Block`:

- Left click: grant Hoopa `500` Ring Sync experience.
- Right click: print current Ring Sync level, experience, cap, and Bond Level.

To test `Ring Pickup`, use the debug compass to reach Ring Sync `Lv.10`, summon Hoopa, then drop items near Hoopa and confirm they drift toward the player.

To test `Spatial Dodge`, use the debug compass and the `Rough Ring Core` to reach Ring Sync `Lv.20`, summon Hoopa, heal to full health, then take one dodgeable hit. The staff tooltip or debug compass will show the remaining cooldown.

## Content Overview

- `Content/Items/Weapons/HoopaSummonStaff.cs`
  - Summons Hoopa.
  - Displays Ring Sync progress.
- `Content/Projectiles/HoopaMinionProjectile.cs`
  - Animated Hoopa minion AI.
  - Follow behavior, basic attack loop, and Ring Pickup.
- `Content/Projectiles/HoopaRingBoltProjectile.cs`
  - Small low-level projectile skill.
- `Content/Players/HoopaPlayer.cs`
  - Saves Ring Sync level, experience, level cap, Bond Level, and witnessed boss rewards.
  - Handles Spatial Dodge unlock and cooldown state.
- `Content/Items/Consumables/`
  - Ring Core breakthrough items.
- `Content/Items/Tools/HoopaDebugCompass.cs`
  - Temporary development helper for growth-system testing.
- `Content/Globals/HoopaExperienceGlobalNPC.cs`
  - Grants experience when valid enemies die.
  - Tracks Hoopa projectile ownership for correct experience credit.
- `Localization/`
  - English and Simplified Chinese text.
- `docs/`
  - Design documents and planning notes.

## Design Docs

- [胡帕经验机制设计](docs/胡帕经验机制设计.md)

## Roadmap

- Expand Bond Level into actual story gates and unlocks.
- Tune early Ring Sync experience gain.
- Tune the first unlockable Hoopa support skill.
- Start the first Pokemon summon pool.
- Design the first `Ring Rift` event prototype.

## Build Notes

This repository imports the local Steam tModLoader `tMLMod.targets` on this machine.

For normal tModLoader development, place or symlink this folder under your tModLoader `ModSources` directory and build it from tModLoader's `Develop Mods` menu.

On this machine, the command-line build used during development is:

```bash
DYLD_LIBRARY_PATH="/Users/lyuyuwei/Library/Application Support/Steam/steamapps/common/tModLoader/Libraries/Native/OSX" \
"/Users/lyuyuwei/Library/Application Support/Steam/steamapps/common/tModLoader/dotnet/dotnet" \
"/Users/lyuyuwei/Library/Application Support/Steam/steamapps/common/tModLoader/tModLoader.dll" \
-server -build "/Users/lyuyuwei/Documents/TmodHuupa"
```
