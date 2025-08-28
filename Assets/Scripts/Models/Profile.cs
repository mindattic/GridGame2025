using System;
using System.Collections.Generic;

// Add this definition if ProfileSettings does not exist elsewhere
[Serializable]
public class ProfileSettings
{
    public float ActorPanMultiplier;
    public float GameSpeed;

    public ProfileSettings() { }

    public ProfileSettings(ProfileSettings other)
    {
        this.ActorPanMultiplier = other.ActorPanMultiplier;
        this.GameSpeed = other.GameSpeed;
    }
}

namespace Game.Models.Profile
{
    [Serializable]
    public class Profile
    {
        public string Key;
        public string Folder;
        public List<SaveState> SaveStates;
        public SaveState CurrentSave;
        public bool HasSaves => SaveStates != null && SaveStates.Count > 0;
        public SaveState LatestSave => HasSaves ? SaveStates[0] : null;
        public ProfileSettings Settings;
    }

    [Serializable]
    public class SaveState
    {
        public int Index;
        public DateTime Timestamp;
        public string FileName;
        public GlobalSaveData Global;
        public StageSaveData Stage;
        public RosterSaveData Roster;
        public PartySaveData Party;

        public SaveState() { }
        public SaveState(SaveState other)
        {
            this.Index = other.Index;
            this.Timestamp = other.Timestamp;
            this.FileName = other.FileName;
            this.Global = new GlobalSaveData(other.Global);
            this.Stage = new StageSaveData(other.Stage);
            this.Roster = new RosterSaveData(other.Roster);
            this.Party = new PartySaveData(other.Party);
        }
        public SaveState(int index, DateTime timestamp, GlobalSaveData global, StageSaveData stage, RosterSaveData roster, PartySaveData party)
        {
            Index = index;
            Timestamp = timestamp;
            FileName = $"Save_{timestamp:yyyyMMdd_HHmmss}.json";
            Global = global;
            Stage = stage;
            Roster = roster;
            Party = party;
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
        public RosterSaveData(RosterSaveData other) { this.Members = other.Members; }
    }

    [Serializable]
    public class PartySaveData
    {
        public List<CharacterLevelPair> Members = new List<CharacterLevelPair>();

        public PartySaveData() { }
        public PartySaveData(PartySaveData other) { this.Members = other.Members; }
    }

    [Serializable]
    public class CharacterLevelPair
    {
        public string Character;
        public int Level;

        // New: per-hero XP persistence
        public int CurrentXP;
        public int TotalXP;

        public CharacterLevelPair() { }
        public CharacterLevelPair(string character, int level = 1, int currentXP = 0, int totalXP = 0)
        {
            Character = character;
            Level = level;
            CurrentXP = currentXP;
            TotalXP = totalXP;
        }
    }
}
