using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using g = Assets.Helpers.GameHelper;

namespace Assets.Helpers
{
    public static class CharacterHelper
    {

        //TODO: Put this into an Enum??
        public const string Barbarian = "Barbarian";
        public const string Bat = "Bat";
        public const string Cleric = "Cleric";
        public const string Captain00 = "Captain00";
        public const string GreenNinja = "GreenNinja";
        public const string Paladin = "Paladin";
        public const string PandaGirl = "PandaGirl";
        public const string Pugilist = "Pugilist";
        public const string RedNinja = "RedNinja";
        public const string Ronin = "Ronin";
        public const string Sellsword = "Sellsword";
        public const string Scorpion = "Scorpion";
        public const string Soldier00 = "Soldier00";
        public const string Soldier01 = "Soldier02";
        public const string Soldier02 = "Soldier01";
        public const string Soldier03 = "Soldier03";
        public const string Slime = "Slime";
        public const string Thief = "Thief";
        public const string Vampire = "Vampire";
        public const string Yeti = "Yeti";


        public static IReadOnlyList<string> AllCharacters { get; } = new[]
        {
            Barbarian,
            Bat,
            Cleric,
            Captain00,
            GreenNinja,
            Paladin,
            PandaGirl,
            Pugilist,
            RedNinja,
            Ronin,
            Sellsword,
            Scorpion,
            Soldier00,
            Soldier01,
            Soldier02,
            Soldier03,
            Slime,
            Thief,
            Vampire,
            Yeti
        };
    }



}
