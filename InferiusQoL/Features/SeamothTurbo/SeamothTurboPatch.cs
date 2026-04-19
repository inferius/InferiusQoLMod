namespace InferiusQoL.Features.SeamothTurbo;

using HarmonyLib;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// Turbo boost pro Seamoth. Postfix patch na SeaMoth.Update.
///
/// Pouziva AddForce s rychlostnim cap-em: pokud aktualni dopredna rychlost je
/// pod cilovou (vanilla_forwardForce * multiplier), prida force az do capu.
/// Tim se zabrani akumulaci a "raketovym" skokum nad hladinou.
///
/// Navic: turbo se aplikuje jen pod vodou - nad hladinou je vanilla drag
/// nizky a Seamoth by skakal. Na hladine se turbo automaticky vypne.
/// </summary>
[HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
public static class SeaMoth_Update_Patch
{
    private const float SeamothBaseDrainPerSecond = 0.15f;

    /// <summary>True jen pri prvnim uspesnem boostu - abychom debug log nespammali.</summary>
    private static bool _loggedFirstBoost = false;

    [HarmonyPostfix]
    public static void Postfix(SeaMoth __instance)
    {
        if (!ShouldBoost(__instance)) return;
        ApplyBoost(__instance);
    }

    private static bool ShouldBoost(SeaMoth seamoth)
    {
        if (seamoth == null) return false;

        var cfg = InferiusConfig.Instance;
        if (!cfg.SeamothTurboEnabled) return false;
        if (SeamothTurboItem.TechType == TechType.None) return false;

        var player = Player.main;
        if (player == null) return false;
        if (player.GetVehicle() != seamoth) return false;

        if (seamoth.modules == null) return false;
        if (seamoth.modules.GetCount(SeamothTurboItem.TechType) <= 0) return false;

        if (!GameInput.GetButtonHeld(GameInput.Button.Sprint)) return false;

        if (!seamoth.HasEnoughEnergy(0.01f)) return false;

        if (!IsUnderwater(seamoth)) return false;

        return true;
    }

    private static void ApplyBoost(SeaMoth seamoth)
    {
        var cfg = InferiusConfig.Instance;
        var speedMult = cfg.SeamothTurboSpeedMultiplier;  // napr. 2.0
        var speedBonus = speedMult - 1f;                  // napr. 1.0 (extra)
        var energyBonus = cfg.SeamothTurboEnergyMultiplier - 1f;

        var rb = seamoth.useRigidbody;
        if (rb != null && speedBonus > 0f)
        {
            Vector3 forward = seamoth.transform.forward;
            float currentForwardSpeed = Vector3.Dot(rb.velocity, forward);
            float targetMaxSpeed = seamoth.forwardForce * speedMult;

            if (currentForwardSpeed < targetMaxSpeed)
            {
                // Aplikuj force jen aby dorazila k targetu - ne dal.
                float deltaNeeded = targetMaxSpeed - currentForwardSpeed;
                float dt = Time.deltaTime;
                float maxStep = seamoth.forwardForce * speedBonus * dt;
                float stepForce = Mathf.Min(maxStep, deltaNeeded);

                // Acceleration mode: aplikuje delta directly, nezavisle na hmotnosti.
                rb.velocity += forward * stepForce;

                if (!_loggedFirstBoost)
                {
                    _loggedFirstBoost = true;
                    QoLLog.Info(Category.Seamoth,
                        $"Turbo boost active: speed {speedMult:0.0}x, target {targetMaxSpeed:0.0} m/s, current {currentForwardSpeed:0.0} m/s");
                }
            }
        }

        // Extra energy drain.
        if (energyBonus > 0f)
        {
            var extra = energyBonus * SeamothBaseDrainPerSecond * Time.deltaTime;
            seamoth.energyInterface?.ConsumeEnergy(extra);
        }
    }

    private static bool IsUnderwater(SeaMoth seamoth)
    {
        var oceanLevel = Ocean.GetOceanLevel();
        return seamoth.transform.position.y + 1.0f < oceanLevel;
    }
}
