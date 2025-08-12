using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Game.Models.Profile
{
    [Serializable]
    public class Profile
    {
        // Fields
        public string Key;
        public string Folder;
        public List<SaveState> SaveStates = new List<SaveState>();
        public ProfileSettings Settings;
        public SaveState CurrentSave;

        // Properties
        public bool HasSaves => SaveStates.Any();
        public bool HasCurrentSave => CurrentSave != null;
        public SaveState LatestSave => HasSaves ? SaveStates.OrderByDescending(x => x.Timestamp).First() : null;

        public Profile() { }
    }

    [Serializable]
    public class ProfileSettings
    {
        public float ActorPanMultiplier;
        public float GameFocus;

        public ProfileSettings() { }

        public ProfileSettings(ProfileSettings other)
        {
            this.ActorPanMultiplier = other.ActorPanMultiplier;
            this.GameFocus = other.GameFocus;
        }
    }

    [Serializable]
    public class SaveState
    {
        //public string Folder;
        public string FileName;
        public DateTime Timestamp;
        public int Index;
        public GlobalSaveData Global;
        public StageSaveData Stage;
        public RosterSaveData Roster;
        public PartySaveData Party;

        public SaveState() { }

        public SaveState(int index, DateTime timestamp, 
            GlobalSaveData global, 
            StageSaveData stage, 
            RosterSaveData roster,
            PartySaveData party)
        {
            Index = index;
            Timestamp = timestamp;
            //Folder = Path.Combine(ProfileHelper.CurrentProfile.Folder, "Saves");
            FileName = $"Save{index:D3}.json";
            Global = global;
            Stage = stage;
            Roster = roster;
            Party = party;
        }

        //Copy constructor
        public SaveState(SaveState other)
        {
            this.Index = other.Index;
            this.Timestamp = other.Timestamp;
            //this.Folder = other.Folder;
            this.FileName = other.FileName;
            this.Global = new GlobalSaveData(other.Global);
            this.Stage = new StageSaveData(other.Stage);
            this.Roster = new RosterSaveData(other.Roster);
            this.Party = new PartySaveData(other.Party);
        }
    }

    [Serializable]
    public class GlobalSaveData
    {
        public int TotalCoins;

        public GlobalSaveData() { }
        public GlobalSaveData(GlobalSaveData other)
        {
            this.TotalCoins = other.TotalCoins;
        }
    }

    [Serializable]
    public class StageSaveData
    {
        public string CurrentStage;
        public int CurrentWave;

        public StageSaveData() { }
        public StageSaveData(StageSaveData other)
        {
            this.CurrentStage = other.CurrentStage;
            this.CurrentWave = other.CurrentWave;
        }
    }

    [Serializable]
    public class RosterSaveData
    {
        public List<CharacterLevelPair> Members = new List<CharacterLevelPair>();

        public RosterSaveData() { }

        public RosterSaveData(RosterSaveData other)
        {
            this.Members = other.Members;
        }
    }


    [Serializable]
    public class PartySaveData
    {
        public List<CharacterLevelPair> Members = new List<CharacterLevelPair>();

        public PartySaveData() { }

        public PartySaveData(PartySaveData other)
        {
            this.Members = other.Members;
        }
    }


    [Serializable]
    public class CharacterLevelPair
    {
        public string Character;
        public int Level;
        public CharacterLevelPair() { }
        public CharacterLevelPair(string character, int level = 1)
        {
            Character = character;
            Level = level;
        }
    }


}
