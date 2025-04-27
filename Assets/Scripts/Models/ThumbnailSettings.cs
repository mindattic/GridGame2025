using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ThumbnailSettings
    {
        public Vector2 Position;
        public Vector2 Scale;
        public Vector2 Range;

        public ThumbnailSettings() { }

        public ThumbnailSettings(Vector2 scale, Vector2 position)
        {
            Scale = scale;
            Position = position;
        }

        public ThumbnailSettings(ThumbnailSettings other)
        {
            Scale = other.Scale;
            Position = other.Position;

        }
    }
}