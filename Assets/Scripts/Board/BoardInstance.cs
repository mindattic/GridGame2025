using Game.Models;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

// BoardInstance represents the game board grid, handling tile generation, board bounds calculation,
// and conversion between board and screen positions. It also holds a reference to the TileMap.
public class BoardInstance : MonoBehaviour
{
    //Fields
    [HideInInspector] public int columnCount = 6;      // Index of columns on the board.
    [HideInInspector] public int rowCount = 8;         // Index of rows on the board.
    [HideInInspector] public Vector2 offset;           // Board offset (used to position the board in world space).
    [HideInInspector] public RectFloat bounds;         // Bounds of the board, calculated from the offset and dimensions.
    [HideInInspector] public Vector2 center;           // Center point of the board bounds.

    /// <summary>
    /// World-space left edge of the board.
    /// </summary>
    public Vector3 WorldLeftEdge => new Vector3(bounds.Left, center.y, 0);

    /// <summary>
    /// World-space right edge of the board.
    /// </summary>
    public Vector3 WorldRightEdge => new Vector3(bounds.Right, center.y, 0);

    /// <summary>
    /// World-space top edge of the board.
    /// </summary>
    public Vector3 WorldTopEdge => new Vector3(center.x, bounds.Top, 0);

    /// <summary>
    /// Show is called to set up the board by calculating its offset, bounds, and generating the tiles.
    /// </summary>
    public void Initialize()
    {
        AssignPosition();
        AssignBounds();
        GenerateTiles();
    }

    /// <summary>
    /// Calculates the offset for the board based on the tile size and desired board centering.
    /// The offset is then applied to the board's transform position.
    /// </summary>
    private void AssignPosition()
    {
        // Calculate x-offset so that the board is centered horizontally.
        // Here, -(tileSize * 3) shifts left by three tiles and subtracts half a tile.
        var x = -(g.TileSize * 3) - g.TileSize / 2;
        // Calculate y-offset to position the board vertically.
        // Here, (tileSize * 4) + tileSize * 2 positions the board using 6 tiles' height.
        var y = (g.TileSize * 4) + g.TileSize / 2;
        offset = new Vector2(x, y);
        // Show the board's world position to the calculated offset.
        transform.position = offset;
    }

    /// <summary>
    /// Calculates the bounds of the board based on the offset, tile size, and board dimensions.
    /// Also calculates the center of the board.
    /// </summary>
    private void AssignBounds()
    {
        bounds = new RectFloat();
        // Top bound: offset y minus half a tile.
        bounds.Top = offset.y - g.TileSize / 2;
        // Right bound: offset x plus the width of all columns plus half a tile.
        bounds.Right = offset.x + (g.TileSize * columnCount) + g.TileSize / 2;
        // Bottom bound: offset y minus the height of all rows minus half a tile.
        bounds.Bottom = offset.y - (g.TileSize * rowCount) - g.TileSize / 2;
        // Left bound: offset x plus half a tile.
        bounds.Left = offset.x + g.TileSize / 2;
        // Calculate center as the average of left/right and top/bottom bounds.
        center = new Vector2(
            (bounds.Left + bounds.Right) / 2,
            (bounds.Top + bounds.Bottom) / 2);
    }

    /// <summary>
    /// Generates the board tiles by instantiating the TilePrefab for each grid position.
    /// Each tile is initialized and added to the global TileMap.
    /// Finally, all tiles found with the "Tile" tag are added to the GameManager's tile list.
    /// </summary>
    private void GenerateTiles()
    {
        var TilePrefab = PrefabRepo.Prefabs["TilePrefab"];

        // Loop over each column and row to generate tiles.
        for (int col = 1; col <= columnCount; col++)
        {
            for (int row = 1; row <= rowCount; row++)
            {
                var prefab = Instantiate(TilePrefab, Vector2.zero, Quaternion.identity);
                var instance = prefab.GetComponent<TileInstance>();
                instance.parent = transform;
                instance.name = $"Tile_{col}x{row}";
                instance.Initialize(col, row);
                g.TileMap.Add(instance);
            }
        }

        g.TileMap.gridOrigin = g.TileMap.GetTile(new Vector2Int(1, 1)).position;
        g.TileMap.tileSize = g.TileSize;

        // Find all GameObjects tagged as "Tile" and add their TileInstance components to the global GameManager's tiles list.
        GameObject.FindGameObjectsWithTag(Tag.Tile).ToList()
            .ForEach(x => g.Tiles.Add(x.GetComponent<TileInstance>()));
    }

