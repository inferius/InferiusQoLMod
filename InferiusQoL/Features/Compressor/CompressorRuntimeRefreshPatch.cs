namespace InferiusQoL.Features.Compressor;

using System;
using System.Collections.Generic;
using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;

/// <summary>
/// Dva listenery na Player inventory:
///
/// 1. equipment.onEquip - pri osazeni Compressor chipu projdeme vsechny
///    eligible items v inventari, oznacime jejich instance a refresh.
///    (Bulk compression existujicich itemu.)
///
/// 2. container.onAddItem - kdyz hrac pickne novy item a chip je osazen,
///    automaticky oznacime novou instanci a refreshneme. (Continuous
///    compression pro nove pickups.)
///
/// Flag _isRefreshing zabrani rekurzi - nase RemoveItem/AddItem by jinak
/// znovu trigger onAddItem event.
/// </summary>
[HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake))]
public static class Compressor_Inventory_Awake_Patch
{
    private static bool _hooked = false;
    private static bool _isRefreshing = false;

    [HarmonyPostfix]
    public static void Postfix(Inventory __instance)
    {
        if (_hooked) return;
        if (__instance?.equipment == null) return;
        if (__instance.container == null) return;

        __instance.equipment.onEquip += OnEquipmentChipEquipped;
        __instance.container.onAddItem += OnPlayerContainerItemAdded;
        _hooked = true;

        QoLLog.Debug(Category.Compressor,
            "Compressor listeners hooked: equipment.onEquip + container.onAddItem");
    }

    // ============================================================
    // Bulk compression on chip equip
    // ============================================================

    private static void OnEquipmentChipEquipped(string slot, InventoryItem item)
    {
        if (!IsOurChip(item)) return;
        QoLLog.Info(Category.Compressor,
            $"Compressor equipped ({slot}) - bulk-marking eligible items in inventory");
        MarkAndRefreshInventory();
    }

    private static bool IsOurChip(InventoryItem item)
    {
        if (item?.item == null) return false;
        if (CompressorItem.TechType == TechType.None) return false;
        if (item.item.GetTechType() != CompressorItem.TechType) return false;
        var cfg = InferiusConfig.Instance;
        return cfg.CompressorEnabled;
    }

    private static void MarkAndRefreshInventory()
    {
        var inv = Inventory.main;
        if (inv?.container == null) return;

        try
        {
            var toRefresh = new List<Pickupable>();
            int newInstances = 0;

            foreach (var invItem in inv.container)
            {
                if (invItem?.item == null) continue;
                var pickupable = invItem.item;
                var tt = pickupable.GetTechType();

                if (CompressorBlacklist.IsBlacklisted(tt)) continue;
                if (invItem.width <= 1 && invItem.height <= 1) continue;

                var uid = pickupable.GetComponent<UniqueIdentifier>();
                if (uid == null || string.IsNullOrEmpty(uid.Id)) continue;

                if (CompressorSaveManager.MarkCompressed(uid.Id))
                    newInstances++;

                toRefresh.Add(pickupable);
            }

            if (newInstances > 0)
                CompressorSaveManager.Save();

            if (toRefresh.Count == 0) return;

            _isRefreshing = true;
            try
            {
                int removed = 0;
                foreach (var p in toRefresh)
                    if (inv.container.RemoveItem(p, forced: true)) removed++;

                int readded = 0;
                foreach (var p in toRefresh)
                    if (inv.container.AddItem(p) != null) readded++;

                QoLLog.Info(Category.Compressor,
                    $"Bulk compression: {toRefresh.Count} candidates, {newInstances} new, removed={removed}, re-added={readded}");
            }
            finally
            {
                _isRefreshing = false;
            }
        }
        catch (Exception ex)
        {
            QoLLog.Error(Category.Compressor, "MarkAndRefreshInventory failed", ex);
            _isRefreshing = false;
        }
    }

    // ============================================================
    // Auto-compression on new pickups (when chip is equipped)
    // ============================================================

    private static void OnPlayerContainerItemAdded(InventoryItem newItem)
    {
        if (_isRefreshing) return;
        if (newItem?.item == null) return;

        var cfg = InferiusConfig.Instance;
        if (!cfg.CompressorEnabled) return;

        // Jen kdyz je chip osazen.
        if (!CompressorItem.IsEquipped()) return;

        var pickupable = newItem.item;
        var tt = pickupable.GetTechType();

        if (CompressorBlacklist.IsBlacklisted(tt)) return;
        if (newItem.width <= 1 && newItem.height <= 1) return;

        var uid = pickupable.GetComponent<UniqueIdentifier>();
        if (uid == null || string.IsNullOrEmpty(uid.Id)) return;

        // Oznacit novou instanci.
        bool wasNew = CompressorSaveManager.MarkCompressed(uid.Id);
        if (wasNew)
            CompressorSaveManager.Save();

        // Refresh jeden item (remove + add) aby InventoryItem constructor
        // pouzil novou 1x1 velikost.
        _isRefreshing = true;
        try
        {
            var inv = Inventory.main;
            if (inv?.container != null)
            {
                if (inv.container.RemoveItem(pickupable, forced: true))
                    inv.container.AddItem(pickupable);
            }

            QoLLog.Info(Category.Compressor,
                $"Auto-compressed new pickup: {tt} (uid {uid.Id})");
        }
        catch (Exception ex)
        {
            QoLLog.Error(Category.Compressor, "Auto-compress failed", ex);
        }
        finally
        {
            _isRefreshing = false;
        }
    }
}
