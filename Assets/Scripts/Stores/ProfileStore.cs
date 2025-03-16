using Assets.Scripts.Models;
using Assets.Scripts.Store;
using Game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ProfileStore", menuName = "Stores/ProfileStore")]
public class ProfileStore : ScriptableObject
{
    // Singleton instance
    private static ProfileStore _instance;
    public static ProfileStore instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("ProfileStore accessed before being initialized! Ensure it's assigned in Awake().");
            return _instance;
        }
    }

    // Auto-initialize before the scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<ProfileStore>("Stores/ProfileStore");
            if (_instance == null)
                Debug.LogError("ProfileStore asset not found in Resources/Stores/ProfileStore");
        }
    }

    // When accessing PlayerActors, we use the latest save file in the currently selected profile.
    public List<StageActor> PlayerActors
    {
        get
        {
            if (CurrentProfile != null &&
                CurrentProfile.SaveFiles != null &&
                CurrentProfile.SaveFiles.Count > 0)
            {
                var latestSave = CurrentProfile.SaveFiles.OrderBy(s => s.Timestamp).Last();
                return latestSave.Party.PlayerActors;
            }
            return new List<StageActor>();
        }
    }

    public Dictionary<string, Profile> profiles;
    public string selectedKey;
    private List<string> folders;

    public bool HasFolders => folders != null && folders.Count > 0;
    public bool HasProfiles => profiles != null && profiles.Count > 0;
    public bool HasSelectedKey => !string.IsNullOrWhiteSpace(selectedKey);
    public bool HasSelectedProfile => HasProfiles && HasSelectedKey && profiles.ContainsKey(selectedKey);
    public Profile CurrentProfile => HasSelectedProfile ? profiles[selectedKey] : null;

    private void OnEnable()
    {
        Load();
    }

    /// <summary>
    /// Loads all profiles from the profiles folder.
    /// </summary>
    public bool Load()
    {
        if (!HasValidFolderStructure)
        {
            Debug.LogError($"Folder structure is invalid `{FolderHelper.Folder.Profiles}`");
            return false;
        }

        GetFolders();
        if (!HasFolders)
        {
            Debug.LogError("No folders found");
            return false;
        }

        PopulateProfiles();
        if (!HasProfiles)
        {
            Debug.LogError("No profiles found");
            return false;
        }

        // Auto-select a profile if one is not already selected.
        var success = HasSelectedKey ? Select(selectedKey) : Select(profiles.Keys.First());
        return success;
    }

    private bool HasValidFolderStructure
    {
        get
        {
            if (!Directory.Exists(FolderHelper.Folder.Profiles))
                Directory.CreateDirectory(FolderHelper.Folder.Profiles);
            return Directory.Exists(FolderHelper.Folder.Profiles);
        }
    }

    private void GetFolders()
    {
        folders = Directory.GetDirectories(FolderHelper.Folder.Profiles).ToList();
        if (folders == null || folders.Count < 1)
        {
            // Create a new profile if none exist.
            Create();
            folders = Directory.GetDirectories(FolderHelper.Folder.Profiles).ToList();
        }
    }

    public void PopulateProfiles()
    {
        profiles = new Dictionary<string, Profile>();
        foreach (var folder in folders)
        {
            string key = new DirectoryInfo(folder).Name;
            var profile = Get(key);
            if (profile != null)
                profiles.Add(key, profile);
        }
    }

    /// <summary>
    /// Creates a new profile folder and initializes it with an initial SaveFile.
    /// </summary>
    public string Create()
    {
        // Generate a unique key and folder path.
        string key;
        string folder;
        do
        {
            key = Guid.NewGuid().ToString("N");
            folder = Path.Combine(FolderHelper.Folder.Profiles, key);
        } while (Directory.Exists(folder));

        Directory.CreateDirectory(folder);

        // Create a new profile (with no game state directly on the profile).
        Profile newProfile = new Profile(key)
        {
            Folder = folder,
            SaveFiles = new List<SaveFile>()
        };

        // Create the initial game state for a new game.
        GlobalSection initialGlobal = new GlobalSection() {
            TotalCoins = 0
        };
        SettingsSection initialSettings = new SettingsSection()
        {
            GameSpeed = 1.0f
        };
        StageSection initialStage = new StageSection() {
            CurrentStageName = "Stage 1"
        };
        PartySection initialParty = new PartySection()
        {
            PlayerActors = new List<StageActor>
            {
                new StageActor { Character = Character.Paladin, Team = Team.Player },
                new StageActor { Character = Character.Barbarian, Team = Team.Player },
                new StageActor { Character = Character.Cleric, Team = Team.Player },
            }
        };

        // Create the initial SaveFile.
        var utcNow = DateTime.UtcNow;
        string fileName = utcNow.ToString("yyyy.MM.dd.HH.mm.ss") + ".json";
        SaveFile initialSave = new SaveFile(fileName, utcNow, initialGlobal, initialSettings, initialStage, initialParty);
        newProfile.SaveFiles.Add(initialSave);

        profiles ??= new Dictionary<string, Profile>();
        profiles.Add(key, newProfile);
        selectedKey = key;

        //Write the initial SaveFile
        string filePath = Path.Combine(newProfile.Folder, fileName);
        string json = JsonConvert.SerializeObject(initialSave, Formatting.Indented);
        try
        {
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing file {filePath}: {ex.Message}");
            return null;
        }

        return key;
    }

    /// <summary>
    /// Saves the current game state as a new SaveFile within the selected profile.
    /// </summary>
    public bool Save()
    {
        if (!HasSelectedProfile)
        {
            Debug.LogError("No valid profile selected for saving.");
            return false;
        }

        if (!SaveProfile(CurrentProfile))
        {
            Debug.LogError("Failed to save profile.");
            return false;
        }
        return true;
    }

    // Creates a new SaveFile from the current game state and writes it to disk.
    private bool SaveProfile(Profile profile)
    {
        if (profile == null)
        {
            Debug.LogError("Profile is null.");
            return false;
        }
        // Generate a new filename based on the current UTC time.

        DateTime utcNow = DateTime.UtcNow;
        string fileName = utcNow.ToString("yyyy.MM.dd.HH.mm.ss") + ".json";
        string filePath = Path.Combine(profile.Folder, fileName);

        // In a real game you would capture the actual current game state.
        // For this example, we simulate it by reusing the state from the latest save (or default values if none exist).
        GlobalSection currentGlobal;
        SettingsSection currentSettings;
        StageSection currentStage;
        PartySection currentParty;

        if (profile.SaveFiles != null && profile.SaveFiles.Count > 0)
        {
            // For demonstration, copy the state from the latest save.
            var latestSave = profile.SaveFiles.OrderBy(x => x.Timestamp).Last();
            currentGlobal = latestSave.Global;
            currentSettings = latestSave.Settings;
            currentStage = latestSave.Stage;
            currentParty = latestSave.Party;
        }
        else
        {
            // Use defaults if no save exists.
            currentGlobal = new GlobalSection();
            currentSettings = new SettingsSection();
            currentStage = new StageSection();
            currentParty = new PartySection()
            {
                PlayerActors = new List<StageActor>
                {
                    new StageActor { Character = Character.Paladin, Team = Team.Player },
                    new StageActor { Character = Character.Barbarian, Team = Team.Player },
                    new StageActor { Character = Character.Cleric, Team = Team.Player },
                    new StageActor { Character = Character.Ninja, Team = Team.Player }
                }
            };
        }

        // Create a new SaveFile capturing the current state.
        SaveFile newSave = new SaveFile(fileName, utcNow, currentGlobal, currentSettings, currentStage, currentParty);

        string json = JsonConvert.SerializeObject(newSave, Formatting.Indented);
        try
        {
            File.WriteAllText(filePath, json);
            if (profile.SaveFiles == null)
                profile.SaveFiles = new List<SaveFile>();
            profile.SaveFiles.Add(newSave);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing file {filePath}: {ex.Message}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Loads a profile by reading the newest JSON save file in its folder.
    /// </summary>
    public Profile Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"Invalid key specified: {key}");
            return null;
        }
        return LoadProfile(key);
    }

    public bool Delete(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"Invalid key specified: {key}");
            return false;
        }
        string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
        Load();
        return true;
    }

    public bool Select(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogError($"Invalid key specified: {name}");
            return false;
        }
        if (!profiles.ContainsKey(name))
        {
            Debug.LogError($"No profile matches key: {name}");
            return false;
        }
        selectedKey = name;
        return true;
    }

    // Loads a Profile by scanning its folder for JSON save files.
    // We sort the filenames (which are timestamps) in descending order and load only the newest one.
    private Profile LoadProfile(string name)
    {
        string folder = Path.Combine(FolderHelper.Folder.Profiles, name);
        var jsonFiles = Directory.GetFiles(folder, "*.json");

        // Create a new profile instance.
        Profile profile = new Profile(name) { Folder = folder, SaveFiles = new List<SaveFile>() };

        if (jsonFiles == null || jsonFiles.Length == 0)
        {
            // No save files exist – create an initial save.
            SaveProfile(profile);
            jsonFiles = Directory.GetFiles(folder, "*.json");
        }

        // Sort the files descending based on filename and pick the first one (newest).
        string latestFile = jsonFiles.OrderByDescending(f => f).First();

        try
        {
            string json = File.ReadAllText(latestFile);
            SaveFile latestSave = JsonConvert.DeserializeObject<SaveFile>(json);
            if (latestSave != null)
            {
                profile.SaveFiles.Add(latestSave);
            }
            return profile;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading file {latestFile}: {ex.Message}");
            return null;
        }
    }
}
