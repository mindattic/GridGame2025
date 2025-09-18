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
                { CharacterHelper.Barbarian, Barbarian.Data() },
                { CharacterHelper.Bat, Bat.Data() },
                { CharacterHelper.Captain00, Captain.Data() },
                { CharacterHelper.Cleric, Cleric.Data() },
                { CharacterHelper.GreenNinja, GreenNinja.Data() },
                { CharacterHelper.Paladin, Paladin.Data() },
                { CharacterHelper.Pugilist, Pugilist.Data() },
                { CharacterHelper.RedNinja, RedNinja.Data() },
                { CharacterHelper.Ronin, Ronin.Data() },
                { CharacterHelper.Scorpion, Scorpion.Data() },
                { CharacterHelper.Sellsword, Sellsword.Data() },
                { CharacterHelper.Slime, Slime.Data() },
                { CharacterHelper.Slime00, Slime00.Data() },
                { CharacterHelper.Slime01, Slime01.Data() },
                { CharacterHelper.Slime02, Slime02.Data() },
                { CharacterHelper.Slime03, Slime03.Data() },
                { CharacterHelper.Soldier00, Soldier00.Data() },
                { CharacterHelper.Soldier01, Soldier01.Data() },
                { CharacterHelper.Soldier02, Soldier02.Data() },
                { CharacterHelper.Soldier03, Soldier03.Data() },
                { CharacterHelper.Thief, Thief.Data() },
                { CharacterHelper.Wolf00, Wolf00.Data() },
                { CharacterHelper.Wolf01, Wolf01.Data() },
                { CharacterHelper.Wolf02, Wolf02.Data() },
                { CharacterHelper.Wolf03, Wolf03.Data() },
                { CharacterHelper.Vampire, Vampire.Data() },
                { CharacterHelper.Yeti, Yeti.Data() }
            };
            isLoaded = true;
        }
    }
}
