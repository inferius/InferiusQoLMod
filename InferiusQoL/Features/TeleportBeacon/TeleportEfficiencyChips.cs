namespace InferiusQoL.Features.TeleportBeacon;

using System.Collections.Generic;
using InferiusQoL.Logging;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;

/// <summary>
/// 3 efficiency chipy pro Teleport Beacon. Hrac je vyrobi ve Modification Station,
/// v UI beaconu je osadi - snizi energy cost teleportu. Multiplikator v configu
/// (default 75%/50%/25%).
/// </summary>
public static class TeleportEfficiencyChips
{
    public static TechType MK1 { get; private set; } = TechType.None;
    public static TechType MK2 { get; private set; } = TechType.None;
    public static TechType MK3 { get; private set; } = TechType.None;

    public static void Register()
    {
        MK1 = RegisterChip(
            classId: "InferiusTeleportEfficiencyMK1",
            displayName: "Teleport Efficiency Chip MK1",
            description: "Reduces teleport energy cost when installed in a Teleport Beacon (default 75% of original).",
            unlockAfter: TechType.AdvancedWiringKit,
            recipe: new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.Silicone, 1),
                }
            });

        MK2 = RegisterChip(
            classId: "InferiusTeleportEfficiencyMK2",
            displayName: "Teleport Efficiency Chip MK2",
            description: "Reduces teleport energy cost more (default 50%). Consumes MK1 in recipe.",
            unlockAfter: MK1,
            recipe: new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(MK1, 1),
                    new Ingredient(TechType.Kyanite, 1),
                    new Ingredient(TechType.Aerogel, 1),
                }
            });

        MK3 = RegisterChip(
            classId: "InferiusTeleportEfficiencyMK3",
            displayName: "Teleport Efficiency Chip MK3",
            description: "Maximum teleport efficiency (default 25% of original cost). Consumes MK2 in recipe.",
            unlockAfter: MK2,
            recipe: new RecipeData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(MK2, 1),
                    new Ingredient(TechType.Polyaniline, 1),
                    new Ingredient(TechType.Kyanite, 1),
                }
            });

        KnownTechHandler.UnlockOnStart(MK1);

        QoLLog.Info(Category.Teleport, $"Registered Teleport Efficiency chips: MK1={MK1}, MK2={MK2}, MK3={MK3}");
    }

    private static TechType RegisterChip(
        string classId,
        string displayName,
        string description,
        TechType unlockAfter,
        RecipeData recipe)
    {
        var info = PrefabInfo.WithTechType(classId, displayName, description);
        info.WithIcon(SpriteManager.Get(TechType.Compass));
        info.WithSizeInInventory(new Vector2int(1, 1));

        var prefab = new CustomPrefab(info);
        prefab.SetGameObject(new CloneTemplate(info, TechType.Compass));
        prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
        prefab.SetUnlock(unlockAfter);

        prefab.SetRecipe(recipe)
            .WithFabricatorType(CraftTree.Type.Workbench)
            .WithStepsToFabricatorTab("CompressorMenu") // reuse compressor tab for now
            .WithCraftingTime(5f);

        prefab.Register();
        return info.TechType;
    }

    /// <summary>Najde nejvyssi MK chip v Player inventari. Vraci TechType.None pokud nic.</summary>
    public static (int tier, TechType techType) FindHighestInInventory()
    {
        var inv = Inventory.main;
        if (inv?.container == null) return (0, TechType.None);

        if (MK3 != TechType.None && inv.container.GetCount(MK3) > 0) return (3, MK3);
        if (MK2 != TechType.None && inv.container.GetCount(MK2) > 0) return (2, MK2);
        if (MK1 != TechType.None && inv.container.GetCount(MK1) > 0) return (1, MK1);
        return (0, TechType.None);
    }

    public static TechType GetTechTypeForTier(int tier) => tier switch
    {
        1 => MK1,
        2 => MK2,
        3 => MK3,
        _ => TechType.None,
    };
}
