using Assets.Data.Actor;
using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Libraries
{
    public static class ActorLibrary
    {
        private static Dictionary<string, ActorData> actors;
        private static bool isLoaded = false;

        public static Dictionary<string, ActorData> Actors
        {
            get
            {
                if (!isLoaded)
                    Load();
                return actors;
            }
        }

        public static ActorData Get(string key)
        {
            if (!isLoaded)
                Load();

            if (string.IsNullOrEmpty(key)) return null;
            return actors.TryGetValue(key, out ActorData data) ? data : null;
        }

        private static void Load()
        {
            if (isLoaded) return;
            actors = new Dictionary<string, ActorData>
            {
                { CharacterClass.Barbarian, Barbarian.Data() },
                { CharacterClass.Bat, Bat.Data() },
                { CharacterClass.Captain00, Captain.Data() },
                { CharacterClass.Cleric, Cleric.Data() },
                { CharacterClass.GreenNinja, GreenNinja.Data() },
                { CharacterClass.Paladin, Paladin.Data() },
                { CharacterClass.Pugilist, Pugilist.Data() },
                { CharacterClass.RedNinja, RedNinja.Data() },
                { CharacterClass.Ronin, Ronin.Data() },
                { CharacterClass.Scorpion, Scorpion.Data() },
                { CharacterClass.Sellsword, Sellsword.Data() },
                { CharacterClass.Slime, Slime.Data() },
                { CharacterClass.Slime00, Slime00.Data() },
                { CharacterClass.Slime01, Slime01.Data() },
                { CharacterClass.Slime02, Slime02.Data() },
                { CharacterClass.Slime03, Slime03.Data() },
                { CharacterClass.Soldier00, Soldier00.Data() },
                { CharacterClass.Soldier01, Soldier01.Data() },
                { CharacterClass.Soldier02, Soldier02.Data() },
                { CharacterClass.Soldier03, Soldier03.Data() },
                { CharacterClass.Thief, Thief.Data() },
                { CharacterClass.Wolf00, Wolf00.Data() },
                { CharacterClass.Wolf01, Wolf01.Data() },
                { CharacterClass.Wolf02, Wolf02.Data() },
                { CharacterClass.Wolf03, Wolf03.Data() },
                { CharacterClass.Vampire, Vampire.Data() },
                { CharacterClass.Yeti, Yeti.Data() }
            };
            isLoaded = true;
        }
    }
}
