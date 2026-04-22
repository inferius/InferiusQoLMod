## Inferius Quality of Life

Modularni QoL balicek pro Subnauticu. Jeden mod obsahuje vsechna zlepseni nize - kazde je konfigurovatelne v hlavnim menu (Options -> Mods -> Inferius Quality of Life) a lze zapinat/vypinat samostatne.

## Featury

| Feature | Co dela |
|-|-|
| **Locker resize** | Zvetsi vanilla Locker i Wall Locker na konfigurovatelnou velikost (default 6x8 / 4x5). Runtime - zmena v Options ihned vidi ve hre. |
| **Inventory resize** | Pridava konfigurovatelny pocet radku/sloupcu do inventare hrace. Runtime update. |
| **Batohy (3 tiery)** | Osazuji se do Chip slotu, kazdy tier pridava N radku extra. Progressive recipe chain. |
| **Seamoth Turbo (3 tiery)** | Upgrade modul pro Seamoth. Sprint key + modul = boost rychlosti s vyssi spotrebou. Surface falloff, Power Upgrade Module discount. |
| **Merged Tanks (4 tiery)** | Kyslikove lahve klonovane z Plasteel s nasobenou kapacitou. T4 je lightweight bez speed penalty. |
| **Reinforced + Hyper baterie** | Nove tiery baterii/power cellu s vyssi kapacitou (default 250/500 mid, 1500/3000 endgame). |
| **Inventory Compressor chip** | Chip v Chip slotu - zmensi vsechny ne-blacklistovane items na 1x1. Per-instance persistent marker, funguje napric save/load a napric kontejnery. |
| **Teleport Beacon** | Buildable teleport zarizeni. UI menu s pojmenovanim, vyberem cile, vzdalenosti, cenou. 3 efficiency chipy pro snizeni energy cost. Model mini-Aurory. |
| **Locker Mover** *(od v0.2.0)* | Stehovani plnych skrini. Zamer na Locker/Wall Locker/Waterproof Locker/Carryall + stisk klavesy (default `G`) -> obsah do clipboardu. Stara skrin prazdna = deconstruct bezne. Opakovany stisk na nove prazdne skrini stejneho typu = obsah se presype zpet. Plne kompatibilni s Inventory Compressorem (slisovane polozky zustanou 1x1). Single-slot, in-memory (v1 neperzistuje pres save/quit). |

## Detekce konfliktu

Pri startu detekujeme nainstalovane mody:
- `RamunesCustomizedStorage` -> vlastni locker resize se nezapne (kompatibilita)
- `AdvancedInventory` -> vlastni scrollable container se nezapne
- `BagEquipment` -> vlastni batohy se nezapnou
- `SlotExtender` -> detekovany, batohy mohou vyuzit extra Chip sloty

## Konzoleove prikazy (`~` konzole)

- `qol_status` - prehled vsech featur + stav detekce
- `qol_log_level <None|Info|Debug|Trace>` - runtime zmena verbosity
- `qol_apply` - force reapply config (inventory)
- Dalsi `qol_*` prikazy pro diagnostiku

## Konfigurace

**V Options menu**: kazda feature ma vlastni sekci s tooglem enabled + slidery. Zmeny se ukladaji do `BepInEx/config/InferiusQoL/config.json` a aplikuji runtime.

**Extra JSON soubory** v `BepInEx/plugins/InferiusQoL/`:
- `Data/CompressorBlacklist.json` - TechTypes ktere **nelze** lisovat (baterie, lahve, ryby, vajicka). User muze editovat.
- `compressed-items.json` - perzistentni seznam slisovanych items (auto-generovany).
- `beacons.json` - perzistentni beacon data (jmena, efficiency tier) (auto-generovany).
- `LanguageFiles/English.json`, `Czech.json` - lokalizacni soubory.

## Lokalizace

Subnautica podporuje vice jazyku. Vsechny texty modu (nazvy itemu, tooltipy, konzoleove hlasky) jsou dostupne v Angličtine a Češtine. Dalsi jazyky muzou byt pridany vytvorenim noveho JSON souboru v `LanguageFiles/`.

## Zavislosti

- Subnautica (ne Below Zero)
- BepInEx 5.x
- Nautilus (ex-SMLHelper 2.x)

## Source Code

| Component | File |
|-|-|
| Plugin entry point | [Plugin.cs](InferiusQoL/Plugin.cs) |
| Config | [InferiusConfig.cs](InferiusQoL/Config/InferiusConfig.cs) |
| Locker resize | [LockerResizePatch.cs](InferiusQoL/Features/LockerResize/LockerResizePatch.cs) |
| Inventory resize | [InventoryResizePatch.cs](InferiusQoL/Features/InventoryResize/InventoryResizePatch.cs) |
| Batohy | [BackpackItems.cs](InferiusQoL/Features/Backpacks/BackpackItems.cs) |
| Seamoth Turbo | [SeamothTurboItems.cs](InferiusQoL/Features/SeamothTurbo/SeamothTurboItems.cs), [SeamothTurboPatch.cs](InferiusQoL/Features/SeamothTurbo/SeamothTurboPatch.cs) |
| Merged Tanks | [TankWelderItems.cs](InferiusQoL/Features/TankWelder/TankWelderItems.cs) |
| Baterie | [BatteryItems.cs](InferiusQoL/Features/Batteries/BatteryItems.cs) |
| Compressor | [CompressorItem.cs](InferiusQoL/Features/Compressor/CompressorItem.cs), [CompressorSizePatch.cs](InferiusQoL/Features/Compressor/CompressorSizePatch.cs) |
| Teleport Beacon | [TeleportBeaconItem.cs](InferiusQoL/Features/TeleportBeacon/TeleportBeaconItem.cs), [TeleportBeaconBehavior.cs](InferiusQoL/Features/TeleportBeacon/TeleportBeaconBehavior.cs) |
| Locker Mover | [LockerMoverFeature.cs](InferiusQoL/Features/LockerMover/LockerMoverFeature.cs), [LockerMoverManager.cs](InferiusQoL/Features/LockerMover/LockerMoverManager.cs), [LockerMoverClipboard.cs](InferiusQoL/Features/LockerMover/LockerMoverClipboard.cs) |

## Licence

TBD.
