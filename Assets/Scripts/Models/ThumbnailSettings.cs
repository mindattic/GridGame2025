using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ThumbnailSettings
    {
        public Vector2 Position;
        public Vector2 Scale;

        public ThumbnailSettings() { }

        public ThumbnailSettings(Vector2 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
          
        }

        //Copy Constructor
        public ThumbnailSettings(ThumbnailSettings other)
        {
            Position = other.Position;
            Scale = other.Scale;
        }
    }
}