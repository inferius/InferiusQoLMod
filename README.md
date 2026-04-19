## Mods

| Name | Version | Description | Source Code |
|-|-|-|-|
| **Inferius Quality of Life** | 0.1.0 | Modularni QoL balicek: vetsi inventar, batohy, lis, retriever terminal, Seamoth turbo, spojovani lahvi, rework baterii, teleport beacon | [here](InferiusQoL/Plugin.cs) |

## Status

Ranne stadium - aktualne jen scaffolding (config framework, logging, console commands).
Jednotlive featury se implementuji postupne.

### Console commands (Subnautica `~` konzole)

- `qol_status` - prehled stavu vsech featur
- `qol_log_level <None|Info|Debug|Trace>` - runtime zmena verbosity
- `qol_retriever_rescan`, `qol_retriever_dump` - retriever diagnostika
- `qol_seamoth_boost_state` - Seamoth turbo diagnostika
- `qol_teleport_list` - seznam teleport beacons
- `qol_migrate_batteries` - force migrace baterii po reworku
