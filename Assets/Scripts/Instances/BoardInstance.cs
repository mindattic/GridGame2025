using Assets.Scripts.Models;
using Game.Models;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// BoardInstance represents the game board grid, handling tile generation, board bounds calculation,
// and conversion between board and screen positions. It also holds a reference to the TileMap.
public class BoardInstance : MonoBehaviour
{
    // Quick Reference Properties:
    // Retrieve the global tile size from the GameManager.
    protected float tileSize => GameManager.instance.tileSize;
    // Access or assign the TileMap, which is a custom container of TileInstance objects.
    protected TileMap tileMap { get => GameManager.instance.tileMap; set => GameManager.instance.tileMap = value; }

    // Fields:
    [SerializeField] public GameObject TilePrefab; // Prefab used to instantiate individual tiles.
    [HideInInspector] public int columnCount = 6;      // Number of columns on the board.
    [HideInInspector] public int rowCount = 8;         // Number of rows on the board.
    [HideInInspector] public Vector2 offset;           // Board offset (used to position the board in world space).
    [HideInInspector] public RectFloat bounds;         // Bounds of the board, calculated from the offset and dimensions.
    [HideInInspector] public Vector2 center;           // Center point of the board bounds.

    /// <summary>
    /// Initialize is called to set up the board by calculating its offset, bounds, and generating the tiles.
    /// </summary>
    public void Initialize()
    {
        CalculateOffset();
        CalculateBounds();
        GenerateTiles();
    }

    /// <summary>
    /// Calculates the offset for the board based on the tile size and desired board centering.
    /// The offset is then applied to the board's transform position.
    /// </summary>
    private void CalculateOffset()
    {
        // Calculate x-offset so that the board is centered horizontally.
        // Here, -(tileSize * 3) shifts left by three tiles and subtracts half a tile.
        var x = -(tileSize * 3) - tileSize / 2;
        // Calculate y-offset to position the board vertically.
        // Here, (tileSize * 4) + tileSize * 2 positions the board using 6 tiles' height.
        var y = (tileSize * 4) + tileSize * 2;
        offset = new Vector2(x, y);
        // Set the board's world position to the calculated offset.
        transform.position = offset;
    }

    /// <summary>
    /// Calculates the bounds of the board based on the offset, tile size, and board dimensions.
    /// Also calculates the center of the board.
    /// </summary>
    private void CalculateBounds()
    {
        bounds = new RectFloat();
        // Top bound: offset y minus half a tile.
        bounds.Top = offset.y - tileSize / 2;
        // Right bound: offset x plus the width of all columns plus half a tile.
        bounds.Right = offset.x + (tileSize * columnCount) + tileSize / 2;
        // Bottom bound: offset y minus the height of all rows minus half a tile.
        bounds.Bottom = offset.y - (tileSize * rowCount) - tileSize / 2;
        // Left bound: offset x plus half a tile.
        bounds.Left = offset.x + tileSize / 2;
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
        // Create a new TileMap to store the board's tiles.
        tileMap = new TileMap();

        // Loop over each column and row to generate tiles.
        for (int col = 1; col <= columnCount; col++)
        {
            for (int row = 1; row <= rowCount; row++)
            {
                // Instantiate a tile prefab at the origin with no rotation.
                var prefab = Instantiate(TilePrefab, Vector2.zero, Quaternion.identity);
                // Get the TileInstance component from the instantiated prefab.
                var instance = prefab.GetComponent<TileInstance>();
                // Set the parent of the tile to be this board, so they are organized under the board.
                instance.parent = transform;
                // Name the tile based on its grid coordinates.
                instance.name = $"Tile_{col}x{row}";
                // Initialize the tile with its column and row positions.
                instance.Initialize(col, row);
                // Add the tile to the TileMap.
                tileMap.Add(instance);
            }
        }

        // Set the grid origin of the TileMap to the position of the tile at grid (1,1).
        tileMap.gridOrigin = tileMap.GetTile(new Vector2Int(1, 1)).position;
        // Set the tile size in the TileMap.
        tileMap.tileSize = tileSize;

        // Find all GameObjects tagged as "Tile" and add their TileInstance components to the global GameManager's tiles list.
        GameObject.FindGameObjectsWithTag(Tag.Tile).ToList()
            .ForEach(x => GameManager.instance.tiles.Add(x.GetComponent<TileInstance>()));
    }

    /// <summary>
    /// Converts a board point (e.g., TopLeft, MiddleCenter) into screen coordinates.
    /// It calculates the world position based on board bounds and converts that to screen space.
    /// </summary>
    /// <param name="point">The board point to convert.</param>
    /// <returns>A Vector2 representing the screen position.</returns>
    public Vector2 ScreenPosition(BoardPoint point)
    {
        // Calculate the world position based on the board point using a switch expression.
        Vector3 worldPosition = point switch
        {
            BoardPoint.TopLeft => new Vector3(bounds.Left, bounds.Top, 0),
            BoardPoint.TopCenter => new Vector3(center.x, bounds.Top, 0),
            BoardPoint.TopRight => new Vector3(bounds.Right, bounds.Top, 0),
            BoardPoint.MiddleLeft => new Vector3(bounds.Left, center.y, 0),
            BoardPoint.MiddleCenter => new Vector3(center.x, center.y, 0),
            BoardPoint.MiddleRight => new Vector3(bounds.Right, center.y, 0),
            BoardPoint.BottomLeft => new Vector3(bounds.Left, bounds.Bottom, 0),
            BoardPoint.BottomCenter => new Vector3(center.x, bounds.Bottom, 0),
            BoardPoint.BottomRight => new Vector3(bounds.Right, bounds.Bottom, 0),
            _ => Vector3.zero // Fallback case returns (0,0,0).
        };

        // Convert the world position to screen space using the main camera.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Return only the X and Y components as a Vector2.
        return new Vector2(screenPosition.x, screenPosition.y);
    }

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
}

// BoardPoint is an enumeration used to specify key reference points on the board,
// such as corners, edges, or the center. This is used for UI positioning.
public enum BoardPoint
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}