    /// <summary>
    /// Converts a board point (e.g., TopLeft, MiddleCenter) into screen coordinates.
    /// It calculates the world position based on board bounds and converts that to screen space.
    /// </summary>
    /// <param name="point">The board point to convert.</param>
    /// <returns>A Vector2 representing the screen position.</returns>
    //public Vector2 ScreenPosition(BoardPoint point)
    //{
    //    // Calculate the world position based on the board point using a switch expression.
    //    Vector3 worldPosition = point switch
    //    {
    //        BoardPoint.TopLeft => new Vector3(bounds.Left, bounds.Top, 0),
    //        BoardPoint.TopCenter => new Vector3(center.x, bounds.Top, 0),
    //        BoardPoint.TopRight => new Vector3(bounds.Right, bounds.Top, 0),
    //        BoardPoint.MiddleLeft => new Vector3(bounds.Left, center.y, 0),
    //        BoardPoint.MiddleCenter => new Vector3(center.x, center.y, 0),
    //        BoardPoint.MiddleRight => new Vector3(bounds.Right, center.y, 0),
    //        BoardPoint.BottomLeft => new Vector3(bounds.Left, bounds.Bottom, 0),
    //        BoardPoint.BottomCenter => new Vector3(center.x, bounds.Bottom, 0),
    //        BoardPoint.BottomRight => new Vector3(bounds.Right, bounds.Bottom, 0),
    //        _ => Vector3.zero // Fallback case returns (0,0,0).
    //    };

    //    // Convert the world position to screen space using the main camera.
    //    Vector3 screenPosition = CameraManager.main.WorldToScreenPoint(worldPosition);

    //    // Return only the X and Y components as a Vector2.
    //    return new Vector2(screenPosition.x, screenPosition.y);
    //}

    /// <summary>
    /// Checks whether a given grid location is within the bounds of the board.
    /// </summary>
    /// <param name="location">The grid location to test.</param>
    /// <returns>True if the location is within the board, otherwise false.</returns>
    public bool InBounds(Vector2Int location)
    {
        return location.x >= 1 && location.x <= columnCount
            && location.y >= 1 && location.y <= rowCount;
    }





#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw board bounds as a wire rectangle
        Gizmos.color = Color.yellow;

        var w = bounds.Right - bounds.Left;
        var h = bounds.Top - bounds.Bottom;
        var c = new Vector3((bounds.Left + bounds.Right) * 0.5f, (bounds.Top + bounds.Bottom) * 0.5f, 0f);

        // If bounds are not assigned yet, preview from current offset/size
        if (w <= 0f || h <= 0f)
        {
            float left = offset.x + g.TileSize * 0.5f;
            float right = offset.x + (g.TileSize * columnCount) + g.TileSize * 0.5f;
            float top = offset.y - g.TileSize * 0.5f;
            float bottom = offset.y - (g.TileSize * rowCount) - g.TileSize * 0.5f;

            w = right - left;
            h = top - bottom;
            c = new Vector3((left + right) * 0.5f, (top + bottom) * 0.5f, 0f);
        }

        Gizmos.DrawWireCube(c, new Vector3(w, h, 0f));
    }
#endif







}

// BoardPoint is an enumeration used to specify key reference points on the board,
// such as corners, edges, or the center. This is used for UI positioning.
