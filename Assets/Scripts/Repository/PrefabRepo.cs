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
            { "ActorPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ActorPrefab") },
            { "AttackLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/AttackLinePrefab") },
            { "CanvasParticlePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CanvasParticlePrefab") },
            { "CoinPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/CoinPrefab") },
            { "ConfirmationDialog", AssetHelper.LoadAsset<GameObject>("Prefabs/ConfirmationDialog") },
            { "DamageTextPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/DamageTextPrefab") },
            { "DottedLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/DottedLinePrefab") },
            { "FootstepPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/FootstepPrefab") },
            { "GhostPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/GhostPrefab") },
            { "KeyboardDialog", AssetHelper.LoadAsset<GameObject>("Prefabs/KeyboardDialog") },
            { "KeyButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/KeyButtonPrefab") },
            { "PortraitPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/PortraitPrefab") },
            { "RosterSlidePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/RosterSlidePrefab") },
            { "SaveFileButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SaveFileButtonPrefab") },
            { "ScreenWidthButtonPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/ScreenWidthButtonPrefab") },
            { "SpellPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SpellPrefab") },
            { "StatRow", AssetHelper.LoadAsset<GameObject>("Prefabs/StatRow") },
            { "SupportLinePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/SupportLinePrefab") },
            { "TilePrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TilePrefab") },
            { "TooltipPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/TooltipPrefab") },
            { "WallPrefab", AssetHelper.LoadAsset<GameObject>("Prefabs/WallPrefab") }
        };
    }

    // Optional helper for direct prefab access
    public static GameObject Get(string key)
    {
        if (prefabs.TryGetValue(key, out var prefab))
            return prefab;

        Debug.LogError($"Prefab '{key}' not found in PrefabRepo.");
        return null;
    }
}
