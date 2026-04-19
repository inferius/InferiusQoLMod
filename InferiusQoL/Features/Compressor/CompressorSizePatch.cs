namespace InferiusQoL.Features.Compressor;

using System.Collections.Generic;
using HarmonyLib;
using InferiusQoL.Config;
using UnityEngine;

/// <summary>
/// Postfix patch na TechData.GetItemSize. Kdyz hrac ma osazeny Compressor chip,
/// vrati 1x1 pro vsechny TechTypes mimo CompressorBlacklist. Efekt je globalni
/// (plati i pro storage kontejnery, Cyclops, atd.).
///
/// Taky udrzuje cache originalnich velikosti - pouziva CompressorRuntimeRefreshPatch
/// pro filtrovani items ktere realne potrebuji refresh.
/// </summary>
[HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize), new[] { typeof(TechType) })]
public static class TechData_GetItemSize_Patch
{
    /// <summary>Cache vanilla velikosti - key TechType, value puvodni Vector2int pred modifikaci.</summary>
    private static readonly Dictionary<TechType, Vector2int> _vanillaSizes = new Dictionary<TechType, Vector2int>();

    [HarmonyPostfix]
    public static void Postfix(TechType techType, ref Vector2int __result)
    {
        // Cache vanilla velikost pri prvnim volani (pred modifikaci).
        if (!_vanillaSizes.ContainsKey(techType))
            _vanillaSizes[techType] = __result;

        var cfg = InferiusConfig.Instance;
        if (!cfg.CompressorEnabled) return;
        if (CompressorItem.TechType == TechType.None) return;
        if (!CompressorItem.IsEquipped()) return;
        if (CompressorBlacklist.IsBlacklisted(techType)) return;

        // Pokud je uz 1x1 nebo mensi, necham.
        if (__result.x <= 1 && __result.y <= 1) return;

        __result = new Vector2int(1, 1);
    }

    /// <summary>Vrati vanilla (pre-patch) velikost item z cache. Fallback na direct
    /// zavolani pokud cache neobsahuje (ale to zaroven cache naplni).</summary>
    public static Vector2int GetVanillaSize(TechType techType)
    {
        if (_vanillaSizes.TryGetValue(techType, out var size))
            return size;
        // Trigger patch -> cache fill. Ale to vrati uz potencialne modified hodnotu
        // pokud chip je equipped. Naplnime rucne pres temp flag.
        var raw = TechData.GetItemSize(techType);
        // Pokud jsme v equipped mode a vrati 1x1, nemuzeme tim naplnit cache spravne.
        // Proto pro uncached items v equipped mode mame fallback = raw (ktery je 1x1
        // pokud !blacklisted). Uncached items by mely byt vzacne po prvnim pruchodu.
        return _vanillaSizes.TryGetValue(techType, out size) ? size : raw;
    }
}
