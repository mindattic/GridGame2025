using Game.Models;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// BoardInstance manages the game board grid. It calculates board offset and bounds,
/// generates tiles, and provides convenient world-edge accessors.
/// </summary>
public class BoardInstance : MonoBehaviour
{
    // Fields
    [HideInInspector] public int columnCount = 6;   // Total number of columns on the board.
    [HideInInspector] public int rowCount = 8;      // Total number of rows on the board.
    [HideInInspector] public Vector2 offset;        // Board origin in world space.
    [HideInInspector] public RectFloat bounds;      // World-space rectangle enclosing the board.
    [HideInInspector] public Vector2 center;        // Center point of the board in world space.
    [HideInInspector] public RectVector3 worldEdges; // Holds top, right, bottom, left midpoints
    [HideInInspector] public RectVector3 screenEdges; // Screen-space edge midpoints

    /// <summary>
    /// Sets up the board by assigning position, computing bounds, and generating tiles.
    /// </summary>
    public void Initialize()
    {
        AssignPosition();
        AssignBounds();
        GenerateTiles();
    }

    /// <summary>
    /// Calculates and applies the board's world-space origin offset so the board is centered.
    /// </summary>
    private void AssignPosition()
    {
        // Center horizontally: shift left by half the board width.
        var x = -(g.TileSize * 3) - g.TileSize * 0.5f;

        // Place vertically: shift down from the origin by half a tile from the top row.
        var y = (g.TileSize * 4) + g.TileSize * 0.5f;

        offset = new Vector2(x, y);

        // Move this transform to match the computed offset.
        transform.position = offset;
    }

    /// <summary>
    /// Computes world-space bounds from the offset, tile size, and board dimensions, and caches the center.
    /// </summary>
    private void AssignBounds()
    {
        bounds = new RectFloat();

        bounds.Top = offset.y - g.TileSize * 0.5f;
        bounds.Right = offset.x + (g.TileSize * columnCount) + g.TileSize * 0.5f;
        bounds.Bottom = offset.y - (g.TileSize * rowCount) - g.TileSize * 0.5f;
        bounds.Left = offset.x + g.TileSize * 0.5f;

        center = new Vector2(
            (bounds.Left + bounds.Right) * 0.5f,
            (bounds.Top + bounds.Bottom) * 0.5f
        );

        // Store all four edge midpoints in RectVector3
        worldEdges = new RectVector3(
            new Vector3(center.x, bounds.Top, 0f),    // Top
            new Vector3(bounds.Right, center.y, 0f),  // Right
            new Vector3(center.x, bounds.Bottom, 0f), // Bottom
            new Vector3(bounds.Left, center.y, 0f)    // Left
        );

        // Convert world-space worldEdges to screen-space worldEdges
        screenEdges = new RectVector3(
            Camera.main.WorldToScreenPoint(worldEdges.Top),
            Camera.main.WorldToScreenPoint(worldEdges.Right),
            Camera.main.WorldToScreenPoint(worldEdges.Bottom),
            Camera.main.WorldToScreenPoint(worldEdges.Left)
        );
    }

    /// <summary>
    /// Instantiates tile prefabs for each grid position, initializes them, and registers them in global maps.
    /// </summary>
    private void GenerateTiles()
    {
        var tilePrefab = PrefabLibrary.Prefabs["TilePrefab"];

        // Create tiles for each grid cell.
        for (int col = 1; col <= columnCount; col++)
        {
            for (int row = 1; row <= rowCount; row++)
            {
                var go = Instantiate(tilePrefab, Vector2.zero, Quaternion.identity);

                var instance = go.GetComponent<TileInstance>();
                instance.parent = transform;
                instance.name = $"Tile_{col}s{row}";
                instance.Initialize(col, row);

                g.TileMap.Add(instance);
            }
        }

        // Set grid origin and tile sizing for the TileMap.
        g.TileMap.gridOrigin = g.TileMap.GetTile(new Vector2Int(1, 1)).position;
        g.TileMap.tileSize = g.TileSize;

        // Cache all tiles from the scene into the global list.
        var tileObjects = GameObject.FindObjectsByType<TileInstance>(FindObjectsSortMode.None);
        foreach (var obj in tileObjects)
        {
            var tile = obj.GetComponent<TileInstance>();
            if (tile != null)
            {
                g.Tiles.Add(tile);
            }
        }
    }

    /// <summary>
    /// Returns true if a grid location is within board bounds.
    /// </summary>
    public bool InBounds(Vector2Int location)
    {
        return location.x >= 1 && location.x <= columnCount
            && location.y >= 1 && location.y <= rowCount;
    }

}
