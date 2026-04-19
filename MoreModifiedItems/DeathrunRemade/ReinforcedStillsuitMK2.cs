namespace MoreModifiedItems.DeathrunRemade;

using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;
using static CraftData;
using Nautilus.Assets.Gadgets;
using MoreModifiedItems.BasicEquipment;
using MoreModifiedItems.Patchers;

internal static class ReinforcedStillsuitMK2
{
    internal static CustomPrefab Instance { get; set; }

    internal static void CreateAndRegister()
    {
        if (!DeathrunCompat.DeathrunLoaded() || !DeathrunCompat.VersionCheck())
            return;

        if (!TechTypeExtensions.FromString("deathrunremade_reinforcedsuit2", out TechType reinforcedsuit2, true) | 
            !TechTypeExtensions.FromString("deathrunremade_reinforcedfiltrationsuit", out TechType reinforcedstillsuit, true) |
            !TechTypeExtensions.FromString("deathrunremade_spineeelscale", out TechType spineeelscale, true))
        {
            Plugin.Log.LogError($"Failed to load Reinforced Stillsuit MKII - {reinforcedsuit2}, {reinforcedstillsuit}, {spineeelscale}");
            return;
        }

        Instance = new CustomPrefab("rssuitmk2", "Reinforced Enhanced Water Filtration Suit Mk2",
            "An upgraded dive suit capable of protecting the user at depths up to 1300m and providing heat protection up to 75C while also containing the water recycling feature of the Enhanced Water Filtration Suit",
            SpriteManager.Get(reinforcedsuit2));

        Instance.Info.WithSizeInInventory(new Vector2int(2, 3));
        Instance.SetEquipment(EquipmentType.Body);

        Instance.SetRecipe(new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>()
            {
                new Ingredient(ReinforcedEnhancedStillsuit.Instance.Info.TechType, 1),
                new Ingredient(TechType.AramidFibers, 1),
                new Ingredient(TechType.AluminumOxide, 2),
                new Ingredient(spineeelscale, 2)
            }
        }).WithCraftingTime(5f).WithFabricatorType(CraftTree.Type.Workbench).WithStepsToFabricatorTab("BodyMenu".Split('/'));

        if (GetBuilderIndex(ReinforcedEnhancedStillsuit.Instance.Info.TechType, out var group, out var category, out _))
            Instance.SetPdaGroupCategoryAfter(group, category, ReinforcedEnhancedStillsuit.Instance.Info.TechType);

        Instance.SetUnlock(reinforcedsuit2).WithAnalysisTech(null);

        var cloneStillsuit = new CloneTemplate(Instance.Info, TechType.WaterFiltrationSuit)
        {
            ModifyPrefab = (obj) =>
            {
                obj.EnsureComponent<ESSBehaviour>();
                obj.SetActive(false);
            }
        };

        Instance.SetGameObject(cloneStillsuit);

        Instance.Register();
        EquipmentPatcher.OverrideMap.Add(Instance.Info.TechType, reinforcedsuit2);
        DeathrunCompat.AddSuitCrushDepthMethod(Instance.Info.TechType, new float[] { 1300f });
        DeathrunCompat.AddNitrogenModifierMethod(Instance.Info.TechType, new float[] { 0.25f, 0.2f });

        Plugin.Log.LogDebug("Reinforced Stillsuit MKII registered");
    }
}
