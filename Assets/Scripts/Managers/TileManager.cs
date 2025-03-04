using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TileManager is responsible for managing tile-specific behaviors on the board.
/// It accesses the global TileMap and the list of TileInstances from the GameManager.
/// </summary>
public class TileManager : MonoBehaviour
{
    // Quick Reference Properties:
    // These properties provide shortcuts to the list of tiles and the tile map,
    // which are stored in the GameManager singleton.

    /// <summary>
    /// Gets the list of all TileInstance objects managed by the game.
    /// </summary>
    protected List<TileInstance> tiles => GameManager.instance.tiles;

    /// <summary>
    /// Gets the TileMap that contains mappings between grid locations and TileInstances.
    /// </summary>
    protected TileMap tileMap => GameManager.instance.tileMap;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It is currently empty but can be used for early initialization if needed.
    /// </summary>
    public void Awake()
    {
        // Initialization code can be added here.
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// It is currently empty but can be used for initialization after Awake.
    /// </summary>
    public void Start()
    {
        // Initialization code that depends on other GameObjects being initialized can be added here.
    }

    /// <summary>
    /// Resets the color of all tiles to their default translucent white.
    /// This can be used to clear any visual highlights on the board.
    /// </summary>
    public void Reset()
    {
        foreach (var tile in tiles)
        {
            // Reset each tile's sprite color to a translucent white.
            tile.spriteRenderer.color = ColorHelper.Translucent.White;
        }
    }

    /// <summary>
    /// Updates the tile colors when the selected player's location changes.
    /// It sets the color of the tile at the previous location to white and
    /// the new location tile to yellow, providing visual feedback.
    /// </summary>
    /// <param name="previousLocation">The grid location that was previously selected.</param>
    /// <param name="newLocation">The new grid location that is now selected.</param>
    public void OnSelectedPlayerLocationChanged(Vector2Int previousLocation, Vector2Int newLocation)
    {
        // Set the previous tile's color back to white.
        tileMap.GetTile(previousLocation).color = ColorHelper.Translucent.White;
        // Highlight the new tile by setting its color to yellow.
        tileMap.GetTile(newLocation).color = ColorHelper.Translucent.Yellow;
    }
}
