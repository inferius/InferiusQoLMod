namespace InferiusQoL.Features.AutoCraft;

using System.Collections.Generic;
using UnityEngine;

internal static class AutoCraftHelpers
{
    public static void Inc<T>(this Dictionary<T, int> dict, T key, int value = 1)
    {
        dict.TryGetValue(key, out var cur);
        dict[key] = cur + value;
    }

    public static void Inc<T>(this Dictionary<T, float> dict, T key, float value = 1f)
    {
        dict.TryGetValue(key, out var cur);
        dict[key] = cur + value;
    }

    public static PowerRelay? GetPowerRelay(this GhostCrafter crafter)
    {
        return crafter?.powerRelay;
    }

    public static float GetDistanceToPlayer(this BaseRoot baseRoot)
    {
        if (baseRoot?.baseComp == null || Player.main == null) return float.MaxValue;
        var grid = baseRoot.baseComp.WorldToGrid(Player.main.transform.position);
        int bestSq = int.MaxValue;
        Int3 bestCell = new Int3(int.MaxValue);
        foreach (var cell in baseRoot.baseComp.AllCells)
        {
            var diff = grid - cell;
            int sq = diff.SquareMagnitude();
            if (sq < bestSq) { bestSq = sq; bestCell = cell; }
        }
        Vector3 worldPos = baseRoot.baseComp.GridToWorld(bestCell);
        return (Player.main.transform.position - worldPos).magnitude;
    }

    public static float GetDistanceToPlayer(this Vehicle vehicle)
    {
        if (vehicle == null || Player.main == null) return float.MaxValue;
        var playerPos = Player.main.transform.position;
        if (vehicle.useRigidbody == null)
            return (playerPos - vehicle.transform.position).magnitude;
        return (playerPos - vehicle.transform.TransformPoint(vehicle.useRigidbody.centerOfMass)).magnitude;
    }

    public static float GetDistanceToPlayer(this SubRoot subRoot)
    {
        if (subRoot == null || Player.main == null) return float.MaxValue;
        if (subRoot is BaseRoot br) return br.GetDistanceToPlayer();
        return (Player.main.transform.position - subRoot.GetWorldCenterOfMass()).magnitude;
    }
}
