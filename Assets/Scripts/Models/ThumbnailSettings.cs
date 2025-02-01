using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ThumbnailSettings
    {
        //Fields
        public int Width;
        public int Height;
        public int OffsetX;
        public int OffsetY;

        public ThumbnailSettings() { }

        public ThumbnailSettings(ThumbnailSettings other) {
            Width = other.Width;
            Height = other.Height;
            OffsetX = other.OffsetX;
            OffsetY = other.OffsetY;
        }
    }
}
