# Inferius Quality of Life

A modular Quality-of-Life package for Subnautica. One mod, many improvements - all configurable, all can be toggled individually.

---

## English description (for Nexus Mods Description tab)

### What it does

**Inferius Quality of Life** bundles the most-requested QoL tweaks into a single mod. Every feature is optional and configurable in-game (Options → Mods → Inferius Quality of Life). All item names, tooltips and labels are localized (English + Czech).

### Features

**Inventory**
- **Enlarge player inventory** - add configurable rows/columns. Runtime updates when you move the slider.
- **Bigger lockers** - resize vanilla Locker and Wall Locker to custom dimensions (default 6×8 / 4×5). Works both on newly-built and existing lockers.
- **Inventory Compressor chip** - equip in Chip slot to shrink every non-blacklisted item to 1×1. Persistent across save/load, base storage, vehicles. User-editable blacklist (fish, eggs, batteries, tanks).

**Backpacks** (3 tiers)
- Equip in Chip slot for extra inventory rows. Progressive recipe chain: Small → Medium → Large. Each tier consumes the previous + new materials.

**Seamoth Turbo** (3 tiers + efficiency handling)
- Vehicle module. Hold Sprint while piloting + module installed = speed boost with higher drain.
- Surface falloff: boost smoothly fades as you approach the surface (no more "jumping out of water" glitches).
- Respects vanilla Vehicle Power Upgrade Module discount.
- Each tier has its own speed/drain multiplier in config.

**Merged Oxygen Tanks** (4 tiers)
- Craft in Modification Station. 2× Plasteel Tank → one merged tank with combined capacity × multiplier.
- T1/T2/T3: inherit vanilla Plasteel speed penalty (weight).
- **T4 Lightweight**: same capacity as T3 but no speed penalty.

**Battery Rework**
- **Reinforced Battery / Power Cell** (default 250/500) - mid-game tier, craftable in Fabricator → Resources → Electronics alongside vanilla.
- **Hyper Battery / Power Cell** (default 1500/3000) - endgame tier in Modification Station, consumes Ion Battery/Cell in recipe.

**Teleport Beacon**
- Buildable interior piece in Habitat Builder. Uses the Aurora mini-model (Starship Souvenir mesh).
- Click the beacon to open a menu: rename it, see all other beacons with distance + energy cost, teleport to any of them.
- Energy cost is distance-based (base cost + per-100m factor).
- **3 Efficiency Chips** (MK1/MK2/MK3) - craft them and install directly in the beacon UI. Each tier reduces teleport cost (default 75% / 50% / 25% of base).
- Teleport drops you exactly 2m in front of the target beacon, facing it - consistent landing.

### Compatibility

The mod detects and respects these other mods:
- **RamunesCustomizedStorage** - our locker resize stays off when you have this installed
- **AdvancedInventory** - our (planned) scrollable inventory stays off
- **BagEquipment** - our backpacks stay off
- **SlotExtender** - detected, extra Chip slots help with multi-chip setups

### Configuration

- **Options menu** (Options → Mods) - each feature has its own section with toggles and sliders. Changes are saved to `BepInEx/config/InferiusQoL/config.json` and applied at runtime where possible.
- **Blacklist file** at `BepInEx/plugins/InferiusQoL/Data/CompressorBlacklist.json` - edit which TechTypes the Compressor cannot shrink (fish, eggs, batteries, tanks by default).
- **Console commands** (`~` key): `qol_status` for overview, `qol_log_level` to change verbosity, and more.

### Requirements

- Subnautica (not Below Zero)
- BepInEx 5.x
- Nautilus (the modern replacement for SMLHelper)

### Credits

Built on top of BepInEx, Nautilus and Harmony. Localization contributors welcome - drop a new JSON file in `LanguageFiles/`.

---

## Český popis (pro Nexus Mods Description)

### Co mod dělá

**Inferius Quality of Life** je modulární Quality-of-Life balíček pro Subnauticu v jednom modu. Každá featura je volitelná a konfigurovatelná přímo ve hře (Options → Mods → Inferius Quality of Life). Všechny názvy položek, tooltipy a popisky jsou v angličtině i češtině.

