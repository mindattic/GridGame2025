using UnityEngine;

namespace Assets.Helper
{

    public static class ColorHelper
    {
        public static Color RGB(float r, float g, float b)
        {
            return new Color(
                Mathf.Clamp(r, 0, 255) / 255,
                Mathf.Clamp(g, 0, 255) / 255,
                Mathf.Clamp(b, 0, 255) / 255,
                255 / 255);
        }

        public static Color RGBA(float r, float g, float b, float a)
        {
            return new Color(
                Mathf.Clamp(r, 0, 255) / 255,
                Mathf.Clamp(g, 0, 255) / 255,
                Mathf.Clamp(b, 0, 255) / 255,
                Mathf.Clamp(a, 0, 255) / 255);
        }

        public static class Solid
        {
            public static Color Gold = RGB(255, 215, 0);
            public static Color Black = RGB(0, 0, 0);
            public static Color Gray = RGB(128, 128, 128);
            public static Color White = RGB(255, 255, 255);
            public static Color LightBlue = RGB(128, 128, 255);
            public static Color LightRed = RGB(255, 128, 128);
            public static Color Red = RGB(255, 0, 0);
            public static Color Green = RGB(0, 255, 0);
            public static Color GunMetal = RGB(42, 52, 57);
            public static Color Yellow = RGB(255, 255, 0);
        }

        public static class HealthBar
        {
            public static Color Green = RGB(0, 255, 0);
            public static Color Red = RGB(255, 0, 0);
            public static Color Yellow = RGB(255, 255, 0);
        }

        public static class ActionBar
        {
            public static Color Blue = RGB(0, 196, 255);
            public static Color Yellow = Color.yellow;
            public static Color Pink = RGB(100, 75, 80);
            public static Color White = Color.white;
        }

        public static class Translucent
        {
            public static Color Gold = RGBA(255, 215, 0, 128);
            public static Color White = RGBA(255, 255, 255, 128);
            public static Color Black = RGBA(0, 0, 0, 128);
            public static Color DarkBlack = RGBA(0, 0, 0, 196);
            public static Color LightBlue = RGBA(128, 128, 255, 128);
            public static Color LightRed = RGBA(255, 128, 128, 128);
            public static Color Red = RGBA(255, 0, 0, 128);
            public static Color Green = RGBA(0, 255, 0, 128);
            public static Color Yellow = RGBA(255, 255, 0, 128);
            public static Color GunMetal = RGBA(42, 52, 57, 128);
        }

        public static class Transparent
        {
            public static Color White = RGBA(255, 255, 255, 0);
            public static Color Red = RGBA(255, 0, 0, 0);
        }

        public static class Tile
        {
            public static Color White = RGBA(255, 255, 255, 96);
            public static Color Yellow = RGBA(255, 255, 0, 96);
        }
    }

}