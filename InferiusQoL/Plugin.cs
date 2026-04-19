namespace InferiusQoL;

using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Console;
using InferiusQoL.Features.SeamothTurbo;
using InferiusQoL.Localization;
using InferiusQoL.Logging;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Plugin : BaseUnityPlugin
{
    public const string HarmonyId = MyPluginInfo.PLUGIN_GUID;

    internal static Plugin Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony Harmony { get; } = new Harmony(HarmonyId);

    internal static bool HasCustomizedStorage { get; private set; }
    internal static bool HasAdvancedInventory { get; private set; }
    internal static bool HasBagEquipment { get; private set; }

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;

        var cfg = InferiusConfig.Instance;
        cfg.Load();

        QoLLog.Initialize(Logger, cfg.Verbosity);
        QoLLog.Info(Category.Core, $"Loading {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}");
        QoLLog.Info(Category.Config, "Config loaded");

        L.LoadAll();
        QoLLog.Info(Category.Config, "Localization loaded");

        ConsoleCommands.Register();
        QoLLog.Info(Category.Core, "Console commands registered");

        // Registrace custom TechTypes (musi byt v Awake, drive nez hra vytvori craft tree).
        if (cfg.SeamothTurboEnabled)
            SeamothTurboItem.Register();

        QoLLog.Info(Category.Core, "Awake completed (detekce cizich modu probehne v Start())");
    }

    private void Start()
    {
        // Start() bezi az po vsech Awake() ostatnich pluginu, takze Chainloader.PluginInfos
        // je uz kompletni. V Awake je detekce neuplna - nekteri pluginy se nacitaji pozdeji.
        DetectExternalMods();

        // Harmony patche az po detekci, aby patche mohly checknout Plugin.HasX flagy.
        Harmony.PatchAll(typeof(Plugin).Assembly);
        QoLLog.Info(Category.Core, "Harmony patches applied");
    }

    private static void DetectExternalMods()
    {
        HasCustomizedStorage = FindPlugin(new[] { "customizedstorage" }, out var csInfo);
        HasAdvancedInventory = FindPlugin(new[] { "advancedinventory" }, out var aiInfo);
        HasBagEquipment = FindPlugin(new[] { "bagequipment" }, out var beInfo);

        LogDetection("CustomizedStorage", "locker resize", HasCustomizedStorage, csInfo);
        LogDetection("AdvancedInventory", "scrollable container", HasAdvancedInventory, aiInfo);
        LogDetection("BagEquipment", "batohy", HasBagEquipment, beInfo);
    }

    private static bool FindPlugin(string[] needles, out string info)
    {
        foreach (var kvp in Chainloader.PluginInfos)
        {
            var guid = kvp.Key?.ToLowerInvariant() ?? "";
            var meta = kvp.Value?.Metadata;
            var name = meta?.Name?.ToLowerInvariant() ?? "";
            var asmName = kvp.Value?.Instance?.GetType().Assembly.GetName().Name?.ToLowerInvariant() ?? "";
            foreach (var needle in needles)
            {
                var n = needle.ToLowerInvariant();
                if (guid.Contains(n) || name.Contains(n) || asmName.Contains(n))
                {
                    info = $"{meta?.Name} (guid={kvp.Key}) v{meta?.Version}";
                    return true;
                }
            }
        }
        info = "";
        return false;
    }

    private static void LogDetection(string label, string featureDesc, bool found, string info)
    {
        if (found)
            QoLLog.Warning(Category.Core,
                $"{label} detected: {info} -> vlastni {featureDesc} se nezapne (konflikt).");
        else
            QoLLog.Info(Category.Core, $"{label} not detected.");
    }
}
