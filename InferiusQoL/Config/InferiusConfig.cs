namespace InferiusQoL.Config;

using InferiusQoL.Logging;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using Newtonsoft.Json;

// LoadOn zamerne bez MenuOpened - reload pri kazdem otevreni menu zpusoboval
// mizeni sliderů (nekompatibilita s nasim poctem fields). Load probehne jen
// jednou pri startu (manualne v Plugin.Awake volame .Load()) a config soubor
// se aktualizuje jen pri SaveOn.ChangeValue / SaveGame.
[Menu(MyPluginInfo.PLUGIN_NAME,
    SaveOn = MenuAttribute.SaveEvents.ChangeValue | MenuAttribute.SaveEvents.SaveGame)]
public class InferiusConfig : ConfigFile
{
    // =====================================================================
    // General
    // =====================================================================

    [Choice("Debug verbosity", new[] { "None", "Info", "Debug", "Trace" }, Order = 0)]
    public string Verbosity = "Info";

    // =====================================================================
    // Player inventory
    // =====================================================================

    [Toggle("Enlarge player inventory", Order = 100)]
    public bool InventoryResizeEnabled = true;

    [Slider("  Extra rows", 0, 6, DefaultValue = 2, Step = 1, Order = 101)]
    public int InventoryExtraRows = 2;

    [Slider("  Extra columns", 0, 4, DefaultValue = 0, Step = 1, Order = 102)]
    public int InventoryExtraCols = 0;

    // =====================================================================
    // Locker resize
    // =====================================================================

    [Toggle("Bigger locker capacity", Order = 200)]
    public bool LockerResizeEnabled = true;

    [Slider("  Locker width (cols)", 4, 12, DefaultValue = 6, Step = 1, Order = 201)]
    public int LockerWidth = 6;

    [Slider("  Locker height (rows)", 4, 16, DefaultValue = 8, Step = 1, Order = 202)]
    public int LockerHeight = 8;

    [Slider("  Wall locker width", 2, 8, DefaultValue = 4, Step = 1, Order = 203)]
    public int WallLockerWidth = 4;

    [Slider("  Wall locker height", 2, 10, DefaultValue = 5, Step = 1, Order = 204)]
    public int WallLockerHeight = 5;

    // =====================================================================
    // Backpacks
    // =====================================================================

    [Toggle("Enable backpacks", Order = 300)]
    public bool BackpacksEnabled = true;

    [Slider("  Small backpack extra rows", 1, 4, DefaultValue = 1, Step = 1, Order = 301)]
    public int BackpackSmallRows = 1;

    [Slider("  Medium backpack extra rows", 1, 6, DefaultValue = 2, Step = 1, Order = 302)]
    public int BackpackMediumRows = 2;

    [Slider("  Large backpack extra rows", 1, 8, DefaultValue = 3, Step = 1, Order = 303)]
    public int BackpackLargeRows = 3;

    // =====================================================================
    // Seamoth turbo
    // =====================================================================

    [Toggle("Enable Seamoth Turbo modules", Order = 400)]
    public bool SeamothTurboEnabled = true;

    // Integer percentage to avoid Nautilus float serialization bug. 100 = 1.00x.
    [Slider("  MK1 speed (%)", 100, 500, DefaultValue = 200, Step = 10, Order = 401)]
    public int SeamothTurboMK1SpeedPercent = 200;

    [Slider("  MK1 energy drain (%)", 100, 1500, DefaultValue = 450, Step = 25, Order = 402)]
    public int SeamothTurboMK1EnergyPercent = 450;

    [Slider("  MK2 speed (%)", 100, 600, DefaultValue = 330, Step = 10, Order = 403)]
    public int SeamothTurboMK2SpeedPercent = 330;

    [Slider("  MK2 energy drain (%)", 100, 2000, DefaultValue = 900, Step = 25, Order = 404)]
    public int SeamothTurboMK2EnergyPercent = 900;

    [Slider("  MK3 speed (%)", 100, 700, DefaultValue = 430, Step = 10, Order = 405)]
    public int SeamothTurboMK3SpeedPercent = 430;

    [Slider("  MK3 energy drain (%)", 100, 3500, DefaultValue = 1800, Step = 50, Order = 406)]
    public int SeamothTurboMK3EnergyPercent = 1800;

    [Slider("  Surface falloff distance (m)", 0, 60, DefaultValue = 30, Step = 1, Order = 407)]
    public int SeamothTurboSurfaceFalloffMeters = 30;

    [JsonIgnore] public float SeamothTurboMK1SpeedMultiplier => SeamothTurboMK1SpeedPercent / 100f;
    [JsonIgnore] public float SeamothTurboMK1EnergyMultiplier => SeamothTurboMK1EnergyPercent / 100f;
    [JsonIgnore] public float SeamothTurboMK2SpeedMultiplier => SeamothTurboMK2SpeedPercent / 100f;
    [JsonIgnore] public float SeamothTurboMK2EnergyMultiplier => SeamothTurboMK2EnergyPercent / 100f;
    [JsonIgnore] public float SeamothTurboMK3SpeedMultiplier => SeamothTurboMK3SpeedPercent / 100f;
    [JsonIgnore] public float SeamothTurboMK3EnergyMultiplier => SeamothTurboMK3EnergyPercent / 100f;

    // =====================================================================
    // Retriever
    // =====================================================================

    [Toggle("Enable Retriever terminal", Order = 500)]
    public bool RetrieverEnabled = true;

    [Slider("  Cost per retrieval (J)", 0, 50, DefaultValue = 5, Step = 1, Order = 501)]
    public int RetrieverActionCostJoules = 5;

