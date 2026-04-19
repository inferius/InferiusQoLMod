namespace InferiusQoL.Features.SeamothTurbo;

using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Turbo boost pro Seamoth (3 tiery MK1/MK2/MK3). Postfix patch na SeaMoth.Update.
///
/// - Velocity cap: pridane force jen do target speed, nikdy nad nim.
/// - Surface falloff: boost se plynule ztlumuje, jak se Seamoth blizi k hladine
///   (nad hladinou 0%, od SurfaceFalloffMeters a hloub 100%). Tim se
///   elegantne resi problem se skoky z vody.
/// - Vehicle Power Upgrade Module discount: spotreba se snizuje pokud je osazeny
///   vanilla modul pro snizeni spotreby (0.5^pocet).
/// - Auto-detekce nejvyssiho tieru (MK3 > MK2 > MK1), ne-stackuje se.
/// </summary>
[HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
public static class SeaMoth_Update_Patch
{
    private const float SeamothBaseDrainPerSecond = 0.15f;

    private static TurboTier _loggedTier = TurboTier.None;

    [HarmonyPostfix]
    public static void Postfix(SeaMoth __instance)
    {
        var tier = ShouldBoost(__instance);
        if (tier == TurboTier.None) return;
        ApplyBoost(__instance, tier);
    }

    private static TurboTier ShouldBoost(SeaMoth seamoth)
    {
        if (seamoth == null) return TurboTier.None;

        var cfg = InferiusConfig.Instance;
        if (!cfg.SeamothTurboEnabled) return TurboTier.None;

        var player = Player.main;
        if (player == null) return TurboTier.None;
        if (player.GetVehicle() != seamoth) return TurboTier.None;

        var tier = SeamothTurboItems.GetEquippedTier(seamoth);
        if (tier == TurboTier.None) return TurboTier.None;

        if (!GameInput.GetButtonHeld(GameInput.Button.Sprint)) return TurboTier.None;

        if (!seamoth.HasEnoughEnergy(0.01f)) return TurboTier.None;

        return tier;
    }

    private static void ApplyBoost(SeaMoth seamoth, TurboTier tier)
    {
        var cfg = InferiusConfig.Instance;
        var (speedMult, energyMult) = SeamothTurboItems.GetTierValues(tier, cfg);

        // Surface falloff: smooth snizeni boostu k hladine.
        float falloff = GetDepthFalloff(seamoth, cfg.SeamothTurboSurfaceFalloffMeters);
        if (falloff <= 0f) return; // nad hladinou, zadny boost

        var speedBonus = (speedMult - 1f) * falloff;
        var energyBonus = (energyMult - 1f) * falloff;

        var rb = seamoth.useRigidbody;
        if (rb != null && speedBonus > 0f)
        {
            Vector3 forward = seamoth.transform.forward;
            float currentForwardSpeed = Vector3.Dot(rb.velocity, forward);
            float targetMaxSpeed = seamoth.forwardForce * (1f + speedBonus);

            if (currentForwardSpeed < targetMaxSpeed)
            {
                float deltaNeeded = targetMaxSpeed - currentForwardSpeed;
                float dt = Time.deltaTime;
                float maxStep = seamoth.forwardForce * speedBonus * dt;
                float stepForce = Mathf.Min(maxStep, deltaNeeded);

                rb.velocity += forward * stepForce;

                if (_loggedTier != tier)
                {
                    _loggedTier = tier;
                    QoLLog.Info(Category.Seamoth,
                        $"Turbo {tier} active: base {speedMult:0.0}x/{energyMult:0.0}x, falloff {falloff:P0}, target {targetMaxSpeed:0.0} m/s");
                }
            }
        }

        // Extra energy drain s PowerUpgradeModule discount.
        if (energyBonus > 0f)
        {
            float powerMult = GetPowerUpgradeMultiplier(seamoth);
            var extra = energyBonus * SeamothBaseDrainPerSecond * Time.deltaTime * powerMult;
            seamoth.energyInterface?.ConsumeEnergy(extra);
        }
    }

    /// <summary>
    /// Vraci 0..1 podle hloubky pod hladinou.
    /// 0 = na hladine nebo nad (zadny boost).
    /// 1 = hlouboko pod hladinou (plny boost).
    /// Plynuly prechod pres falloffDistance.
    /// </summary>
    private static float GetDepthFalloff(SeaMoth seamoth, float falloffDistance)
    {
        if (falloffDistance <= 0.01f) return 1f; // falloff disabled
        float oceanLevel = Ocean.GetOceanLevel();
        float depth = oceanLevel - seamoth.transform.position.y;
        return Mathf.Clamp01(depth / falloffDistance);
    }

    /// <summary>
    /// Aplikuje discount spotreby pokud je osazeny vanilla Vehicle Power Upgrade Module.
    /// Kazdy osazeny modul snizuje spotrebu na polovinu (vanilla chovani).
    /// </summary>
    private static float GetPowerUpgradeMultiplier(SeaMoth seamoth)
    {
        if (seamoth.modules == null) return 1f;
        int count = seamoth.modules.GetCount(TechType.VehiclePowerUpgradeModule);
        if (count <= 0) return 1f;
        return Mathf.Pow(0.5f, count);
    }
}
