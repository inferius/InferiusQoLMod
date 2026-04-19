namespace InferiusQoL.Features.SeamothTurbo;

using System.Collections.Generic;
using InferiusQoL.Logging;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;

/// <summary>
/// Custom TechType registrace pro Seamoth Turbo upgrade modul.
/// Ikona je zatim placeholder z vanilla modulu, pozdeji ji nahradime vlastni PNG.
/// </summary>
public static class SeamothTurboItem
{
    public const string ClassId = "InferiusSeamothTurbo";

    public static TechType TechType { get; private set; } = TechType.None;

    public static void Register()
    {
        var info = PrefabInfo.WithTechType(
            classId: ClassId,
            displayName: "Seamoth Turbo Module",
            description: "Boosts Seamoth speed while sprinting at the cost of increased energy consumption. Configurable in mod options.");

        // Placeholder icon: reuse vanilla Seamoth Solar Charge sprite. Replace later.
        info.WithIcon(SpriteManager.Get(TechType.SeamothSolarCharge));

        var prefab = new CustomPrefab(info);

        // Model: klonujeme vanilla Seamoth Solar Charge modul (ma tvar modulove karty).
        prefab.SetGameObject(new CloneTemplate(info, TechType.SeamothSolarCharge));

        // PDA kategorie - vehicle upgrades.
        prefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

        // Odemka: hned jak ma hrac Seamoth. Crafting limitovan materialy.
        prefab.SetUnlock(TechType.Seamoth);

        // Recipe: mid-game (Magnetite z Jellyshroom Caves / Mountain Island).
        prefab.SetRecipe(new RecipeData
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.Copper, 1),
                new Ingredient(TechType.Magnetite, 2),
                new Ingredient(TechType.Lubricant, 1),
            }
        })
        .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
        .WithStepsToFabricatorTab("SeamothModules")
        .WithCraftingTime(5f);

        // Vehicle module equipment slot, passive (ne-selectable).
        prefab.SetEquipment(EquipmentType.VehicleModule)
            .WithQuickSlotType(QuickSlotType.Passive);

        prefab.Register();
        TechType = info.TechType;

        QoLLog.Info(Category.Seamoth, $"Registered Seamoth Turbo module as TechType {TechType}");
    }
}
