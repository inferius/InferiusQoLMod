namespace InferiusQoL.Features.LockerMover;

using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Facade pro registraci LockerMover feature. Manager MonoBehaviour
/// zavesujeme az na Player.Awake (postfix patch nize) protoze GO vytvoreny
/// v BepInEx Plugin.Awake pred scene loadem se ocitne v DontDestroyOnLoad
/// limbu - Awake fire ale Update netika.
/// </summary>
public static class LockerMoverFeature
{
    private static GameObject? _hostGO;

    public static void Init()
    {
        // Samotne vytvoreni manageru deferuje Harmony patch na Player.Awake.
        // GO vytvoreny v BepInEx Plugin.Awake pred scene loadem skonci v
        // DontDestroyOnLoad limbu - Awake fire ale Update netika.
    }

    internal static void EnsureManager()
    {
        if (_hostGO != null) return;

        _hostGO = new GameObject("InferiusQoL_LockerMoverManager");
        Object.DontDestroyOnLoad(_hostGO);
        _hostGO.AddComponent<LockerMoverManager>();
        QoLLog.Info(Category.LockerMover, "LockerMover initialized");
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
public static class LockerMoverPlayerAwakePatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!InferiusConfig.Instance.LockerMoverEnabled) return;
        LockerMoverFeature.EnsureManager();
    }
}
