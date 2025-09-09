    using UnityEngine;

    namespace Assets.Scripts.Models
    {
        [System.Serializable]
        public class CanvasThumbnailSettings
        {
            public float X;
            public float Y;
            public int Width;
            public int Height;

            public CanvasThumbnailSettings() { }

            public CanvasThumbnailSettings(float x, float y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            // Copy constructor
            public CanvasThumbnailSettings(CanvasThumbnailSettings other)
            {
                if (other == null) return;
                X = other.X;
                Y = other.Y;
                Width = other.Width;
                Height = other.Height;
            }

            /// <summary>
            /// Returns a square crop of size blockPixels taken from the top-center.
            /// Ignores the provided ThumbnailSettings.
            /// </summary>
            public static CanvasThumbnailSettings Generate(float blockPixels = 255f)
            {
                int edge = Mathf.RoundToInt(blockPixels);

                // X/Y are offsets used by the canvas code; 0,0 keeps the crop centered horizontally.
                // Height/Width define the crop size. Top-center presentation is handled by the canvas.
                return new CanvasThumbnailSettings(0f, 0f, edge, edge);
            }
        }
    }
