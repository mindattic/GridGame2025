using Assets.Scripts.Models;
using Game.Models;
using System.Linq;
using UnityEngine;

public class BoardInstance : MonoBehaviour
{
   //Quick Reference Properties
    protected float tileSize => GameManager.instance.tileSize;
    protected ProfileManager profileManager => GameManager.instance.profileManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected BoardInstance board => GameManager.instance.board;

    //Fields
    [SerializeField] public GameObject TilePrefab;
    [HideInInspector] public int columnCount = 6;
    [HideInInspector] public int rowCount = 8;
    [HideInInspector] public Vector2 offset;
    [HideInInspector] public RectFloat bounds;
    [HideInInspector] public TileMap tileMap = new TileMap();
    [HideInInspector] public Vector2Int NowhereLocation = new Vector2Int(-1, -1);
    [HideInInspector] public Vector3 NowherePosition = new Vector3(-1000, -1000, -1000);
    [HideInInspector] public Vector2 center;
    [HideInInspector] public Vector3 gridOrigin;

    public void Initialize()
    {
        CalculateOffset();
        CalculateBounds();
        GenerateTiles();
    }

    private void CalculateOffset()
    {
        var x = -(tileSize * 3) - tileSize / 2;
        var y = (tileSize * 4) + tileSize * 2;
        offset = new Vector2(x, y);
        transform.position = offset;
    }

    private void CalculateBounds()
    {
        bounds = new RectFloat();
        bounds.Top = offset.y - tileSize / 2;
        bounds.Right = offset.x + (tileSize * columnCount) + tileSize / 2;
        bounds.Bottom = offset.y - (tileSize * rowCount) - tileSize / 2;
        bounds.Left = offset.x + tileSize / 2;
        center = new Vector2(
            (bounds.Left + bounds.Right) / 2,
            (bounds.Top + bounds.Bottom) / 2);
    }

    private void GenerateTiles()
    {
        for (int col = 1; col <= columnCount; col++)
        {
            for (int row = 1; row <= rowCount; row++)
            {
                var prefab = Instantiate(TilePrefab, Vector2.zero, Quaternion.identity);
                var instance = prefab.GetComponent<TileInstance>();
                instance.parent = board.transform;
                instance.name = $"Tile_{col}x{row}";
                instance.Initialize(col, row);
                tileMap.Add(instance);
                //tiles.SpawnActor(trailInstance);
            }
        }

        //TODO: Assign grid origin
        gridOrigin = new Vector3(offset.x + tileSize / 2, offset.y - tileSize / 2, 0);


        //Assign tiles queue
        GameObject.FindGameObjectsWithTag(Tag.Tile).ToList()
            .ForEach(x => GameManager.instance.tiles.Add(x.GetComponent<TileInstance>()));
    }


    public Vector2 ScreenPosition(BoardPoint point)
    {
        //Ensure the world position is correctly calculated based on the board bounds
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
            _ => Vector3.zero //Fallback case
        };

        //ConvertString the world position to screen space
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        //Return only the X and Y screen coordinates
        return new Vector2(screenPosition.x, screenPosition.y);
    }

    public TileInstance GetClosestTileEfficient(Vector3 worldPosition)
    {
        // Assume gridOrigin is defined as the center of tile (1,1):
        // gridOrigin = new Vector3(offset.x + tileSize/2, offset.y - tileSize/2, 0);
        // Convert world position to grid coordinates (1-based indexing):
        int col = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / tileSize) + 1;
        int row = Mathf.RoundToInt((gridOrigin.y - worldPosition.y) / tileSize) + 1;

        Vector2Int gridLocation = new Vector2Int(col, row);

        if (tileMap.ContainsLocation(gridLocation))
        {
            return tileMap.GetTile(gridLocation);
        }
        else
        {
            // Fallback: linear search (less efficient)
            return GameManager.instance.tiles
                .OrderBy(x => Vector3.Distance(x.transform.position, worldPosition))
                .First();
        }
    }


}

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
