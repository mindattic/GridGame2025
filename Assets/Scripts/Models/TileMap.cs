using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    // The TileMap class manages the mapping between grid locations (Vector2Int) and world positions (Vector3)
    // with associated TileInstance objects. It uses two dictionaries for fast lookups by location and by position.
    public class TileMap
    {
        // Dictionary mapping grid locations (e.g., column/row coordinates) to their corresponding TileEntry.
        private Dictionary<Vector2Int, TileEntry> locationToEntry = new Dictionary<Vector2Int, TileEntry>();
        // Dictionary mapping world positions to their corresponding TileEntry.
        private Dictionary<Vector3, TileEntry> positionToEntry = new Dictionary<Vector3, TileEntry>();

        // The origin of the grid in world space; typically the position of the first tile.
        public Vector3 gridOrigin;
        // The size of each tile; used to compute positions and distances.
        public float tileSize;

        /// <summary>
        /// Adds a new tile to the map with the specified grid location and world position.
        /// This method creates a new TileEntry and stores it in both lookup dictionaries.
        /// </summary>
        /// <param name="location">Grid coordinates of the tile.</param>
        /// <param name="position">World space position of the tile.</param>
        /// <param name="tile">The TileInstance component associated with the tile.</param>
        public void Add(Vector2Int location, Vector3 position, TileInstance tile)
        {
            // Create a new TileEntry to hold the mapping information.
            var entry = new TileEntry(location, position, tile);
            // Map the grid location to this tile entry.
            locationToEntry[location] = entry;
            // Map the world position to this tile entry.
            positionToEntry[position] = entry;
        }

        /// <summary>
        /// Adds a TileInstance to the map using its inherent location and position properties.
        /// </summary>
        /// <param name="tile">The TileInstance to add.</param>
        public void Add(TileInstance tile)
        {
            // Retrieve the location and position directly from the TileInstance.
            var location = tile.location;
            var position = tile.position;
            // Delegate to the other Add method.
            Add(location, position, tile);
        }

        /// <summary>
        /// Retrieves the world position associated with a given grid location.
        /// If the location is not found, calculates the position using a fallback Geometry method.
        /// </summary>
        /// <param name="location">The grid coordinates to look up.</param>
        /// <returns>The corresponding world position.</returns>
        public Vector3 GetPosition(Vector2Int location)
        {
            return locationToEntry.TryGetValue(location, out var entry)
                ? entry.Position
                : Geometry.GetPositionByLocation(location);
        }

        /// <summary>
        /// Retrieves the grid location corresponding to a given world position.
        /// If the position is not found, calculates the location using a fallback Geometry method.
        /// </summary>
        /// <param name="position">The world position to look up.</param>
        /// <returns>The corresponding grid coordinates.</returns>
        public Vector2Int GetLocation(Vector3 position)
        {
            return positionToEntry.TryGetValue(position, out var entry)
                ? entry.Location
                : Geometry.GetLocationByPosition(position);
        }

        /// <summary>
        /// Retrieves the grid location for the specified column and row.
        /// Throws an exception if no tile exists at the specified coordinates.
        /// </summary>
        /// <param name="col">The column index.</param>
        /// <param name="row">The row index.</param>
        /// <returns>The grid location as a Vector2Int.</returns>
        public Vector2Int GetLocation(int col, int row)
        {
            var location = new Vector2Int(col, row);
            if (!locationToEntry.ContainsKey(location))
                throw new UnityException($"No tile found at column {col}, row {row}");
            return location;
        }

        /// <summary>
        /// Retrieves the TileInstance at the specified grid location.
        /// Returns null if no tile is found.
        /// </summary>
        /// <param name="location">Grid coordinates of the tile.</param>
        /// <returns>The TileInstance at the location, or null if not found.</returns>
        public TileInstance GetTile(Vector2Int location)
        {
            return locationToEntry.TryGetValue(location, out var entry) ? entry.Tile : null;
        }

        /// <summary>
        /// Retrieves the TileInstance at the specified world position.
        /// Returns null if no tile is found.
        /// </summary>
        /// <param name="position">World space position of the tile.</param>
        /// <returns>The TileInstance at the position, or null if not found.</returns>
        public TileInstance GetTile(Vector3 position)
        {
            return positionToEntry.TryGetValue(position, out var entry) ? entry.Tile : null;
        }

        /// <summary>
        /// Finds the tile closest to the given world position using an efficient grid conversion.
        /// It calculates the relative grid coordinates and retrieves the corresponding tile.
        /// </summary>
        /// <param name="position">The world position to search near.</param>
        /// <returns>The closest TileInstance, or null if not found.</returns>
        public TileInstance GetClosestTileEfficient(Vector3 position)
        {
            // Convert the world position to grid space by calculating relative offsets from gridOrigin.
            float relativeX = (position.x - gridOrigin.x) / tileSize;
            float relativeY = (gridOrigin.y - position.y) / tileSize; // Invert Y due to coordinate system differences.
            // Round the relative coordinates to the nearest integer and adjust for 1-indexed grid.
            int x = Mathf.RoundToInt(relativeX) + 1;
            int y = Mathf.RoundToInt(relativeY) + 1;
            Vector2Int tileLocation = new Vector2Int(x, y);

            // Optional: Debug output to verify conversion.
            // Debug.Log($"World Pos: {position} → Tile Location: {tileLocation}");

            return GetTile(tileLocation);
        }

        /// <summary>
        /// Checks if the TileMap contains a tile at the specified grid location.
        /// </summary>
        /// <param name="location">The grid coordinates to check.</param>
        /// <returns>True if a tile exists at the location; otherwise, false.</returns>
        public bool ContainsLocation(Vector2Int location)
        {
            return locationToEntry.ContainsKey(location);
        }

        /// <summary>
        /// Checks if the TileMap contains a tile at the specified world position.
        /// </summary>
        /// <param name="position">The world position to check.</param>
        /// <returns>True if a tile exists at the position; otherwise, false.</returns>
        public bool ContainsPosition(Vector3 position)
        {
            return positionToEntry.ContainsKey(position);
        }

        /// <summary>
        /// Removes a tile from the TileMap based on its grid location.
        /// The corresponding entry is removed from both dictionaries.
        /// </summary>
        /// <param name="location">The grid coordinates of the tile to remove.</param>
        public void RemoveByLocation(Vector2Int location)
        {
            if (locationToEntry.TryGetValue(location, out var entry))
            {
                locationToEntry.Remove(location);
                positionToEntry.Remove(entry.Position);
            }
        }

        /// <summary>
        /// Removes a tile from the TileMap based on its world position.
        /// The corresponding entry is removed from both dictionaries.
        /// </summary>
        /// <param name="position">The world position of the tile to remove.</param>
        public void RemoveByPosition(Vector3 position)
        {
            if (positionToEntry.TryGetValue(position, out var entry))
            {
                positionToEntry.Remove(position);
                locationToEntry.Remove(entry.Location);
            }
        }

        /// <summary>
        /// Private class representing an entry in the TileMap.
        /// It holds the grid location, world position, and associated TileInstance.
        /// </summary>
        private class TileEntry
        {
            public Vector2Int Location { get; }
            public Vector3 Position { get; }
            public TileInstance Tile { get; }

            /// <summary>
            /// Constructs a new TileEntry with the specified grid location, world position, and tile instance.
            /// </summary>
            /// <param name="location">Grid coordinates for the tile.</param>
            /// <param name="position">World space position for the tile.</param>
            /// <param name="tile">The associated TileInstance.</param>
            public TileEntry(Vector2Int location, Vector3 position, TileInstance tile)
            {
                Location = location;
                Position = position;
                Tile = tile;
            }
        }
    }
}
