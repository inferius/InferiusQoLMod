namespace InferiusQoL.Console;

using System.Text;
using InferiusQoL.Config;
using InferiusQoL.Localization;
using InferiusQoL.Logging;
using Nautilus.Commands;
using Nautilus.Handlers;

public static class ConsoleCommands
{
    public static void Register()
    {
        ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));
    }

    [ConsoleCommand("qol_apply")]
    public static string QolApply()
    {
        InferiusQoL.Features.InventoryResize.InventoryResizePatch.ApplyRuntime();
        return "Applied runtime config. Note: some changes still require restart (locker resize, batteries, custom items).";
    }

    [ConsoleCommand("qol_status")]
    public static string QolStatus()
    {
        var c = InferiusConfig.Instance;
        var sb = new StringBuilder();
        sb.AppendLine(L.Get("InferiusQoL.Status.Header", MyPluginInfo.PLUGIN_VERSION));
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.Verbosity", QoLLog.CurrentVerbosity)}");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.CustomizedStorage", Plugin.HasCustomizedStorage)}");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.AdvancedInventory", Plugin.HasAdvancedInventory)}");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.BagEquipment", Plugin.HasBagEquipment)}");
        sb.AppendLine(L.Get("InferiusQoL.Status.Features"));
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.InventoryResize"),-18} {OnOff(c.InventoryResizeEnabled)} (+{c.InventoryExtraRows}R/+{c.InventoryExtraCols}C)");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.LockerResize"),-18} {OnOff(c.LockerResizeEnabled)} ({c.LockerWidth}x{c.LockerHeight}, wall {c.WallLockerWidth}x{c.WallLockerHeight})");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.Backpacks"),-18} {OnOff(c.BackpacksEnabled)} (S/M/L = {c.BackpackSmallCapacity}/{c.BackpackMediumCapacity}/{c.BackpackLargeCapacity})");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.SeamothTurbo"),-18} {OnOff(c.SeamothTurboEnabled)} ({c.SeamothTurboSpeedMultiplier:0.0}x spd, {c.SeamothTurboEnergyMultiplier:0.00}x drain)");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.Retriever"),-18} {OnOff(c.RetrieverEnabled)} ({c.RetrieverActionCostJoules} J/item, min {c.RetrieverMinBasePowerPercent}% power)");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.Compressor"),-18} {OnOff(c.CompressorEnabled)} (energy: {(c.CompressorRequiresEnergy ? c.CompressorEnergyCost + " J" : "off")})");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.TankWelder"),-18} {OnOff(c.TankWelderEnabled)} (T1 {c.TankWelderT1Multiplier:0.0}x / T2 {c.TankWelderT2Multiplier:0.0}x / T3 {c.TankWelderT3Multiplier:0.0}x)");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.BatteryRework"),-18} {OnOff(c.BatteryReworkEnabled)} (B{c.VanillaBatteryCapacity}/PC{c.VanillaPowerCellCapacity}/RB{c.ReinforcedBatteryCapacity}/RPC{c.ReinforcedPowerCellCapacity}/HB{c.HyperBatteryCapacity}/HPC{c.HyperPowerCellCapacity})");
        sb.AppendLine($"  {L.Get("InferiusQoL.Status.TeleportBeacon"),-18} {OnOff(c.TeleportBeaconEnabled)} ({c.TeleportSourceCostJoules}+{c.TeleportTargetCostJoules} J, cd {c.TeleportCooldownSeconds}s, min {c.TeleportMinBasePowerPercent}%)");
        return sb.ToString();
    }

    [ConsoleCommand("qol_log_level")]
    public static string QolLogLevel(string level = "")
    {
        if (string.IsNullOrEmpty(level))
            return L.Get("InferiusQoL.Console.LogLevelCurrent", QoLLog.CurrentVerbosity);

        QoLLog.SetVerbosity(level);
        return L.Get("InferiusQoL.Console.LogLevelSet", QoLLog.CurrentVerbosity);
    }

    [ConsoleCommand("qol_retriever_rescan")]
    public static string QolRetrieverRescan() => L.Get("InferiusQoL.Console.NotImplemented", "retriever");

    [ConsoleCommand("qol_retriever_dump")]
    public static string QolRetrieverDump() => L.Get("InferiusQoL.Console.NotImplemented", "retriever");

    [ConsoleCommand("qol_seamoth_boost_state")]
    public static string QolSeamothBoostState() => L.Get("InferiusQoL.Console.NotImplemented", "seamoth_turbo");

    [ConsoleCommand("qol_teleport_list")]
    public static string QolTeleportList() => L.Get("InferiusQoL.Console.NotImplemented", "teleport_beacon");

    [ConsoleCommand("qol_migrate_batteries")]
    public static string QolMigrateBatteries() => L.Get("InferiusQoL.Console.NotImplemented", "battery_rework");

    private static string OnOff(bool v) => v ? L.Get("InferiusQoL.On") : L.Get("InferiusQoL.Off");
}
