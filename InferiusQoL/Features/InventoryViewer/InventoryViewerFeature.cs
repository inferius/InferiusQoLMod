namespace InferiusQoL.Features.InventoryViewer;

using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Init pres Harmony postfix na Player.Awake (stejny pattern jako LockerMover) -
/// GO vytvoreny v BepInEx Plugin.Awake by skoncil v DontDestroyOnLoad limbu.
/// </summary>
public static class InventoryViewerFeature
{
    private static GameObject? _hostGO;

    public static void Init() { /* deferred to Player.Awake */ }

    internal static void EnsureManager()
    {
        if (_hostGO != null) return;
        _hostGO = new GameObject("InferiusQoL_InventoryViewerManager");
        Object.DontDestroyOnLoad(_hostGO);
        _hostGO.AddComponent<InventoryViewerManager>();
        QoLLog.Info(Category.Inventory, "InventoryViewer initialized");
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
public static class InventoryViewer_Player_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!InferiusConfig.Instance.InventoryViewerEnabled) return;
        InventoryViewerFeature.EnsureManager();
    }
}
