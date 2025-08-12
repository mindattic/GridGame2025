using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private GameObject CoinPrefab;

    public void Awake()
    {
        CoinPrefab = PrefabLibrary.Prefabs["CoinPrefab"];
    }

    public void Spawn(Vector3 position)
    {
        var go = Instantiate(CoinPrefab, Vector2.zero, Quaternion.identity);
        var instance = go.GetComponent<CoinInstance>();
        instance.name = $"Coin_{Guid.NewGuid():N}";
        instance.Spawn(position);
    }

}
