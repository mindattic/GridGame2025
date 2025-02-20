using System;
using System.Collections.Generic;
using static FolderHelper;
using Global = Game.Models.ProfileGlobalSection;
using Party = Game.Models.ProfilePartySection;
using Stage = Game.Models.ProfileStageSection;

namespace Game.Models
{
    [Serializable]
    public class Profile
    {
        public string Guid;
        public string Folder;

        public Global Global { get; set; }
        public Stage Stage { get; set; }
        public Party Party { get; set; }

        public Profile() { }

        public Profile(string guid)
        {
            Guid = guid;

            Folder = CreateFolder(Folders.Profiles, Guid);

            Global = new Global
            {
                TotalCoins = 0
            };

            Stage = new Stage
            {
                CurrentStageName = "Stage 1"
            };

            Party = new Party();
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

        public ProfileGlobalSection()
        {
            TotalCoins = 0;
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
