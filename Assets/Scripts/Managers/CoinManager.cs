using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] public GameObject CoinPrefab;

    public void Spawn(Vector3 position)
    {
        var prefab = Instantiate(CoinPrefab, Vector2.zero, Quaternion.identity);
        var instance = prefab.GetComponent<CoinInstance>();
        instance.name = $"Coin_{Guid.NewGuid():N}";
        instance.Spawn(position);
    }

}
