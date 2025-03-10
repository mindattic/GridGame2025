using System;
using System.Collections.Generic;
using static FolderHelper;
using GlobalSection = Game.Models.ProfileGlobalSection;
using SettingsSection = Game.Models.ProfileSettingsSection;
using PartySection = Game.Models.ProfilePartySection;
using StageSection = Game.Models.ProfileStageSection;

namespace Game.Models
{
    [Serializable]
    public class Profile
    {
        public string Guid;
        public string Folder;

        public GlobalSection Global { get; set; }
        public SettingsSection Settings { get; set; }
        public StageSection Stage { get; set; }
        public PartySection Party { get; set; }

        public Profile() { }

        public Profile(string guid)
        {
            Guid = guid;

            Folder = CreateFolder(Folders.Profiles, Guid);

            Global = new GlobalSection
            {
                TotalCoins = 0,
                PreviousSceneName = "Title"
            };

            Settings = new SettingsSection
            {

                GameSpeed = 1.0f
            };


            Stage = new StageSection
            {
                CurrentStageName = "Stage 1"
            };

            Party = new PartySection();
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Guid) || string.IsNullOrWhiteSpace(Folder))
                return false;

            if (Global == null || Stage == null || Party == null)
                return false;

            return true;
        }

    }


    [Serializable]
    public class ProfileSection { }

    [Serializable]
    public class ProfileGlobalSection : ProfileSection
    {
        public int TotalCoins;
        public string PreviousSceneName;

        public ProfileGlobalSection()
        {
            TotalCoins = 0;
            PreviousSceneName = "Title";
        }
    }

    [Serializable]
    public class ProfileSettingsSection : ProfileSection
    {
        public float GameSpeed;

        public ProfileSettingsSection()
        {
            GameSpeed = 1.0f;
        }
    }

    [Serializable]
    public class ProfileStageSection : ProfileSection
    {
        public string CurrentStageName;

        public ProfileStageSection()
        {
            CurrentStageName = "Stage 1";
        }
    }

    [Serializable]
    public class ProfilePartySection : ProfileSection
    {
        public List<Member> Members = new List<Member>();

        public ProfilePartySection() { }

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
