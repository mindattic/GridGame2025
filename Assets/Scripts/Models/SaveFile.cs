using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{

    // The SaveFile holds a complete snapshot of the game state.
    [Serializable]
    public class SaveFile
    {
        public string Folder;      // (Optional) The folder where this save resides.
        public string FileName;    // e.g. "2025.02.16.09.07.33.json"
        public DateTime Timestamp;

        public GlobalSection Global;
        public SettingsSection Settings;
        public StageSection Stage;
        public PartySection Party;

        // Create a new SaveFile with the provided game state.
        public SaveFile(string fileName, DateTime timestamp, GlobalSection global, SettingsSection settings, StageSection stage, PartySection party)
        {
            FileName = fileName;
            Timestamp = timestamp;
            Global = global;
            Settings = settings;
            Stage = stage;
            Party = party;
        }
    }

    [Serializable]
    public class GlobalSection
    {
        public int TotalCoins;
        public GlobalSection()
        {
        }
    }

    [Serializable]
    public class SettingsSection
    {
        public float GameSpeed;

        public SettingsSection()
        {
        }
    }

    [Serializable]
    public class StageSection 
    {
        public string CurrentStageName;

        public StageSection()
        {
        }
    }

    [Serializable]
    public class PartySection
    {
        public List<StageActor> PlayerActors;

        public PartySection()
        {
            PlayerActors = new List<StageActor>();
        }
    }

}
