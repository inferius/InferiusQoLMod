namespace InferiusQoL.Features.Compressor;

using System.Collections.Generic;
using System.Linq;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Bezpecna dekomprese: najde vsechny Pickupable s markerem, vytlaci je z
/// vsech containeru (Inventory + StorageContainer), dropne jako world loot
/// u hrace, a vymaze markery. Pickupy zpet pak probehnou vanilla velikosti.
///
/// Iteruje i INACTIVE StorageContainers pres Resources.FindObjectsOfTypeAll
/// (Carryally/Waterproof Lockery v inventari jsou SetActive(false) ale stale
/// maji obsah).
///
/// Neresi nuclear reactor / bioreactor (komprimovane items tam nemaji smysl).
/// </summary>
public static class CompressorDecompressAll
{
    public static string Run()
    {
        var player = Player.main;
        if (player == null) return "Player not loaded";

        var targets = CollectCompressed();
        if (targets.Count == 0)
        {
            CompressorSaveManager.ClearAll();
            return "No compressed items found. Markers cleared.";
        }

        int dropped = 0;
        int failed = 0;
        int removeFailed = 0;

        var dropBase = player.transform.position + Vector3.up * 0.5f;

        foreach (var entry in targets)
        {
            try
            {
                var p = entry.Pickupable;
                if (p == null)
                {
                    QoLLog.Warning(Category.Compressor, "Decompress: pickupable null, skip");
                    continue;
                }

                var uid = p.GetComponent<UniqueIdentifier>();
                var uidStr = uid?.Id ?? "<no-uid>";
                var tt = p.GetTechType();

                // 1. Remove from container.
                bool removed = false;
                if (entry.SourceContainer != null)
                {
                    removed = entry.SourceContainer.RemoveItem(p, true);
                }
                if (!removed)
                {
                    QoLLog.Warning(Category.Compressor,
                        $"  RemoveItem FAILED for {tt} (uid={uidStr}) - skipping");
                    removeFailed++;
                    continue;
                }

                // 2. Marker pryc.
                if (uid != null) CompressorSaveManager.Remove(uid.Id);

                // 3. Activate + detach (container muze mit item kinematicky
                // deactivovany + reparented).
                p.gameObject.SetActive(true);
                p.transform.SetParent(null, true);

                // 4. Drop do sveta. checkPosition=false at Subnautica neodmitne
                // pozici blizko prekazek - chceme je proste shodit na zem.
                p.Drop(dropBase, Vector3.zero, false);

                QoLLog.Debug(Category.Compressor,
                    $"  Decompressed {tt} (uid={uidStr})");
                dropped++;
            }
            catch (System.Exception ex)
            {
                failed++;
                QoLLog.Error(Category.Compressor, "Decompress item threw", ex);
            }
        }

        CompressorSaveManager.ClearAll();
        QoLLog.Info(Category.Compressor,
            $"Decompress all complete: dropped={dropped}, remove_failed={removeFailed}, exceptions={failed}");

        return $"Decompress: {dropped} dropped at your feet, "
               + $"{removeFailed} couldn't be removed, {failed} errors. Check log for details.";
    }

    private static List<Entry> CollectCompressed()
    {
        var list = new List<Entry>();

        // Inventory hrace.
        var inv = Inventory.main;
        if (inv?.container != null)
        {
            foreach (var invItem in inv.container.ToList())
            {
                var p = invItem?.item;
                if (p == null) continue;
                var uid = p.GetComponent<UniqueIdentifier>();
                if (uid == null) continue;
                if (!CompressorSaveManager.IsInstanceCompressed(uid.Id)) continue;
                list.Add(new Entry { Pickupable = p, SourceContainer = inv.container });
            }
        }

        // Vsechny StorageContainer v scene (lockery, carryally vcetne inaktivnich).
        var all = Resources.FindObjectsOfTypeAll<StorageContainer>()
            .Where(sc => sc != null && sc.gameObject != null
                         && !string.IsNullOrEmpty(sc.gameObject.scene.name))
            .ToArray();
        int containerCount = 0;
        foreach (var sc in all)
        {
            if (sc?.container == null) continue;
            containerCount++;
            foreach (var invItem in sc.container.ToList())
            {
                var p = invItem?.item;
                if (p == null) continue;
                var uid = p.GetComponent<UniqueIdentifier>();
                if (uid == null) continue;
                if (!CompressorSaveManager.IsInstanceCompressed(uid.Id)) continue;
                list.Add(new Entry { Pickupable = p, SourceContainer = sc.container });
            }
        }
        QoLLog.Info(Category.Compressor,
            $"Decompress scan: {containerCount} storage containers, {list.Count} compressed items found, "
            + $"{CompressorSaveManager.Count} markers in save file");

        return list;
    }

    private sealed class Entry
    {
        public Pickupable Pickupable = null!;
        public ItemsContainer? SourceContainer;
    }
}
