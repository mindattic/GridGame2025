using System;
using System.Collections.Generic;

namespace Game.Models
{
    [Serializable]
    public class Profile
    {
        public string Key;
        public string Folder;

        public GlobalSection Global { get; set; }
        public SettingsSection Settings { get; set; }
        public StageSection Stage { get; set; }
        public PartySection Party { get; set; }

        public Profile() { }

        public Profile(string key)
        {
            Key = key;
            // Folder will be set later by ProfileStore when the folder is created
            Global = new GlobalSection();
            Settings = new SettingsSection();
            Stage = new StageSection();
            Party = new PartySection();
        }
    }

    [Serializable]
    public class ProfileSection { }

    [Serializable]
    public class GlobalSection : ProfileSection
    {
        public int TotalCoins;
        public string PreviousSceneName;
        public DateTime DateCreated;

        public GlobalSection()
        {
            TotalCoins = 0;
            PreviousSceneName = "Title";
            DateCreated = DateTime.UtcNow;
        }
    }

    [Serializable]
    public class SettingsSection : ProfileSection
    {
        public float GameSpeed;

        public SettingsSection()
        {
            GameSpeed = 1.0f;
        }
    }

    [Serializable]
    public class StageSection : ProfileSection
    {
        public string CurrentStageName;

        public StageSection()
        {
            CurrentStageName = "Stage 1";
        }
    }

    [Serializable]
    public class PartySection : ProfileSection
    {
        public List<Member> Members = new List<Member>();

        public PartySection() { }
    }

    [Serializable]
    public class Member
    {
        public string Name;
        public Character Character;
        public int Index = -1;
        public ActorStats Stats;
        //public ActorEquipment Equipment;

        public bool IsInParty => Index > 0;
    }
}
