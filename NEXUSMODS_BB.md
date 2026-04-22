[size=6][b]Inferius Quality of Life[/b][/size]

A modular Quality-of-Life package for Subnautica. One mod, many improvements - all configurable, all can be toggled individually.

[line]

[size=5][b]English description (for Nexus Mods Description tab)[/b][/size]

[size=4][b]What it does[/b][/size]

[b]Inferius Quality of Life[/b] bundles the most-requested QoL tweaks into a single mod. Every feature is optional and configurable in-game (Options -> Mods -> Inferius Quality of Life). All item names, tooltips and labels are localized (English + Czech).

[size=4][b]Features[/b][/size]

[b]Inventory[/b]
[list]
[*][b]Enlarge player inventory[/b] - add configurable rows/columns. Runtime updates when you move the slider.
[*][b]Bigger lockers[/b] - resize vanilla Locker and Wall Locker to custom dimensions (default 6x8 / 4x5). Works both on newly-built and existing lockers.
[*][b]Inventory Compressor chip[/b] - equip in Chip slot to shrink every non-blacklisted item to 1x1. Persistent across save/load, base storage, vehicles. User-editable blacklist (fish, eggs, batteries, tanks).
[/list]

[b]Backpacks[/b] (3 tiers)
[list]
[*]Equip in Chip slot for extra inventory rows. Progressive recipe chain: Small -> Medium -> Large. Each tier consumes the previous + new materials.
[/list]

[b]Seamoth Turbo[/b] (3 tiers + efficiency handling)
[list]
[*]Vehicle module. Hold Sprint while piloting + module installed = speed boost with higher drain.
[*]Surface falloff: boost smoothly fades as you approach the surface (no more "jumping out of water" glitches).
[*]Respects vanilla Vehicle Power Upgrade Module discount.
[*]Each tier has its own speed/drain multiplier in config.
[/list]

[b]Merged Oxygen Tanks[/b] (4 tiers)
[list]
[*]Craft in Modification Station. 2x Plasteel Tank -> one merged tank with combined capacity x multiplier.
[*]T1/T2/T3: inherit vanilla Plasteel speed penalty (weight).
[*][b]T4 Lightweight[/b]: same capacity as T3 but no speed penalty.
[/list]

[b]Battery Rework[/b]
[list]
[*][b]Reinforced Battery / Power Cell[/b] (default 250/500) - mid-game tier, craftable in Fabricator -> Resources -> Electronics alongside vanilla.
[*][b]Hyper Battery / Power Cell[/b] (default 1500/3000) - endgame tier in Modification Station, consumes Ion Battery/Cell in recipe.
[/list]

[b]Teleport Beacon[/b]
[list]
[*]Buildable interior piece in Habitat Builder. Uses the Aurora mini-model (Starship Souvenir mesh).
[*]Click the beacon to open a menu: rename it, see all other beacons with distance + energy cost, teleport to any of them.
[*]Energy cost is distance-based (base cost + per-100m factor).
[*][b]3 Efficiency Chips[/b] (MK1/MK2/MK3) - craft them and install directly in the beacon UI. Each tier reduces teleport cost (default 75% / 50% / 25% of base).
[*]Teleport drops you exactly 2m in front of the target beacon, facing it - consistent landing.
[/list]

[b]Locker Mover[/b] [i](since v0.2.0)[/i]
[list]
[*]Relocate full lockers without manually unloading every item. Point at a full Locker / Wall Locker / Waterproof Locker / Carryall, press the keybind (default [code]G[/code]) - the contents move to an in-memory clipboard and the locker becomes empty (ready to deconstruct).
[*]Point at a new empty locker of the same type and press the key again - contents drop back in.
[*]Fully compatible with the [b]Inventory Compressor[/b]: compressed items stay 1x1 throughout the move.
[*]Single-slot clipboard. Current limitation: contents are lost on save/quit while the clipboard is non-empty (v1 does not persist).
[/list]

[size=4][b]Compatibility[/b][/size]

The mod detects and respects these other mods:
[list]
[*][b]AdvancedInventory[/b] - our (planned) scrollable inventory stays off
[*][b]SlotExtender[/b] - detected, extra Chip slots help with multi-chip setups
[/list]

[size=4][b]Configuration[/b][/size]

[list]
[*][b]Options menu[/b] (Options -> Mods) - each feature has its own section with toggles and sliders. Changes are saved to [code]BepInEx/config/InferiusQoL/config.json[/code] and applied at runtime where possible.
[*][b]Blacklist file[/b] at [code]BepInEx/plugins/InferiusQoL/Data/CompressorBlacklist.json[/code] - edit which TechTypes the Compressor cannot shrink (fish, eggs, batteries, tanks by default).
[*][b]Console commands[/b] ([code]~[/code] key): [code]qol_status[/code] for overview, [code]qol_log_level[/code] to change verbosity, and more.
[/list]

[size=4][b]Requirements[/b][/size]

[list]
[*]Subnautica (not Below Zero)
[*]BepInEx 5.x
[*]Nautilus (the modern replacement for SMLHelper)
[/list]

[size=4][b]Credits[/b][/size]

Built on top of BepInEx, Nautilus and Harmony. Localization contributors welcome - drop a new JSON file in [code]LanguageFiles/[/code].

[line]

[size=5][b]Cesky popis (pro Nexus Mods Description)[/b][/size]

[size=4][b]Co mod dela[/b][/size]

[b]Inferius Quality of Life[/b] je modularni Quality-of-Life balicek pro Subnauticu v jednom modu. Kazda featura je volitelna a konfigurovatelna primo ve hre (Options -> Mods -> Inferius Quality of Life). Vsechny nazvy polozek, tooltipy a popisky jsou v anglictine i cestine.

