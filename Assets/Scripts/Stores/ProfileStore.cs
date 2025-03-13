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

    // Serialized fields
    public Dictionary<string, Profile> profiles;
    public string selectedKey;
    private List<string> folders;

    // Properties
    public bool HasFolders => folders != null && folders.Count > 0;
    public bool HasProfiles => profiles != null && profiles.Count > 0;
    public bool HasSelectedKey => !string.IsNullOrWhiteSpace(selectedKey);
    public bool HasSelectedProfile => HasProfiles && HasSelectedKey && profiles.ContainsKey(selectedKey);
    public Profile selectedProfile => HasSelectedProfile ? profiles[selectedKey] : null;

    // Startup
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

        // Auto-select a profile
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
            // Create a new profile if none exist
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
    /// Creates a new profile folder and initializes its JSON file.
    /// </summary>
    /// <returns>The key of the new profile, or null if creation failed.</returns>
    public string Create()
    {
        // Generate a unique key and folder path
        string key;
        string folder;
        do
        {
            key = Guid.NewGuid().ToString("N");
            folder = Path.Combine(FolderHelper.Folder.Profiles, key);
        } while (Directory.Exists(folder));

        Directory.CreateDirectory(folder);

        // Create a new profile with default sections
        Profile newProfile = new Profile(key)
        {
            Folder = folder
        };

        profiles ??= new Dictionary<string, Profile>();
        profiles.Add(key, newProfile);
        selectedKey = key;

        // Save the complete profile as a single JSON file
        if (!SaveProfile(newProfile))
        {
            Debug.LogError($"Failed to create new profile with key `{key}`");
            return null;
        }

        return key;
    }

    /// <summary>
    /// Saves the entire selected profile to a single JSON file.
    /// </summary>
    public bool Save()
    {
        if (!HasSelectedProfile)
        {
            Debug.LogError("An invalid save file was specified.");
            return false;
        }

        if (!SaveProfile(selectedProfile))
        {
            Debug.LogError("Failed to save profile.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads a profile from its key by deserializing its JSON file.
    /// </summary>
    public Profile Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"An invalid key was specified `{key}`");
            return null;
        }
        return LoadProfile(key);
    }

    public bool Delete(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"An invalid key was specified `{key}`");
            return false;
        }

        string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
        if (Directory.Exists(folder))
        {
            // Delete the folder recursively
            Directory.Delete(folder, true);
        }

        // Reload profiles after deletion
        Load();
        return true;
    }

    /// <summary>
    /// Selects the profile with the specified key.
    /// </summary>
    public bool Select(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"An invalid key was specified `{key}`");
            return false;
        }

        if (!profiles.ContainsKey(key))
        {
            Debug.LogError($"There is no profile that matches key `{key}`");
            return false;
        }

        selectedKey = key;
        return true;
    }

    /// <summary>
    /// Serializes the entire profile to a single JSON file.
    /// </summary>
    private bool SaveProfile(Profile profile)
    {
        if (profile == null)
        {
            Debug.LogError("Profile is null.");
            return false;
        }
        string filePath = Path.Combine(profile.Folder, "profile.json");
        try
        {
            string json = JsonConvert.SerializeObject(profile, Formatting.Indented);
            if (string.IsNullOrWhiteSpace(json) || json == "{}")
            {
                Debug.LogError("Failed to serialize profile.");
                return false;
            }
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing file {filePath}: {ex.Message}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Loads the profile by deserializing its JSON file.
    /// If the file doesn't exist, a default profile is created, saved, and returned.
    /// </summary>
    private Profile LoadProfile(string key)
    {
        string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
        string filePath = Path.Combine(folder, "profile.json");
        if (!File.Exists(filePath))
        {
            // Create a default profile and save it
            Profile defaultProfile = new Profile(key) { Folder = folder };
            try
            {
                string json = JsonConvert.SerializeObject(defaultProfile, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing file {filePath}: {ex.Message}");
                return null;
            }
            return defaultProfile;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            Profile profile = JsonConvert.DeserializeObject<Profile>(json);
            if (profile == null)
            {
                Debug.LogError("Failed to deserialize profile.json.");
            }
            // Ensure the Folder property is correctly set
            profile.Folder = folder;
            return profile;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading file {filePath}: {ex.Message}");
            return null;
        }
    }
}
