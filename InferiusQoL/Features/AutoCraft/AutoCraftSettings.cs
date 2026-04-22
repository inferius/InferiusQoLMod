namespace InferiusQoL.Features.AutoCraft;

using System.Collections.Generic;
using InferiusQoL.Config;

public enum NeighboringStorage { Off, Inside, Range100 }
public enum ReturnSurplus { Inventory, Lockers }

/// <summary>
/// Facade nad InferiusConfig pro AutoCraft. Prevadi string volby z Choice
/// dropdown na enumy + drzi globalni seznam disabled auto-craft recipes.
/// </summary>
internal static class AutoCraftSettings
{
    public static readonly HashSet<TechType> DisabledAutoCraftRecipes = new HashSet<TechType>();

    public static bool AutoCraft => InferiusConfig.Instance.AutoCraftEnabled;

    public static NeighboringStorage UseStorage
    {
        get
        {
            var v = InferiusConfig.Instance.AutoCraftUseStorage;
            if (string.Equals(v, "Off", System.StringComparison.OrdinalIgnoreCase)) return NeighboringStorage.Off;
            if (v != null && v.StartsWith("Inside", System.StringComparison.OrdinalIgnoreCase)) return NeighboringStorage.Inside;
            return NeighboringStorage.Range100;
        }
    }

    public static float RangeMeters => InferiusConfig.Instance.AutoCraftRangeMeters;
    public static float RangeMetersSquared => RangeMeters * RangeMeters;

    public static ReturnSurplus ReturnSurplus
    {
        get
        {
            var v = InferiusConfig.Instance.AutoCraftReturnSurplus;
            if (string.Equals(v, "Lockers", System.StringComparison.OrdinalIgnoreCase)) return ReturnSurplus.Lockers;
            return ReturnSurplus.Inventory;
        }
    }

    public static bool BetterTooltips => InferiusConfig.Instance.AutoCraftBetterTooltips;

    public static float SpeedMultiplier =>
        System.Math.Max(1, InferiusConfig.Instance.AutoCraftSpeedPercent) / 100f;

    /// <summary>Vrati nasobitel pro batch craft dle stisknuteho modifieru.</summary>
    public static int GetBatchMultiplier()
    {
        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl)
            || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightControl))
            return System.Math.Max(1, InferiusConfig.Instance.AutoCraftCtrlMultiplier);
        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift)
            || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift))
            return System.Math.Max(1, InferiusConfig.Instance.AutoCraftShiftMultiplier);
        return 1;
    }
}
