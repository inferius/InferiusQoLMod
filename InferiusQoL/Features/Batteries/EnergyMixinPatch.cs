namespace InferiusQoL.Features.Batteries;

using System.Collections.Generic;
using HarmonyLib;
using InferiusQoL.Assets;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Injektuje custom battery/power cell TechTypes do compatibleBatteries listu
/// kazdeho EnergyMixin pri jeho Awake. Bez tohoto patche tools/vehicles nase
/// custom tiery neakceptuji ("nejdou nasadit").
///
/// Navic klonuje vanilla 3D model a swapne main texture za nasi PNG, aby
/// vehicle kompartment ukazoval spravnou nalepku misto genericke Battery.
///
/// Inspirovano PrimeSonic CustomBatteries (EnergyMixinPatcher.cs).
/// </summary>
[HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.Awake))]
public static class EnergyMixin_Awake_Patch
{
    // Mapping custom TechType -> (sample vanilla TT pro klonovani modelu, PNG filename).
    private static readonly Dictionary<TechType, (TechType cloneFrom, string texture)> CustomModelMap = new();

    public static void RegisterModelSource(TechType custom, TechType cloneFrom, string texture)
    {
        CustomModelMap[custom] = (cloneFrom, texture);
    }

    [HarmonyPrefix]
    public static void Prefix(EnergyMixin __instance)
    {
        if (!__instance.allowBatteryReplacement) return;
        if (__instance.compatibleBatteries == null) return;

        var compat = __instance.compatibleBatteries;

        bool acceptsBatteries = compat.Contains(TechType.Battery)
                             || compat.Contains(TechType.PrecursorIonBattery);

        if (acceptsBatteries)
        {
            foreach (var tt in BatteryItems.CustomBatteryTypes)
                if (tt != TechType.None && !compat.Contains(tt)) compat.Add(tt);

            InjectModels(__instance, BatteryItems.CustomBatteryTypes);
        }

        bool acceptsPowerCells = compat.Contains(TechType.PowerCell)
                              || compat.Contains(TechType.PrecursorIonPowerCell);

        if (acceptsPowerCells)
        {
            foreach (var tt in BatteryItems.CustomPowerCellTypes)
                if (tt != TechType.None && !compat.Contains(tt)) compat.Add(tt);

            InjectModels(__instance, BatteryItems.CustomPowerCellTypes);
        }
    }

    private static void InjectModels(EnergyMixin mixin, List<TechType> customTypes)
    {
        if (customTypes.Count == 0) return;

        var models = new List<EnergyMixin.BatteryModels>(mixin.batteryModels ?? new EnergyMixin.BatteryModels[0]);
        var existing = new HashSet<TechType>();
        foreach (var m in models) existing.Add(m.techType);

        foreach (var tt in customTypes)
        {
            if (tt == TechType.None || existing.Contains(tt)) continue;
            if (!CustomModelMap.TryGetValue(tt, out var info)) continue;

            // Najdi vanilla model (BatteryModels entry s techType == cloneFrom).
            GameObject? sourceModel = null;
            foreach (var m in models)
            {
                if (m.techType == info.cloneFrom) { sourceModel = m.model; break; }
            }
            if (sourceModel == null) continue;

            // Klonuj do same parent.
            var clone = Object.Instantiate(sourceModel, sourceModel.transform.parent);
            clone.name = tt.AsString() + "_model";
            clone.SetActive(false);

            // Swap texturu.
            var tex = IconLoader.LoadTexture(info.texture);
            if (tex != null)
            {
                var renderer = clone.GetComponentInChildren<Renderer>();
                if (renderer != null && renderer.material != null)
                    renderer.material.SetTexture(ShaderPropertyID._MainTex, tex);
            }

            models.Add(new EnergyMixin.BatteryModels { model = clone, techType = tt });
        }

        mixin.batteryModels = models.ToArray();
    }
}

[HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery))]
public static class EnergyMixin_NotifyHasBattery_Patch
{
    [HarmonyPostfix]
    public static void Postfix(EnergyMixin __instance, InventoryItem item)
    {
        var tt = item?.item?.GetTechType() ?? TechType.None;
        if (tt == TechType.None) return;
        if (!BatteryItems.CustomBatteryTypes.Contains(tt)
            && !BatteryItems.CustomPowerCellTypes.Contains(tt)) return;
        if (__instance.batteryModels == null) return;

        // Aktivuj spravny model, deaktivuj ostatni.
        for (int i = 0; i < __instance.batteryModels.Length; i++)
        {
            var m = __instance.batteryModels[i];
            if (m.model != null)
                m.model.SetActive(m.techType == tt);
        }
    }
}
