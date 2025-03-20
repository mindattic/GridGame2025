namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ThumbnailSettings
    {
        //Fields
        public int OffsetX;
        public int OffsetY;
        public int Width;
        public int Height;

        public ThumbnailSettings() { }

        public ThumbnailSettings(int offsetX, int offsetY, int width, int height)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Width = width;
            Height = height;
        }

        public ThumbnailSettings(ThumbnailSettings other)
        {
            OffsetX = other.OffsetX;
            OffsetY = other.OffsetY;
            Width = other.Width;
            Height = other.Height;
        }
    }
}
