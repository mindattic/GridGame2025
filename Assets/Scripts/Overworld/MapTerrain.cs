using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class MapTerrain : MonoBehaviour
{
    [Tooltip("Discard islands smaller than this many vertices.")]
    public int minVertexCount = 6;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        var sprite = sr ? sr.sprite : null;
        if (sprite == null) return;

        // Ensure clean slate (nothing serialized in scene)
        var existing = GetComponent<PolygonCollider2D>();
        if (existing) Destroy(existing);

        var poly = gameObject.AddComponent<PolygonCollider2D>();
        BuildFromSpritePhysicsShape(sprite, poly, minVertexCount);
    }

    private static void BuildFromSpritePhysicsShape(Sprite sprite, PolygonCollider2D poly, int minVerts)
    {
        int shapeCount = sprite.GetPhysicsShapeCount();
        var points = new List<Vector2>();
        var paths = new List<Vector2[]>(shapeCount);

        for (int i = 0; i < shapeCount; i++)
        {
            points.Clear();
            sprite.GetPhysicsShape(i, points);

            if (points.Count >= minVerts)
                paths.Add(points.ToArray());
        }

        poly.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
            poly.SetPath(i, paths[i]);
    }
}