### Featury

**Inventář**
- **Zvětšený inventář hráče** - přidání konfigurovatelných řádků/sloupců. Změna sliderů v Options se projeví ihned.
- **Větší skříně** - mění vanilla Locker a Wall Locker na vlastní rozměry (default 6×8 / 4×5). Funguje na nově postavené i existující skříně.
- **Inventory Compressor chip** - osaď do Chip slotu a všechny položky mimo blacklist se zmenší na 1×1. Perzistentní napříč save/load, skříněmi i vozidly. Uživatelsky editovatelný blacklist (ryby, vajíčka, baterie, lahve).

**Batohy** (3 tiery)
- Osazují se do Chip slotu, každý tier přidá extra řádky. Progresivní recept: Malý → Střední → Velký. Vyšší tier spotřebuje nižší + další materiály.

**Seamoth Turbo** (3 tiery + detekce efektivity)
- Vehicle modul. Sprint + modul při pilotáži = boost rychlosti s vyšší spotřebou.
- Surface falloff: boost plynule klesá jak se blížíš k hladině (žádné skoky nad vodu).
- Respektuje vanilla Vehicle Power Upgrade Module slevu na spotřebě.
- Každý tier má vlastní multiplikátor rychlosti a spotřeby v Options.

**Spojené kyslíkové lahve** (4 tiery)
- Craft ve Vylepšovací stanici. 2× Plasteel Tank → jedna spojená lahev s kombinovanou kapacitou × násobitel.
- T1/T2/T3: dědí vanilla Plasteel penalty na rychlost plavání (váha).
- **T4 Odlehčená**: stejná kapacita jako T3 bez jakéhokoliv penalty.

**Rework baterií**
- **Reinforced Baterie / Power Cell** (default 250/500) - mid-game tier, craft ve Fabricatoru → Resources → Electronics vedle vanilla.
- **Hyper Baterie / Power Cell** (default 1500/3000) - endgame tier ve Vylepšovací stanici, recept spotřebuje Ion Baterii/Cell.

**Teleportační maják**
- Buildable interior piece v Habitat Builderu. Model mini-Aurory (mesh z Starship Souvenir).
- Klikni na maják → menu: pojmenuj ho, ukáže všechny ostatní majáky s vzdáleností a cenou, teleport na libovolný.
- Energetická cena je úměrná vzdálenosti (základ + per-100m faktor).
- **3 Efficiency čipy** (MK1/MK2/MK3) - vyrob je, osaď v UI majáku. Každý tier snižuje cenu (default 75% / 50% / 25% z plné).
- Teleport tě přesně na 2 m před cílový maják, čelem k němu - konzistentní místo dopadu.

### Kompatibilita

Mod detekuje a respektuje tyto další mody:
- **RamunesCustomizedStorage** - naše locker resize se nezapne, pokud ho máš
- **AdvancedInventory** - naše (plánovaná) scrollable inventář se nezapne
- **BagEquipment** - naše batohy se nezapnou
- **SlotExtender** - detekován, extra Chip sloty pomáhají s multi-chip setupem

### Konfigurace

- **Options menu** (Options → Mods) - každá featura má vlastní sekci s toggly a slidery. Změny se uloží do `BepInEx/config/InferiusQoL/config.json` a aplikují runtime kde je to možné.
- **Blacklist soubor** v `BepInEx/plugins/InferiusQoL/Data/CompressorBlacklist.json` - edituj který TechType Compressor nezmenšuje.
- **Konzolové příkazy** (`~` klávesa): `qol_status` pro přehled, `qol_log_level` pro změnu verbosity, další.

### Požadavky

- Subnautica (ne Below Zero)
- BepInEx 5.x
- Nautilus (moderní náhrada SMLHelperu)

### Poděkování

Postaveno na BepInEx, Nautilus a Harmony. Překlady do dalších jazyků vítány - stačí JSON soubor do `LanguageFiles/`.
