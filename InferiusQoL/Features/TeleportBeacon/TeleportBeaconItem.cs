namespace InferiusQoL.Features.TeleportBeacon;

using System.Collections.Generic;
using InferiusQoL.Logging;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;

/// <summary>
/// Teleport Beacon - buildable interior piece klonovany z vanilla StarshipSouvenir
/// (minimodel Aurory). Buildable v Habitat Builder, stacionarni po umisteni.
/// Klik na beacon spusti teleport na nejblizsi jiny beacon.
/// </summary>
public static class TeleportBeaconItem
{
    public const string ClassId = "InferiusTeleportBeacon";
    public static TechType TechType { get; private set; } = TechType.None;

    public static void Register()
    {
        var info = PrefabInfo.WithTechType(
            ClassId,
            "Teleport Beacon",
            "Stationary teleport device. Build 2+ across your bases to teleport between them.");

        info.WithIcon(SpriteManager.Get(TechType.Beacon));

        var prefab = new CustomPrefab(info);

        // StarshipSouvenir v teto verzi Subnauticy neni buildable (je jen pickupable
        // decoration). Klonujeme Bench - spolehlivy buildable interior piece.
        // Custom model (Aurora mini) by vyzadoval asset bundle, zvazeno pozdeji.
        var cloneTemplate = new CloneTemplate(info, TechType.Bench)
        {
            ModifyPrefab = (obj) =>
            {
                // Odstrante vanilla Bench komponentu (sit interakce), aby si nase
                // TeleportBeaconBehavior mohla prevzit IHandTarget hook.
                var bench = obj.GetComponent<Bench>();
                if (bench != null)
                    Object.DestroyImmediate(bench);

                // Odstranit sit-related objekty (trigger collider pro sit, sit points).
                // Pokud existuje child "SitPoint", odstranit.
                var sitPoint = obj.transform.Find("SitPoint");
                if (sitPoint != null)
                    Object.DestroyImmediate(sitPoint.gameObject);

                if (obj.GetComponent<TeleportBeaconBehavior>() == null)
                    obj.AddComponent<TeleportBeaconBehavior>();

                // Prepsat TechTag + Constructable techType na nas custom (clone zdedil vanilla).
                var techTag = obj.GetComponent<TechTag>();
                if (techTag != null)
                    techTag.type = info.TechType;

                var constructable = obj.GetComponent<Constructable>();
                if (constructable != null)
                {
                    constructable.techType = info.TechType;
                    constructable.allowedOnGround = true;
                    constructable.allowedInBase = true;
                    constructable.allowedInSub = true;
                    constructable.allowedOutside = false;
                    constructable.allowedOnConstructables = true;
                    constructable.allowedOnWall = false;
                    constructable.allowedOnCeiling = false;
                    constructable.rotationEnabled = true;
                    constructable.deconstructionAllowed = true;
                    constructable.forceUpright = true;
                }

                QoLLog.Info(Category.Teleport,
                    $"ModifyPrefab TeleportBeacon: Constructable {(constructable != null ? "set" : "MISSING")}, TechTag {(techTag != null ? "set" : "MISSING")}");
            },
        };
        prefab.SetGameObject(cloneTemplate);

        prefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);

        prefab.SetRecipe(new RecipeData
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.AdvancedWiringKit, 1),
                new Ingredient(TechType.Kyanite, 1),
                new Ingredient(TechType.Polyaniline, 1),
                new Ingredient(TechType.Aerogel, 1),
            }
        })
        .WithFabricatorType(CraftTree.Type.Constructor)
        .WithCraftingTime(10f);

        prefab.Register();
        TechType = info.TechType;

        // Odemknuti od startu (bez Analysis).
        KnownTechHandler.UnlockOnStart(TechType);

        QoLLog.Info(Category.Teleport, $"Registered Teleport Beacon as {TechType} (unlocked on start)");
    }
}
