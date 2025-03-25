using Game.Models.Profile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "ProfileRepo", menuName = "Repositories/ProfileRepo")]
public class ProfileRepo : ScriptableObject
{
    // Singleton instance
    private static ProfileRepo Instance;
    public static ProfileRepo instance
    {
        get
        {
            if (Instance == null)
                Debug.LogError("ProfileRepo accessed before being initialized! Ensure it's assigned in Awake().");
            return Instance;
        }
    }

    // Auto-initialize before the scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            Instance = Resources.Load<ProfileRepo>(RepositoryHelper.ProfileRepo);
            if (Instance == null)
                Debug.LogError($"{RepositoryHelper.ProfileRepo} asset not found.");
        }
    }

    // Fields
    private List<string> folders = new List<string>();
    public Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
    public string currentProfileKey;

    // Properties
    public bool HasFolders => folders.Any();
    public bool HasProfiles => profiles.Any();
    public bool HasCurrentProfile => HasProfiles && !string.IsNullOrWhiteSpace(currentProfileKey) && profiles.ContainsKey(currentProfileKey);
    public bool HasCurrentSave => HasCurrentProfile && CurrentProfile.HasSaves && CurrentProfile.CurrentSave != null;
    public Profile CurrentProfile => HasCurrentProfile ? profiles[currentProfileKey] : null;

    // Constructor
    private void OnEnable()
    {
        Reload();
    }

    public bool Reload()
    {
        // Create profiles folder (if applicable)
        if (!Directory.Exists(FolderHelper.Folder.Profiles))
            Directory.CreateDirectory(FolderHelper.Folder.Profiles);
        if (!Directory.Exists(FolderHelper.Folder.Profiles))
        {
            Debug.LogError($"Folder structure is invalid `{FolderHelper.Folder.Profiles}`");
            return false;
        }

        // Retrieve all folders in profile directory
        folders = Directory.GetDirectories(FolderHelper.Folder.Profiles).ToList();
        if (!HasFolders)
        {
            //TODO: Somehow fix this so that monobehavior calls this and creates a profile when none is available...
            //SceneManager.LoadSceneAsync(SceneHelper.ProfileCreate);
            return false;
        }

        folders = Directory.GetDirectories(FolderHelper.Folder.Profiles).ToList();
        if (!folders.Any())
        {
            Debug.LogError("No folders found");
            return false;
        }

        //Populate profile list
        profiles = new Dictionary<string, Profile>();
        foreach (var folder in folders)
        {
            string key = new DirectoryInfo(folder).Name;
            var profile = GetProfile(key);
            if (profile != null)
                profiles.Add(key, profile);
        }
        if (!profiles.Any())
        {
            Debug.LogError("No profiles found");
            return false;
        }

        // Auto-select a profile if one is not already selected.
        var success = !string.IsNullOrWhiteSpace(currentProfileKey)
            ? SelectProfile(currentProfileKey)
            : SelectProfile(profiles.Keys.First());

        return success;
    }

    public string CreateProfile(string input)
    {
        //Generate a unique key and create profile folder
        string key;
        string folder;
        do
        {
            key = !string.IsNullOrWhiteSpace(input) ? input : $"{Guid.NewGuid():N}";
            folder = Path.Combine(FolderHelper.Folder.Profiles, key);
        } while (Directory.Exists(folder));
        Directory.CreateDirectory(folder);

        // Create a new profile object
        Profile newProfile = new Profile()
        {
            Key = key,
            Folder = folder,
            SaveStates = new List<SaveState>()
        };

        // Create the initial SaveState and append to new profile object
        SaveState newSave = new SaveState(
            1,
            DateTime.UtcNow,
            new GlobalSaveData(ProfileHelper.DefaultGlobal),
            new StageSaveData(ProfileHelper.DefaultStage),
            new PartySaveData(ProfileHelper.DefaultParty));
        newProfile.SaveStates.Add(newSave);

        // Append profile and set as current
        profiles.Add(key, newProfile);
        currentProfileKey = key;

        // Write the initial SaveState to disk
        string savesFolder = Path.Combine(newProfile.Folder, "Saves");
        Directory.CreateDirectory(savesFolder);
        string filePath = Path.Combine(savesFolder, newSave.FileName);
        string json = JsonConvert.SerializeObject(newSave, Formatting.Indented);
        try
        {
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing file {filePath}: {ex.Message}");
            return null;
        }

        // Initialize settings in a separate file.
        newProfile.Settings = LoadSettings(newProfile);

        return key;
    }

    public Profile GetProfile(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"Invalid key specified: {key}");
            return null;
        }

        string folder = Path.Combine(FolderHelper.Folder.Profiles, key);

        //Reconstruct profile
        Profile profile = new Profile()
        {
            Key = key,
            Folder = folder,
            SaveStates = new List<SaveState>()
        };

        //Retrieve save files by profile
        string savesFolder = Path.Combine(folder, "Saves");
        var saveFiles = Directory.GetFiles(savesFolder, "*.json").ToArray();
        foreach (var file in saveFiles)
        {
            try
            {
                string json = File.ReadAllText(file);
                SaveState save = JsonConvert.DeserializeObject<SaveState>(json);
                if (save != null)
                    profile.SaveStates.Add(save);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading file {file}: {ex.Message}");
            }
        }

        //Sort the SaveStates list by timestamp
        profile.SaveStates = profile.SaveStates.OrderByDescending(x => x.Timestamp).ToList();

        // Load settings from the separate settings file.
        profile.Settings = LoadSettings(profile);

        // Automatically select latest save as current save when retrieving profile.
        if (profile.HasSaves && profile.CurrentSave == null)
            profile.CurrentSave = profile.LatestSave;

        return profile;
    }

    public bool DeleteProfile(string key)
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
        Reload();
        return true;
    }

    public bool SelectProfile(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"Invalid key specified: {key}");
            return false;
        }
        if (!profiles.ContainsKey(key))
        {
            Debug.LogError($"No profile matches key: {key}");
            return false;
        }
        currentProfileKey = key;
        return true;
    }

    // Loads settings from a separate settings.json file in the profile folder.
    private ProfileSettings LoadSettings(Profile profile)
    {
        string settingsPath = Path.Combine(profile.Folder, ProfileHelper.SettingsFileName);
        if (File.Exists(settingsPath))
        {
            try
            {
                string json = File.ReadAllText(settingsPath);
                var settings = JsonConvert.DeserializeObject<ProfileSettings>(json);
                return settings;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading settings file {settingsPath}: {ex.Message}");
            }
        }

        //If the file doesn't exist or an error occurs, create one with the default settings.
        SaveSettings(profile, ProfileHelper.DefaultSettings);
        return ProfileHelper.DefaultSettings;
    }

    //Writes the given settings to settings.json in the profile folder.
    private void SaveSettings(Profile profile, ProfileSettings settings)
    {
        string settingsPath = Path.Combine(profile.Folder, ProfileHelper.SettingsFileName);
        try
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing settings file {settingsPath}: {ex.Message}");
        }
    }

    //Public Save method – pass true to overwrite current save, false to create new.
    public bool Save(bool overwrite = false)
    {
        bool success = !overwrite ? CreateSave() : OverwriteSave();
        if (!success)
        {
            Debug.LogError($"Failed to {(!overwrite ? "create new" : "overwrite existing")} save file.");
            return false;
        }
        return true;
    }

    private bool HasValidCurrentSave()
    {
        if (!HasCurrentProfile)
        {
            Debug.LogError("Profile is null.");
            return false;
        }
        if (!CurrentProfile.HasSaves)
        {
            Debug.LogError("Profile has no save states.");
            return false;
        }
        if (!CurrentProfile.HasCurrentSave)
        {
            Debug.LogError("Profile has no current save.");
            return false;
        }
        return true;
    }

    //Creates a new SaveState from the current game state.
    private bool CreateSave()
    {
        if (!HasValidCurrentSave())
            return false;

        SaveState newSave = new SaveState(
            CurrentProfile.SaveStates.Count,
            DateTime.UtcNow,
            CurrentProfile.CurrentSave.Global,
            CurrentProfile.CurrentSave.Stage,
            CurrentProfile.CurrentSave.Party);

        string savesFolder = Path.Combine(CurrentProfile.Folder, "Saves");
        string filePath = Path.Combine(savesFolder, newSave.FileName);
        string json = JsonConvert.SerializeObject(newSave, Formatting.Indented);
        try
        {
            File.WriteAllText(filePath, json);
            CurrentProfile.SaveStates.Add(newSave);
            CurrentProfile.CurrentSave = newSave; // update active save
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing file {filePath}: {ex.Message}");
            return false;
        }

        //Sort the SaveStates list by timestamp
        CurrentProfile.SaveStates = CurrentProfile.SaveStates.OrderByDescending(x => x.Timestamp).ToList();

        return true;
    }

    // Overwrites the current SaveState with the current game state.
    private bool OverwriteSave()
    {
        if (!HasValidCurrentSave())
            return false;

        var existingSave = new SaveState(CurrentProfile.CurrentSave);
        if (existingSave == null)
        {
            Debug.LogError("Failed to retrieve existing save.");
            return false;
        }

        string savesFolder = Path.Combine(CurrentProfile.Folder, "Saves");
        string filePath = Path.Combine(savesFolder, existingSave.FileName);
        string json = JsonConvert.SerializeObject(existingSave, Formatting.Indented);
        try
        {
            File.WriteAllText(filePath, json);
            CurrentProfile.SaveStates.Remove(CurrentProfile.CurrentSave);
            CurrentProfile.SaveStates.Insert(0, existingSave);
            CurrentProfile.CurrentSave = existingSave; // update active save
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error overwriting file {filePath}: {ex.Message}");
            return false;
        }

        //Sort the SaveStates list by timestamp
        CurrentProfile.SaveStates = CurrentProfile.SaveStates.OrderByDescending(x => x.Timestamp).ToList();

        return true;
    }
}
