namespace InferiusQoL.Features.Compressor;

using System;
using System.Collections.Generic;
using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;

/// <summary>
/// Pri osazeni/odsazeni Compressor chipu projdeme items v Player inventari a
/// refreshnem (remove + add) jen ty, kterych se komprese realne tyka:
///
///   - Ne-blacklistovane items s vanilla velikosti > 1x1
///   - A soucasnou velikosti v inventari, ktera nesedi s cilovou velikosti
///
/// Typicky jsou to 5-10 itemu (batoh, scanner, welder, propulsion cannon atd.),
/// ne stovky. Refresh je rychly.
/// </summary>
[HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake))]
public static class Compressor_Inventory_Awake_Patch
{
    private static bool _hooked = false;

    [HarmonyPostfix]
    public static void Postfix(Inventory __instance)
    {
        if (_hooked) return;
        if (__instance?.equipment == null) return;

        // Napoji jen onEquip. Odsazeni chipu ZAMERNE nic neodlisuje - jednou slisovane
        // items zustavaji 1x1. Dava to smysl gameplay-wise: chip je permanentni upgrade
        // inventare, ne toggle.
        __instance.equipment.onEquip += OnEquipmentChipEquipped;
        _hooked = true;

        QoLLog.Debug(Category.Compressor, "Compressor onEquip listener hooked (unequip does not un-compress)");
    }

    private static void OnEquipmentChipEquipped(string slot, InventoryItem item)
    {
        if (!IsOurChip(item)) return;
        QoLLog.Info(Category.Compressor, $"Compressor equipped ({slot}) - compressing eligible inventory items");
        RefreshInventoryItemSizes(targetCompressed: true);
    }

    private static bool IsOurChip(InventoryItem item)
    {
        if (item?.item == null) return false;
        if (CompressorItem.TechType == TechType.None) return false;
        if (item.item.GetTechType() != CompressorItem.TechType) return false;
        var cfg = InferiusConfig.Instance;
        return cfg.CompressorEnabled;
    }

    /// <summary>
    /// Vyfiltruje jen items, ktere jsou > 1x1 vanilla (kompresovatelne) a soucasne
    /// jejich aktualni velikost v inventari neodpovida cilove. Tyto items refreshne
    /// remove + add cyklem, coz prepocita grid layout.
    /// </summary>
    private static void RefreshInventoryItemSizes(bool targetCompressed)
    {
        var inv = Inventory.main;
        if (inv?.container == null) return;

        try
        {
            var toRefresh = new List<Pickupable>();

            foreach (var invItem in inv.container)
            {
                if (invItem?.item == null) continue;
                var tt = invItem.item.GetTechType();

                // Blacklisted items nikdy nekompresujeme.
                if (CompressorBlacklist.IsBlacklisted(tt)) continue;

                // Vanilla velikost (z cache). Pokud uz je 1x1, item neni ovlivnen chipem.
                var vanillaSize = TechData_GetItemSize_Patch.GetVanillaSize(tt);
                if (vanillaSize.x <= 1 && vanillaSize.y <= 1) continue;

                // Zjistit zda soucasna velikost v inventari odpovida cilove.
                bool currentlyCompressed = (invItem.width <= 1 && invItem.height <= 1);
                if (targetCompressed && currentlyCompressed) continue; // uz je 1x1
                if (!targetCompressed && !currentlyCompressed) continue; // uz je vanilla

                toRefresh.Add(invItem.item);
            }

            if (toRefresh.Count == 0)
            {
                QoLLog.Debug(Category.Compressor, "No inventory items need refresh");
                return;
            }

            // Remove + re-add. AddItem zavola TechData.GetItemSize (nas patch s aktualnim
            // chip stavem) a vytvori InventoryItem s novou velikosti. Container sam
            // prelayoutuje grid.
            int removed = 0;
            foreach (var p in toRefresh)
            {
                if (inv.container.RemoveItem(p, forced: true))
                    removed++;
            }

            int readded = 0;
            foreach (var p in toRefresh)
            {
                if (inv.container.AddItem(p) != null)
                    readded++;
            }

            QoLLog.Info(Category.Compressor,
                $"Inventory refresh: candidates={toRefresh.Count}, removed={removed}, re-added={readded}");
        }
        catch (Exception ex)
        {
            QoLLog.Error(Category.Compressor, "RefreshInventoryItemSizes failed", ex);
        }
    }
}
