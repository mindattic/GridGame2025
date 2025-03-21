namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ThumbnailSettings
    {
        //Fields
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public ThumbnailSettings() { }

        public ThumbnailSettings(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public ThumbnailSettings(ThumbnailSettings other)
        {
            X = other.X;
            Y = other.Y;
            Width = other.Width;
            Height = other.Height;
        }
    }
}
