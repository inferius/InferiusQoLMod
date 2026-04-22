namespace InferiusQoL.Features.OxygenRefill;

using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;

/// <summary>
/// Rychlejsi auto-refill kyslikoveho tanku po vynoru nad hladinu / pobytu v
/// moonpoolu / habitatu.
///
/// Vanilla chova OxygenManager.oxygenUnitsPerSecondSurface = 30f jako rate
/// pri kterem se v Update() pridava oxygen kdyz hrac je nad oceanLevel-1
/// NEBO CanBreathe() (v base). Staci zmenit pole na config hodnotu a vanilla
/// logika si dal zachova svoje podminky (surface + breathable).
///
/// POZOR: OxygenManager nema vlastni Awake/Start metodu - hookujeme pres
/// Player.Awake postfix (Player drzi OxygenManager jako child komponenta).
/// Runtime re-apply resi ConfigSavePatch.ApplyRuntime.
/// </summary>
public static class OxygenRefillFeature
{
    public static void ApplyTo(OxygenManager mgr)
    {
        if (mgr == null) return;
        var cfg = InferiusConfig.Instance;
        if (!cfg.OxygenRefillEnabled)
        {
            // Vanilla default 30. Pokud user vypnul feature za behu, vratime 30.
            mgr.oxygenUnitsPerSecondSurface = 30f;
            return;
        }
        mgr.oxygenUnitsPerSecondSurface = cfg.OxygenRefillRate;
        QoLLog.Trace(Category.Oxygen,
            $"OxygenManager rate -> {cfg.OxygenRefillRate}/s");
    }

    public static void ApplyRuntime()
    {
        var all = UnityEngine.Object.FindObjectsOfType<OxygenManager>();
        foreach (var m in all) ApplyTo(m);
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
public static class Player_Awake_OxygenRefillPatch
{
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        if (__instance == null) return;
        var mgr = __instance.GetComponent<OxygenManager>();
        OxygenRefillFeature.ApplyTo(mgr);
    }
}

/// <summary>
/// Doplneni O2 do vsech tanku v inventari (neequipnuty - equipnuty resi vanilla
/// via registered sources). Vola se za vanilla AddOxygenAtSurface se stejnou
/// podminkou (hladina nebo in-base).
/// </summary>
[HarmonyPatch(typeof(OxygenManager), nameof(OxygenManager.AddOxygenAtSurface))]
public static class OxygenManager_AddOxygenAtSurface_Patch
{
    [HarmonyPostfix]
    public static void Postfix(OxygenManager __instance, float timeInterval)
    {
        try
        {
            var cfg = InferiusConfig.Instance;
            if (!cfg.OxygenRefillEnabled) return;
            if (!cfg.OxygenRefillInventoryTanks) return;

            var player = Player.main;
            if (player == null) return;

            bool aboveWater = player.transform.position.y > Ocean.GetOceanLevel() - 1f;
            bool canBreathe = player.CanBreathe();
            if (!aboveWater && !canBreathe) return;

            var inv = Inventory.main;
            if (inv?.container == null) return;

            float amount = timeInterval * cfg.OxygenRefillRate;
            foreach (var invItem in inv.container)
            {
                if (invItem?.item == null) continue;
                var ox = invItem.item.GetComponent<Oxygen>();
                if (ox == null) continue;
                ox.AddOxygen(amount);
            }
        }
        catch (System.Exception ex)
        {
            QoLLog.Error(Category.Oxygen, "Inventory tank refill threw", ex);
        }
    }
}
