using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabRepo
{
    private static Dictionary<string, GameObject> prefabs;

    public static Dictionary<string, GameObject> Prefabs
    {
        get
        {
            if (prefabs == null)
                Load();
            return prefabs;
        }
    }

    private static void Load()
    {
        prefabs = new Dictionary<string, GameObject>
        {
            { "AbilityButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/AbilityButtonPrefab") },
            { "ActorPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ActorPrefab") },
            { "AttackLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/AttackLinePrefab") },
            { "CanvasParticlePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CanvasParticlePrefab") },
            { "CoinPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CoinPrefab") },
            { "ConfirmationDialog", AssetHelper.LoadAsset<GameObject>("Prefabs/ConfirmationDialog") },
            { "CombatTextPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CombatTextPrefab") },
            { "DottedLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/DottedLinePrefab") },
            { "FootstepPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/FootstepPrefab") },
            { "GhostPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/GhostPrefab") },
            { "KeyboardDialog", AssetHelper.LoadAsset<GameObject>("Prefabs/KeyboardDialog") },
            { "KeyButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/KeyButtonPrefab") },
            { "Portrait2DPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/Portrait2DPrefab") },
            { "Portrait3DPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/Portrait3DPrefab") },
            { "RosterSlidePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/RosterSlidePrefab") },
            { "SaveFileButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SaveFileButtonPrefab") },
            { "ScreenWidthButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ScreenWidthButtonPrefab") },
            { "SynergyLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SynergyLinePrefab") },
            { "SynergyLineStrandPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SynergyLineStrandPrefab") },
            { "ProjectilePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ProjectilePrefab") },
            { "StatRow", AssetHelper.LoadAsset<GameObject>("Prefabs/StatRow") },
            { "SupportLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SupportLinePrefab") },
            { "TargetLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TargetLinePrefab") },
            { "TilePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TilePrefab") },
            { "TooltipPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TooltipPrefab") },
            { "WallPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/WallPrefab") }
        };
    }

    // Safe direct access
    public static GameObject Get(string key)
    {
        if (prefabs == null)
            Load();

        if (prefabs.TryGetValue(key, out var prefab) && prefab != null)
            return prefab;

        Debug.LogError($"Prefab '{key}' not found or is null in PrefabRepo.");
        return null;
    }
}
