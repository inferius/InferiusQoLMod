namespace InferiusQoL.Features.TankWelder;

using HarmonyLib;
using InferiusQoL.Logging;

/// <summary>
/// Postfix patch na UnderwaterMotor.AlterMaxSpeed. Kdyz ma hrac osazeny T4 merged tank
/// (lightweight varianta), pridame zpet vanilla Plasteel speed penalty ke kompenzaci -
/// efektivne zrusime speed reduction od osazene lahve.
/// </summary>
[HarmonyPatch(typeof(UnderwaterMotor), nameof(UnderwaterMotor.AlterMaxSpeed))]
public static class UnderwaterMotor_AlterMaxSpeed_Patch
{
    /// <summary>Vanilla PlasteelTank speed penalty (m/s). Odhad - upresnuje se testem.</summary>
    private const float PlasteelTankSpeedPenalty = 0.45f;

    [HarmonyPostfix]
    public static void Postfix(UnderwaterMotor __instance, ref float __result)
    {
        var inv = Inventory.main;
        if (inv?.equipment == null) return;

        if (TankWelderItems.MergedTankT4 != TechType.None
            && inv.equipment.GetCount(TankWelderItems.MergedTankT4) > 0)
        {
            __result += PlasteelTankSpeedPenalty;
        }
    }
}
