namespace InferiusQoL.Features.LockerResize;

using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

[HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake))]
public static class StorageContainer_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(StorageContainer __instance)
    {
        var cfg = InferiusConfig.Instance;

        if (!cfg.LockerResizeEnabled) return;
        if (Plugin.HasCustomizedStorage) return;

        var techType = ResolveTechType(__instance.gameObject);
        int newW = 0, newH = 0;

        switch (techType)
        {
            case TechType.Locker:
                newW = cfg.LockerWidth;
                newH = cfg.LockerHeight;
                break;
            case TechType.SmallLocker:
                newW = cfg.WallLockerWidth;
                newH = cfg.WallLockerHeight;
                break;
            default:
                return;
        }

        if (newW <= 0 || newH <= 0) return;
        if (__instance.width == newW && __instance.height == newH)
        {
            QoLLog.Trace(Category.Locker, $"{techType}: already {newW}x{newH}, skip");
            return;
        }

        var oldW = __instance.width;
        var oldH = __instance.height;

        __instance.Resize(newW, newH);
        QoLLog.Info(Category.Locker, $"{techType}: {oldW}x{oldH} -> {newW}x{newH}");
    }

    private static TechType ResolveTechType(GameObject go)
    {
        var techTag = go.GetComponent<TechTag>();
        if (techTag != null && techTag.type != TechType.None)
            return techTag.type;

        var constructable = go.GetComponent<Constructable>();
        if (constructable != null && constructable.techType != TechType.None)
            return constructable.techType;

        var parentConstructable = go.GetComponentInParent<Constructable>();
        if (parentConstructable != null && parentConstructable.techType != TechType.None)
            return parentConstructable.techType;

        return TechType.None;
    }

    /// <summary>
    /// Iteruje vsechny StorageContainer v scene a aplikuje aktualni config velikost.
    /// Vola se pri zmene configu v Options (ConfigSavePatch).
    /// </summary>
    public static void ApplyRuntime()
    {
        var cfg = InferiusConfig.Instance;
        if (!cfg.LockerResizeEnabled) return;
        if (Plugin.HasCustomizedStorage) return;

        var containers = UnityEngine.Object.FindObjectsOfType<StorageContainer>();
        int resized = 0;
        foreach (var sc in containers)
        {
            if (sc == null) continue;
            var techType = ResolveTechType(sc.gameObject);
            int newW = 0, newH = 0;
            switch (techType)
            {
                case TechType.Locker:
                    newW = cfg.LockerWidth;
                    newH = cfg.LockerHeight;
                    break;
                case TechType.SmallLocker:
                    newW = cfg.WallLockerWidth;
                    newH = cfg.WallLockerHeight;
                    break;
                default:
                    continue;
            }
            if (newW <= 0 || newH <= 0) continue;
            if (sc.width == newW && sc.height == newH) continue;

            sc.Resize(newW, newH);
            resized++;
        }

        if (resized > 0)
            QoLLog.Info(Category.Locker, $"ApplyRuntime: {resized} lockers resized");
    }
}
