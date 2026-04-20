namespace InferiusQoL.Config;

using HarmonyLib;
using InferiusQoL.Features.InventoryResize;
using InferiusQoL.Features.LockerResize;
using InferiusQoL.Logging;
using Nautilus.Json;

/// <summary>
/// Postfix na ConfigFile.Save(). Vola se kdykoliv Nautilus ulozi config po zmene
/// slideru/toggle v Options menu (diky SaveOn = ChangeValue). Zajistuje, ze po zmene
/// configu se runtime aplikuji featury, ktere umi zmenu absorbovat za behu.
/// </summary>
[HarmonyPatch(typeof(ConfigFile), nameof(ConfigFile.Save))]
public static class ConfigSavePatch
{
    [HarmonyPostfix]
    public static void Postfix(ConfigFile __instance)
    {
        // Patch je globalni pres vsechny ConfigFile instance (i z jinych modu).
        // Reagujeme jen na nas config.
        if (!(__instance is InferiusConfig cfg)) return;

        // Nautilus muze pri kazdem otevreni Options vytvorit novou instanci configu,
        // ktera NENI nas singleton. Proto predavame primo __instance z patchu
        // (ma aktualni hodnoty, ktere uzivatel ve slideru nastavil).
        var singleton = InferiusConfig.Instance;
        var isSingleton = ReferenceEquals(cfg, singleton);
        QoLLog.Info(Category.Config,
            $"Save: rows={cfg.InventoryExtraRows} cols={cfg.InventoryExtraCols} "
            + $"(singleton_rows={singleton.InventoryExtraRows}, same_instance={isSingleton})");

        try
        {
            InventoryResizePatch.ApplyRuntime(cfg);
            StorageContainer_Awake_Patch.ApplyRuntime();
        }
        catch (System.Exception ex)
        {
            QoLLog.Error(Category.Config, "ApplyRuntime after Save threw", ex);
        }
    }
}
