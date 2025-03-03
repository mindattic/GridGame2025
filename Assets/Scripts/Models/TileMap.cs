using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class TileMap
    {

        private Dictionary<Vector2Int, TileEntry> locationToEntry = new Dictionary<Vector2Int, TileEntry>();
        private Dictionary<Vector3, TileEntry> positionToEntry = new Dictionary<Vector3, TileEntry>();
        public Vector3 gridOrigin;
        public float tileSize;

        public void Add(Vector2Int location, Vector3 position, TileInstance tile)
        {
            var entry = new TileEntry(location, position, tile);
            locationToEntry[location] = entry;
            positionToEntry[position] = entry;
        }

        public void Add(TileInstance tile)
        {
            var location = tile.location;
            var position = tile.position;
            Add(location, position, tile);
        }

        public Vector3 GetPosition(Vector2Int location)
        {
            return locationToEntry.TryGetValue(location, out var entry) ? entry.Position : Geometry.GetPositionByLocation(location);
        }

        public Vector2Int GetLocation(Vector3 position)
        {
            return positionToEntry.TryGetValue(position, out var entry) ? entry.Location : Geometry.GetLocationByPosition(position);
        }

        public Vector2Int GetLocation(int col, int row)
        {
            var location = new Vector2Int(col, row);
            if (!locationToEntry.ContainsKey(location))
                throw new UnityException($"No tile found at column {col}, row {row}");

            return location;
        }

        public TileInstance GetTile(Vector2Int location)
        {
            return locationToEntry.TryGetValue(location, out var entry) ? entry.Tile : null;
        }

        public TileInstance GetTile(Vector3 position)
        {
            return positionToEntry.TryGetValue(position, out var entry) ? entry.Tile : null;
        }


        public TileInstance GetClosestTileEfficient(Vector3 position)
        {
            // Convert world position to grid space
            float relativeX = (position.x - gridOrigin.x) / tileSize;
            float relativeY = (gridOrigin.y - position.y) / tileSize; // Inverting Y
            int x = Mathf.RoundToInt(relativeX) + 1;
            int y = Mathf.RoundToInt(relativeY) + 1;
            Vector2Int tileLocation = new Vector2Int(x, y);

            // Debugging output
            //Debug.Log($"World Pos: {position} → Tile Location: {tileLocation}");

            return GetTile(tileLocation);
        }




        public bool ContainsLocation(Vector2Int location)
        {
            return locationToEntry.ContainsKey(location);
        }

        public bool ContainsPosition(Vector3 position)
        {
            return positionToEntry.ContainsKey(position);
        }

        public void RemoveByLocation(Vector2Int location)
        {
            if (locationToEntry.TryGetValue(location, out var entry))
            {
                locationToEntry.Remove(location);
                positionToEntry.Remove(entry.Position);
            }
        }

        public void RemoveByPosition(Vector3 position)
        {
            if (positionToEntry.TryGetValue(position, out var entry))
            {
                positionToEntry.Remove(position);
                locationToEntry.Remove(entry.Location);
            }
        }

        private class TileEntry
        {
            public Vector2Int Location { get; }
            public Vector3 Position { get; }
            public TileInstance Tile { get; }

            public TileEntry(Vector2Int location, Vector3 position, TileInstance tile)
            {
                Location = location;
                Position = position;
                Tile = tile;
            }
        }


    }

}
