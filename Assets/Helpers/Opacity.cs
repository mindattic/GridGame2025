namespace Assets.Helper
{



    public static class Opacity
    {
        // Standard opacity levels
        public const float Opaque = 1f;
        public const float Percent90 = 0.90f;
        public const float Percent80 = 0.80f;
        public const float Percent70 = 0.70f;
        public const float Percent60 = 0.60f;
        public const float Percent50 = 0.50f;
        public const float Percent40 = 0.40f;
        public const float Percent30 = 0.30f;
        public const float Percent20 = 0.20f;
        public const float Percent10 = 0.10f;
        public const float Transparent = 0f;

        // Opacity values based on byte alpha (0–255)
        public static class Translucent
        {
            public const float Alpha196 = 0.76862745f;
            public const float Alpha128 = 0.50196078f;
            public const float Alpha64 = 0.25098039f;
            public const float Alpha32 = 0.12549020f;
        }
    }
}