namespace InferiusQoL.Features.TeleportBeacon;

using System.Collections.Generic;
using InferiusQoL.Config;
using InferiusQoL.Logging;
using UnityEngine;

/// <summary>
/// MonoBehaviour pripojeny na kazdy spawnly Teleport Beacon prefab. Implementuje
/// IHandTarget pro interakci s hracem (hover tooltip + click teleport).
///
/// Registry vsech aktivnich instanci (All) slouzi k vyhledani cilu pri teleportu.
/// Nejblizsi jiny beacon = cil. Energy check a drain na obou basech, cooldown
/// per beacon.
/// </summary>
public class TeleportBeaconBehavior : MonoBehaviour, IHandTarget
{
    public static readonly List<TeleportBeaconBehavior> All = new List<TeleportBeaconBehavior>();

    private float _cooldownUntil = 0f;

    private void OnEnable()
    {
        if (!All.Contains(this)) All.Add(this);
        QoLLog.Debug(Category.Teleport, $"Beacon registered. Total active: {All.Count}");
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    public void OnHandHover(GUIHand hand)
    {
        if (!InferiusConfig.Instance.TeleportBeaconEnabled) return;

        HandReticle.main.SetIcon(HandReticle.IconType.Interact);
        HandReticle.main.SetText(HandReticle.TextType.Hand, GetHoverText(), false);
    }

    public void OnHandClick(GUIHand hand)
    {
        var cfg = InferiusConfig.Instance;
        if (!cfg.TeleportBeaconEnabled) return;

        if (Time.time < _cooldownUntil)
        {
            var remaining = (int)(_cooldownUntil - Time.time);
            ErrorMessage.AddMessage($"Teleport on cooldown: {remaining}s");
            return;
        }

        var target = FindClosestOtherBeacon();
        if (target == null)
        {
            ErrorMessage.AddMessage("No other teleport beacons found");
            return;
        }

        // Check power
        var thisBase = GetPowerRelay(this);
        var targetBase = GetPowerRelay(target);
        if (!HasEnoughPower(thisBase, cfg.TeleportMinBasePowerPercent))
        {
            ErrorMessage.AddMessage("Source base power too low");
            return;
        }
        if (!HasEnoughPower(targetBase, cfg.TeleportMinBasePowerPercent))
        {
            ErrorMessage.AddMessage("Target base power too low");
            return;
        }

        // Energy cost = base cost + distance-based cost.
        var distance = Vector3.Distance(transform.position, target.transform.position);
        var distanceCost = (distance / 100f) * cfg.TeleportCostPerHundredMeters;
        var totalSourceCost = cfg.TeleportSourceCostJoules + (distanceCost * 0.5f);
        var totalTargetCost = cfg.TeleportTargetCostJoules + (distanceCost * 0.5f);

        DrainPower(thisBase, totalSourceCost);
        DrainPower(targetBase, totalTargetCost);

        // Teleport player pres Subnautica standard API.
        var destination = target.transform.position + Vector3.up * 1.5f;
        var player = Player.main;
        if (player != null)
        {
            player.SetPosition(destination);
            if (player.rigidBody != null)
            {
                player.rigidBody.velocity = Vector3.zero;
                player.rigidBody.ResetInertiaTensor();
            }
        }

        _cooldownUntil = Time.time + cfg.TeleportCooldownSeconds;
        target._cooldownUntil = Time.time + cfg.TeleportCooldownSeconds; // target tez cooldown

        QoLLog.Info(Category.Teleport,
            $"Teleported {distance:0}m to beacon at {target.transform.position}, drained {totalSourceCost:0}+{totalTargetCost:0} J");
    }

    private string GetHoverText()
    {
        var target = FindClosestOtherBeacon();
        if (target == null)
            return "Teleport Beacon (no other beacons)";

        var dist = Vector3.Distance(transform.position, target.transform.position);
        if (Time.time < _cooldownUntil)
        {
            var remaining = (int)(_cooldownUntil - Time.time);
            return $"Teleport Beacon (cooldown: {remaining}s)";
        }
        var cfg = InferiusConfig.Instance;
        var distanceCost = (dist / 100f) * cfg.TeleportCostPerHundredMeters;
        var totalCost = cfg.TeleportSourceCostJoules + cfg.TeleportTargetCostJoules + distanceCost;
        return $"Teleport to nearest beacon ({dist:0}m, ~{totalCost:0} J)";
    }

    private TeleportBeaconBehavior? FindClosestOtherBeacon()
    {
        float minDist = float.MaxValue;
        TeleportBeaconBehavior? closest = null;
        foreach (var b in All)
        {
            if (b == null || b == this) continue;
            var dist = Vector3.Distance(transform.position, b.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = b;
            }
        }
        return closest;
    }

    private static PowerRelay? GetPowerRelay(TeleportBeaconBehavior beacon)
    {
        if (beacon == null) return null;
        return beacon.GetComponentInParent<PowerRelay>();
    }

    private static bool HasEnoughPower(PowerRelay? relay, float minPercent)
    {
        if (relay == null) return true; // mimo base nebo bez relay = bez kontroly
        if (relay.GetMaxPower() <= 0f) return false;
        var ratio = relay.GetPower() / relay.GetMaxPower() * 100f;
        return ratio >= minPercent;
    }

    private static void DrainPower(PowerRelay? relay, float amount)
    {
        if (relay == null) return;
        if (amount <= 0f) return;
        relay.ConsumeEnergy(amount, out _);
    }
}