    [Slider("  Min base power (%)", 0, 100, DefaultValue = 20, Step = 5, Order = 502)]
    public int RetrieverMinBasePowerPercent = 20;

    // =====================================================================
    // Compressor (lis)
    // =====================================================================

    [Toggle("Enable Compressor (item press)", Order = 600)]
    public bool CompressorEnabled = true;

    [Toggle("  Requires energy to compress", Order = 601)]
    public bool CompressorRequiresEnergy = true;

    [Slider("  Energy per compression (J)", 0, 100, DefaultValue = 10, Step = 1, Order = 602)]
    public int CompressorEnergyCost = 10;

    // =====================================================================
    // Tank Welder
    // =====================================================================

    [Toggle("Enable Tank Welder", Order = 700)]
    public bool TankWelderEnabled = true;

    // Integer percentage to avoid Nautilus float serialization bug.
    [Slider("  Tier 1 multiplier (%)", 100, 300, DefaultValue = 150, Step = 10, Order = 701)]
    public int TankWelderT1Percent = 150;

    [Slider("  Tier 2 multiplier (%)", 100, 300, DefaultValue = 200, Step = 10, Order = 702)]
    public int TankWelderT2Percent = 200;

    [Slider("  Tier 3 multiplier (%)", 100, 300, DefaultValue = 230, Step = 10, Order = 703)]
    public int TankWelderT3Percent = 230;

    [Slider("  Tier 4 multiplier (%)", 100, 400, DefaultValue = 230, Step = 10, Order = 704)]
    public int TankWelderT4Percent = 230;

    [JsonIgnore] public float TankWelderT1Multiplier => TankWelderT1Percent / 100f;
    [JsonIgnore] public float TankWelderT2Multiplier => TankWelderT2Percent / 100f;
    [JsonIgnore] public float TankWelderT3Multiplier => TankWelderT3Percent / 100f;
    [JsonIgnore] public float TankWelderT4Multiplier => TankWelderT4Percent / 100f;

    // =====================================================================
    // Batteries rework
    // =====================================================================

    [Toggle("Enable battery rework", Order = 800)]
    public bool BatteryReworkEnabled = true;

    [Slider("  Vanilla Battery capacity", 25, 200, DefaultValue = 50, Step = 5, Order = 801)]
    public int VanillaBatteryCapacity = 50;

    [Slider("  Vanilla Power Cell capacity", 50, 400, DefaultValue = 100, Step = 10, Order = 802)]
    public int VanillaPowerCellCapacity = 100;

    [Slider("  Reinforced Battery capacity", 100, 500, DefaultValue = 250, Step = 10, Order = 803)]
    public int ReinforcedBatteryCapacity = 250;

    [Slider("  Reinforced Power Cell capacity", 200, 1000, DefaultValue = 500, Step = 20, Order = 804)]
    public int ReinforcedPowerCellCapacity = 500;

    [Slider("  Hyper Battery capacity", 1000, 3000, DefaultValue = 1500, Step = 50, Order = 805)]
    public int HyperBatteryCapacity = 1500;

    [Slider("  Hyper Power Cell capacity", 2000, 6000, DefaultValue = 3000, Step = 100, Order = 806)]
    public int HyperPowerCellCapacity = 3000;

    [Toggle("  Migrate old saves (clamp charge)", Order = 807)]
    public bool BatteryMigrateOldSaves = true;

    // =====================================================================
    // Teleport Beacon
    // =====================================================================

    [Toggle("Enable Teleport Beacon", Order = 900)]
    public bool TeleportBeaconEnabled = true;

    [Slider("  Source base cost (J)", 0, 5000, DefaultValue = 100, Step = 25, Order = 901)]
    public int TeleportSourceCostJoules = 100;

    [Slider("  Target base cost (J)", 0, 5000, DefaultValue = 100, Step = 25, Order = 902)]
    public int TeleportTargetCostJoules = 100;

    [Slider("  Min power on both bases (%)", 0, 100, DefaultValue = 20, Step = 5, Order = 903)]
    public int TeleportMinBasePowerPercent = 20;

    [Slider("  Cooldown (s)", 0, 300, DefaultValue = 15, Step = 5, Order = 904)]
    public int TeleportCooldownSeconds = 15;

    [Slider("  Energy cost per 100m (J)", 0, 500, DefaultValue = 20, Step = 5, Order = 905)]
    public int TeleportCostPerHundredMeters = 20;

    [Slider("  Efficiency chip MK1 cost (%)", 10, 100, DefaultValue = 75, Step = 5, Order = 906)]
    public int TeleportEfficiencyMK1Percent = 75;

    [Slider("  Efficiency chip MK2 cost (%)", 10, 100, DefaultValue = 50, Step = 5, Order = 907)]
    public int TeleportEfficiencyMK2Percent = 50;

    [Slider("  Efficiency chip MK3 cost (%)", 5, 100, DefaultValue = 25, Step = 5, Order = 908)]
    public int TeleportEfficiencyMK3Percent = 25;

    public float GetEfficiencyMultiplier(int tier) => tier switch
    {
        1 => TeleportEfficiencyMK1Percent / 100f,
        2 => TeleportEfficiencyMK2Percent / 100f,
        3 => TeleportEfficiencyMK3Percent / 100f,
        _ => 1f,
    };

    // =====================================================================
    // Singleton
    // =====================================================================

    public static InferiusConfig Instance { get; } = OptionsPanelHandler.RegisterModOptions<InferiusConfig>();

    // Nautilus attribute-based ConfigFile nevola On<Field>Changed handlery
    // (konvence imperative API). Runtime reakci na zmeny v Options menu resi
    // Harmony patch ConfigSavePatch, ktery hook-ne na ConfigFile.Save a po nem
    // zavola feature.ApplyRuntime metody.
}