[size=4][b]Featury[/b][/size]

[b]Inventar[/b]
[list]
[*][b]Zvetseny inventar hrace[/b] - pridani konfigurovatelnych radku/sloupcu. Zmena slideru v Options se projevi ihned.
[*][b]Vetsi skrine[/b] - meni vanilla Locker a Wall Locker na vlastni rozmery (default 6x8 / 4x5). Funguje na nove postavene i existujici skrine.
[*][b]Inventory Compressor chip[/b] - osad do Chip slotu a vsechny polozky mimo blacklist se zmensi na 1x1. Perzistentni napric save/load, skrinemi i vozidly. Uzivatelsky editovatelny blacklist (ryby, vajicka, baterie, lahve).
[/list]

[b]Batohy[/b] (3 tiery)
[list]
[*]Osazuji se do Chip slotu, kazdy tier prida extra radky. Progresivni recept: Maly -> Stredni -> Velky. Vyssi tier spotrebuje nizsi + dalsi materialy.
[/list]

[b]Seamoth Turbo[/b] (3 tiery + detekce efektivity)
[list]
[*]Vehicle modul. Sprint + modul pri pilotazi = boost rychlosti s vyssi spotrebou.
[*]Surface falloff: boost plynule klesa jak se blizis k hladine (zadne skoky nad vodu).
[*]Respektuje vanilla Vehicle Power Upgrade Module slevu na spotrebe.
[*]Kazdy tier ma vlastni multiplikator rychlosti a spotreby v Options.
[/list]

[b]Spojene kyslikove lahve[/b] (4 tiery)
[list]
[*]Craft ve Vylepsovaci stanici. 2x Plasteel Tank -> jedna spojena lahev s kombinovanou kapacitou x nasobitel.
[*]T1/T2/T3: dedi vanilla Plasteel penalty na rychlost plavani (vaha).
[*][b]T4 Odlehcena[/b]: stejna kapacita jako T3 bez jakehokoliv penalty.
[/list]

[b]Rework baterii[/b]
[list]
[*][b]Reinforced Baterie / Power Cell[/b] (default 250/500) - mid-game tier, craft ve Fabricatoru -> Resources -> Electronics vedle vanilla.
[*][b]Hyper Baterie / Power Cell[/b] (default 1500/3000) - endgame tier ve Vylepsovaci stanici, recept spotrebuje Ion Baterii/Cell.
[/list]

[b]Teleportacni majak[/b]
[list]
[*]Buildable interior piece v Habitat Builderu. Model mini-Aurory (mesh z Starship Souvenir).
[*]Klikni na majak -> menu: pojmenuj ho, ukaze vsechny ostatni majaky s vzdalenosti a cenou, teleport na libovolny.
[*]Energeticka cena je umerna vzdalenosti (zaklad + per-100m faktor).
[*][b]3 Efficiency cipy[/b] (MK1/MK2/MK3) - vyrob je, osad v UI majaku. Kazdy tier snizuje cenu (default 75% / 50% / 25% z plne).
[*]Teleport te presne na 2 m pred cilovy majak, celem k nemu - konzistentni misto dopadu.
[/list]

[b]Stehovani skrini (Locker Mover)[/b] [i](od v0.2.0)[/i]
[list]
[*]Presun plnou skrin bez rucniho vyndavani kazde polozky. Zamer plny Locker / Wall Locker / Waterproof Locker / Carryall a stiskni klavesu (default [code]G[/code]) - obsah se presune do in-memory clipboardu, skrin zustane prazdna (pripravena k demontazi).
[*]Zamer novou prazdnou skrin stejneho typu a stiskni klavesu znovu - obsah se presype zpet.
[*]Plne kompatibilni s [b]Inventory Compressorem[/b]: slisovane polozky zustavaji 1x1 i behem presunu.
[*]Single-slot clipboard. Aktualni omezeni: obsah clipboardu neprezije save/quit (v1 neperzistuje).
[/list]

[size=4][b]Kompatibilita[/b][/size]

Mod detekuje a respektuje tyto dalsi mody:
[list]
[*][b]RamunesCustomizedStorage[/b] - nase locker resize se nezapne, pokud ho mas
[*][b]AdvancedInventory[/b] - nase (planovana) scrollable inventar se nezapne
[*][b]BagEquipment[/b] - nase batohy se nezapnou
[*][b]SlotExtender[/b] - detekovan, extra Chip sloty pomahaji s multi-chip setupem
[/list]

[size=4][b]Konfigurace[/b][/size]

[list]
[*][b]Options menu[/b] (Options -> Mods) - kazda featura ma vlastni sekci s toggly a slidery. Zmeny se ulozi do [code]BepInEx/config/InferiusQoL/config.json[/code] a aplikuji runtime kde je to mozne.
[*][b]Blacklist soubor[/b] v [code]BepInEx/plugins/InferiusQoL/Data/CompressorBlacklist.json[/code] - edituj ktery TechType Compressor nezmensuje.
[*][b]Konzolove prikazy[/b] ([code]~[/code] klavesa): [code]qol_status[/code] pro prehled, [code]qol_log_level[/code] pro zmenu verbosity, dalsi.
[/list]

[size=4][b]Pozadavky[/b][/size]

[list]
[*]Subnautica (ne Below Zero)
[*]BepInEx 5.x
[*]Nautilus (moderni nahrada SMLHelperu)
[/list]

[size=4][b]Podekovani[/b][/size]

Postaveno na BepInEx, Nautilus a Harmony. Preklady do dalsich jazyku vitany - staci JSON soubor do [code]LanguageFiles/[/code].
