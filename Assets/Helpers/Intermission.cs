namespace Assets.Helper
{

    public static class Intermission
    {
        public static class Before
        {

            public static class Enemy
            {
                public static float Move = 0;
                public static float Attack = 0;
            }

            public static class Player
            {
                public static float Attack = 0;
            }

            public static class Portrait
            {
                public static float SlideIn = 0;
            }

            public static class HealthBar
            {
                public static float Drain = Interval.OneSecond;
            }

            public static class ActionBar
            {
                public static float Drain = 0;
            }


        }

        public static class After
        {
            public static class Player
            {
                public static float Attack = 0;
            }

            public static class HealthBar
            {
                public static float Empty = 0;
            }

        }

    }


}