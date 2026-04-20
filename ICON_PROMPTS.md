# AI Image Generation Prompts for Custom Item Icons

Prompts for DALL-E, Midjourney, Stable Diffusion or similar generators to create Subnautica-style inventory icons for the mod's custom items.

## Base style prompt (prepend to every icon)

> Subnautica inventory icon style, circular cyan-bordered portrait, dark navy blue background with subtle gradient, 3D-rendered item, slight angled perspective view, glossy metallic highlights, no text or labels, transparent background outside the circle, clean game-UI icon, 512x512

## Individual item prompts

### Backpacks

**Small Backpack** (`InferiusBackpackSmall`)
> [base style prompt] + compact black tactical backpack, yellow strap highlights, small size, waterproof diving gear aesthetic

**Medium Backpack** (`InferiusBackpackMedium`)
> [base style prompt] + medium-sized diving backpack, reinforced seams, extra pouches, yellow and orange accents, underwater equipment look

**Large Backpack** (`InferiusBackpackLarge`)
> [base style prompt] + large endgame diving backpack, heavy-duty composite material, multiple straps, glowing cyan energy core visible, futuristic deep-sea gear

### Seamoth Turbo Modules

**Seamoth Turbo MK1** (`InferiusSeamothTurboMK1`)
> [base style prompt] + angular vehicle upgrade module cartridge, turquoise glowing energy core, "MK1" marking etched in metal, Seamoth-style submarine tech

**Seamoth Turbo MK2** (`InferiusSeamothTurboMK2`)
> [base style prompt] + refined vehicle module cartridge, dual blue energy cores, metallic bronze accents, "MK2" engraving, more complex circuitry than MK1

**Seamoth Turbo MK3** (`InferiusSeamothTurboMK3`)
> [base style prompt] + endgame vehicle module, triple plasma cores glowing white-hot, obsidian black housing, precursor-tech highlights, "MK3" glowing inscription

### Merged Tanks

**Merged Ultra Tank T1/T2/T3** (`InferiusMergedTankT1-3`)
> [base style prompt] + two fused Plasteel oxygen tanks welded together, single central valve, visible compression seams, metallic grey with orange trim

**Lightweight Merged Tank T4** (`InferiusMergedTankT4`)
> [base style prompt] + sleek white-and-cyan aerogel oxygen tank, hollow lightweight frame design, subtle glow, precursor alloy aesthetic

### Batteries

**Reinforced Battery** (`InferiusReinforcedBattery`)
> [base style prompt] + reinforced battery cylinder with ruby-red crystal inlay, magnetite plate wrapping, stronger vanilla battery feel

**Reinforced Power Cell** (`InferiusReinforcedPowerCell`)
> [base style prompt] + larger power cell with twin ruby cores, bronze casing, thicker reinforced housing

**Hyper Battery** (`InferiusHyperBattery`)
> [base style prompt] + futuristic battery with glowing violet ion plasma, kyanite blue accents, aerogel transparent shell

**Hyper Power Cell** (`InferiusHyperPowerCell`)
> [base style prompt] + massive endgame power cell, twin violet plasma cores, advanced wiring visible through transparent polymer housing

### Compressor

**Inventory Compressor Chip** (`InferiusCompressor`)
> [base style prompt] + small circuit chip with compression wave pattern on top, silver-and-cyan holographic readout, diving equipment aesthetic

### Teleport

**Teleport Beacon** (`InferiusTeleportBeacon`)
> [base style prompt] + miniature Aurora spaceship model on metal base, glowing cyan energy ring beneath, beacon transmitter styling

**Teleport Efficiency Chip MK1** (`InferiusTeleportEfficiencyMK1`)
> [base style prompt] + small circuit chip with single cyan flow line pattern, silver casing, "MK1" marking

**Teleport Efficiency Chip MK2** (`InferiusTeleportEfficiencyMK2`)
> [base style prompt] + chip with dual cyan flow lines, brass accents, "MK2" marking, refined design

**Teleport Efficiency Chip MK3** (`InferiusTeleportEfficiencyMK3`)
> [base style prompt] + endgame chip with triple plasma flow lines, obsidian black substrate, "MK3" glowing inscription, precursor-tech feel

## How to use generated icons

1. Generate 512×512 PNG with transparent background
2. Resize to 128×128 or 256×256 (Subnautica typical icon size)
3. Save as `<ClassId>.png` (e.g. `InferiusBackpackSmall.png`) in `InferiusQoL/Assets/Icons/`
4. Replace `SpriteManager.Get(TechType.X)` in the corresponding `RegisterX()` method with:
   ```csharp
   info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(dllDir, "Assets", "Icons", "ClassId.png")))
   ```

## Alternative: community resources

If AI generation doesn't yield satisfactory results:
- [Subnautica Modding Discord](https://discord.gg/subnautica) `#modding` channel - community artists often help with free or commissioned icons.
- [Nexus Mods Subnautica](https://www.nexusmods.com/subnautica) - look at how other mods (like `Cyclops Enhancement` or `Sleek Bases`) style their icons.
