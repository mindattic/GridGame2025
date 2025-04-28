using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialRepo
{
    private static Dictionary<string, Material> materials;

    public static Dictionary<string, Material> Materials
    {
        get
        {
            if (materials == null)
                Load();
            return materials;
        }
    }

    private static void Load()
    {
        materials = new Dictionary<string, Material>
        {
            { "EnemyParallax", AssetHelper.LoadAsset<Material>("Materials/EnemyParallax") },
            { "PlayerParallax", AssetHelper.LoadAsset<Material>("Materials/PlayerParallax") },
            { "SpriteOutline", AssetHelper.LoadAsset<Material>("Materials/SpriteOutline") },
            { "SpritePan",     AssetHelper.LoadAsset<Material>("Materials/SpritePan") }
        };
    }
}
