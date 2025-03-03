﻿using System.Collections.Generic;

namespace Game.Instances.Actor
{
    public static class ActorLayer
    {
        //Define names for all layers
        public static class Name
        {
            public const string Front = "Front";
            public const string Back = "Back";



            public const string Opaque = "Opaque";
            public const string Quality = "Quality";
            public const string Glow = "Glow";
            public const string Parallax = "Parallax";
            public const string Thumbnail = "Thumbnail";
            public const string Frame = "Frame";
            public const string StatusIcon = "StatusIcon";

            //HealthBar Sub-Objects
            public static class HealthBar
            {
                public const string Root = "HealthBar";
                public const string Back = "HealthBarBack";
                public const string Drain = "HealthBarDrain";
                public const string Fill = "HealthBarFill";
                public const string Text = "HealthBarText";
            }

            //ActionBar Sub-Objects
            public static class ActionBar
            {
                public const string Root = "ActionBar";
                public const string Back = "ActionBarBack";
                public const string Drain = "ActionBarDrain";
                public const string Fill = "ActionBarFill";
                public const string Text = "ActionBarText";
            }

            public const string Mask = "Mask";
            public const string RadialBack = "RadialBack";
            public const string RadialFill = "RadialFill";
            public const string RadialText = "RadialText";
            public const string TurnDelayText = "TurnDelayText";
            public const string NameTagText = "NameTagText";
            public const string WeaponIcon = "WeaponIcon";

            //Armor Sub-Objects
            public static class Armor
            {
                public const string Root = "Armor";
                public const string ArmorNorth = "ArmorNorth";
                public const string ArmorEast = "ArmorEast";
                public const string ArmorSouth = "ArmorSouth";
                public const string ArmorWest = "ArmorWest";
            }
        }

        public static class Value
        {
            public const int Opaque = 1;
            public const int Quality = 2;
            public const int Glow = 3;
            public const int Parallax = 4;
            public const int Thumbnail = 5;
            public const int Frame = 6;
            public const int StatusIcon = 7;

            public static class HealthBar
            {
                public const int Back = 8;  
                public const int Drain = 9;
                public const int Fill = 10;
                public const int Text = 11;
            }

            public static class ActionBar
            {
                public const int Back = 12;
                public const int Drain = 13;
                public const int Fill = 14;
                public const int Text = 15;
            }

            public const int Mask = 16;
            public const int RadialBack = 17;
            public const int RadialFill = 18;
            public const int RadialText = 19;
            public const int TurnDelayText = 20;
            public const int NameTagText = 21;
            public const int WeaponIcon = 22;
           

            public static class Armor
            {
                public const int ArmorNorth = 23;
                public const int ArmorEast = 24;
                public const int ArmorSouth = 25;
                public const int ArmorWest = 26;
            }

        }

    }
}
