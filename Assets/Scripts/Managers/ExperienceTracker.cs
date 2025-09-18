using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    // Holds per-battle XP gains and participants, persisted across scene loads until consumed by VictoryScreen.
    public static class ExperienceTracker
    {
        public class Entry
        {
            public string Character;
            public int XPGained;
        }

        private static readonly Dictionary<string, int> characterXP = new Dictionary<string, int>();
        private static readonly HashSet<string> participants = new HashSet<string>();

        public static string NextSceneAfterVictory = Assets.Helpers.SceneHelper.Overworld; // configurable default

        public static void StartSession(IEnumerable<string> participantCharacters)
        {
            characterXP.Clear();
            participants.Clear();
            if (participantCharacters != null)
            {
                foreach (var c in participantCharacters)
                {
                    if (!string.IsNullOrEmpty(c)) participants.Add(c);
                }
            }
        }

        public static void AddParticipant(string character)
        {
            if (string.IsNullOrEmpty(character)) return;
            participants.Add(character);
        }

        public static void AddXP(string character, int amount)
        {
            if (string.IsNullOrEmpty(character) || amount <= 0) return;
            if (characterXP.TryGetValue(character, out var cur))
                characterXP[character] = cur + amount;
            else
                characterXP[character] = amount;
        }

        public static int GetXPGained(string character)
        {
            if (string.IsNullOrEmpty(character)) return 0;
            return characterXP.TryGetValue(character, out var v) ? v : 0;
        }

        public static IReadOnlyDictionary<string, int> AllGains => characterXP;
        public static IReadOnlyCollection<string> Participants => participants;

        public static void Clear()
        {
            characterXP.Clear();
            participants.Clear();
        }
    }
}
