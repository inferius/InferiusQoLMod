namespace InferiusQoL.Features.Compressor;

using HarmonyLib;
using InferiusQoL.Config;
using UnityEngine;

/// <summary>
/// Per-instance komprese pres 4 synchronizovane patches:
///
/// 1. InventoryItem(Pickupable) constructor postfix - pokud instance ma marker,
///    nastavi _width=1, _height=1 (per-instance layout marker).
///
/// 2. ItemsContainer.AddItem prefix/postfix - nastavi ThreadStatic kontext na
///    Pickupable. Tim behem container layout decisions (free space search,
///    grid placement) TechData.GetItemSize vrati 1x1 pokud je markered.
///
/// 3. uGUI_ItemsContainer.OnAddItem prefix/postfix - nastavi kontext na
///    InventoryItem pro UI sprite rendering.
///
/// 4. TechData.GetItemSize postfix - cte kontext a vrati 1x1 pokud markered
///    instance. Pokud neni kontext, necha vanilla.
/// </summary>
internal static class CompressorRenderContext
{
    [System.ThreadStatic]
    private static InventoryItem? _currentInventoryItem;

    [System.ThreadStatic]
    private static Pickupable? _currentPickupable;

    public static InventoryItem? CurrentInventoryItem
    {
        get => _currentInventoryItem;
        set => _currentInventoryItem = value;
    }

    public static Pickupable? CurrentPickupable
    {
        get => _currentPickupable;
        set => _currentPickupable = value;
    }

    /// <summary>Vrati aktuální Pickupable v kontextu (primarne pres CurrentPickupable, fallback InventoryItem.item).</summary>
    public static Pickupable? EffectivePickupable =>
        _currentPickupable ?? _currentInventoryItem?.item;
}

[HarmonyPatch(typeof(InventoryItem), MethodType.Constructor, new[] { typeof(Pickupable) })]
public static class InventoryItem_Constructor_Patch
{
    [HarmonyPostfix]
    public static void Postfix(InventoryItem __instance, Pickupable pickupable)
    {
        if (__instance == null || pickupable == null) return;

        var cfg = InferiusConfig.Instance;
        if (!cfg.CompressorEnabled) return;

        var uid = pickupable.GetComponent<UniqueIdentifier>();
        if (uid == null || string.IsNullOrEmpty(uid.Id)) return;
        if (!CompressorSaveManager.IsInstanceCompressed(uid.Id)) return;

        // Self-cleanup: pokud TT je nyni v blacklistu (napr. user pridal nase
        // custom items az dodatecne), smazeme marker a vratime vanilla velikost.
        var tt = pickupable.GetTechType();
        if (CompressorBlacklist.IsBlacklisted(tt))
        {
            CompressorSaveManager.Remove(uid.Id);
            return;
        }

        __instance._width = 1;
        __instance._height = 1;
    }
}

[HarmonyPatch(typeof(ItemsContainer), nameof(ItemsContainer.AddItem), new[] { typeof(Pickupable) })]
public static class ItemsContainer_AddItem_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Pickupable pickupable)
    {
        // Nestartuj kontext pokud je blacklisted - vanilla velikost je spravna.
        if (pickupable != null && CompressorBlacklist.IsBlacklisted(pickupable.GetTechType()))
        {
            CompressorRenderContext.CurrentPickupable = null;
            return;
        }
        CompressorRenderContext.CurrentPickupable = pickupable;
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        CompressorRenderContext.CurrentPickupable = null;
    }
}

[HarmonyPatch(typeof(uGUI_ItemsContainer), "OnAddItem")]
public static class uGUI_ItemsContainer_OnAddItem_Patch
{
    [HarmonyPrefix]
    public static void Prefix(InventoryItem item)
    {
        CompressorRenderContext.CurrentInventoryItem = item;
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        CompressorRenderContext.CurrentInventoryItem = null;
    }
}

[HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize), new[] { typeof(TechType) })]
public static class TechData_GetItemSize_Patch
{
    [HarmonyPostfix]
    public static void Postfix(TechType techType, ref Vector2int __result)
    {
        var cfg = InferiusConfig.Instance;
        if (!cfg.CompressorEnabled) return;
        if (__result.x <= 1 && __result.y <= 1) return;

        var pickupable = CompressorRenderContext.EffectivePickupable;
        if (pickupable == null) return;

        // Sanity: kontext techtype musi odpovidat volanemu.
        if (pickupable.GetTechType() != techType) return;

        var uid = pickupable.GetComponent<UniqueIdentifier>();
        if (uid == null || string.IsNullOrEmpty(uid.Id)) return;
        if (!CompressorSaveManager.IsInstanceCompressed(uid.Id)) return;

        __result = new Vector2int(1, 1);
    }
}
