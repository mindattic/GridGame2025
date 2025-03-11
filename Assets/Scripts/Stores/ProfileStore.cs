using Assets.Scripts.Store;
using Game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using Debug = UnityEngine.Debug;
using GlobalSection = Game.Models.ProfileGlobalSection;
using PartySection = Game.Models.ProfilePartySection;
using SettingsSection = Game.Models.ProfileSettingsSection;
using StageSection = Game.Models.ProfileStageSection;

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

    // Auto-initialize the ProfileStore before the scene loads.
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

    //Serialized fields
    [SerializeField] public Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
    [SerializeField] public Profile current;


    // Property to check if any profiles exist
    public bool HasProfiles => profiles != null && profiles.Count > 0;

    //Property to validate the folder structure
    private bool HasValidFolderStructure
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FolderHelper.Folders.Profiles))
            {
                Debug.LogError("FolderHelper.Folders.Profiles is null or whitespace.");
                return false;
            }
            if (!Directory.Exists(FolderHelper.Folders.Profiles))
                Directory.CreateDirectory(FolderHelper.Folders.Profiles);
            return Directory.Exists(FolderHelper.Folders.Profiles);
        }
    }

    // Called when the ScriptableObject is enabled.
    private void OnEnable()
    {
        Load();

        if (!HasProfiles)
        {
            Debug.LogError($"Failed to retrieve any valid profile");
            return;
        }

        //TODO: Allow user to select a profile. For now, select the first valid profile.
        Select(profiles.First().Key);
    }

    /// <summary>
    /// Loads all profiles from the profiles folder.
    /// </summary>
    public void Load()
    {
        if (!HasValidFolderStructure)
        {
            Debug.LogError("Folder structure is invalid.");
            return;
        }

        //Retrieve profile folder; create new profile if none exist
        List<string> folders = Directory.GetDirectories(FolderHelper.Folders.Profiles).ToList();
        if (!folders?.Any() == null)
        {
            string guid = Create();
            if (string.IsNullOrWhiteSpace(guid))
            {
                Debug.LogError($"Failed to create new profile");
                return;
            }
            Load();
        }

        //Valdiate at least one profile folder exists
        if (!folders?.Any() == null)
        {
            Debug.LogError($"Failed to retrieve any profile directories from `{FolderHelper.Folders.Profiles}`");
            return;
        }

        //Clear profile collection and iterate across folders to generate list of profiles
        profiles.Clear();
        foreach (var folder in folders)
        {
            string guid = new DirectoryInfo(folder).Name;
            if (string.IsNullOrWhiteSpace(guid))
                continue;

            var profile = Get(guid);
            if (profile == null || !profile.IsValid())
                continue;

            profiles.Add(guid, profile);
        }

        if (!profiles.Any())
        {
            Debug.LogError("Failed to load any valid profiles.");
            return;
        }
    }

    /// <summary>
    /// Creates a new profile folder and initializes its JSON files.
    /// </summary>
    /// <returns>The GUID of the new profile, or null if creation failed.</returns>
    public string Create()
    {
        //Generate a unique GUID
        string guid;
        do
        {
            guid = Guid.NewGuid().ToString("N");
        } while (Directory.Exists(Path.Combine(FolderHelper.Folders.Profiles, guid)));

        //Create the profile folder
        string folder = Path.Combine(FolderHelper.Folders.Profiles, guid);
        Directory.CreateDirectory(folder);

        if (!Directory.Exists(folder))
        {
            Debug.LogError($"Failed to create folder `{folder}`");
            return null;
        }

        // Create a new Profile instance and assign its folder.
        current = new Profile(guid)
        {
            Folder = folder
        };

        // Save default sections.
        bool savedGlobal = SaveSection<GlobalSection>();
        bool savedSettings = SaveSection<SettingsSection>();
        bool savedStage = SaveSection<StageSection>();
        bool savedParty = SaveSection<PartySection>();

        if (!savedGlobal || !savedSettings || !savedStage || !savedParty)
        {
            Debug.LogError($"Failed to create new profile with guid `{guid}`");
            return null;
        }

        return guid;
    }

    /// <summary>
    /// Saves all sections of the current profile.
    /// </summary>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public bool Update()
    {
        if (current == null || !current.IsValid())
        {
            Debug.LogError("An invalid save file was specified.");
            return false;
        }

        bool savedGlobal = SaveSection<GlobalSection>();
        bool savedSettings = SaveSection<SettingsSection>();
        bool savedStage = SaveSection<StageSection>();
        bool savedParty = SaveSection<PartySection>();

        if (!savedGlobal || !savedSettings || !savedStage || !savedParty)
        {
            Debug.LogError("Failed to save one or more components.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads a profile from the specified GUID.
    /// </summary>
    /// <param name="guid">The GUID of the profile to load.</param>
    /// <returns>The loaded Profile, or null if loading failed.</returns>
    public Profile Get(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            Debug.LogError($"An invalid guid was specified `{guid}`");
            return null;
        }

        var profile = new Profile(guid)
        {
            Global = LoadSection<GlobalSection>(guid),
            Settings = LoadSection<SettingsSection>(guid),
            Stage = LoadSection<StageSection>(guid),
            Party = LoadSection<PartySection>(guid)
        };

        if (!profile.IsValid())
        {
            Debug.LogError($"Failed to instantiate profile `{guid}`");
            return null;
        }

        return profile;
    }

    // TODO: Implement deletion logic for profile directories.
    public bool Delete(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            Debug.LogError($"An invalid guid was specified `{guid}`");
            return false;
        }

        // Delete profile directory logic should be implemented here.
        string folder = Path.Combine(FolderHelper.Folders.Profiles, guid);
        Directory.Delete(folder);

        //Validate folder was deleted
        if (Directory.Exists(folder))
        {
            Debug.LogError($"Failed to delete folder `{folder}`");
            return false;
        }

        //Reload all profiles
        Load();

        if (!HasProfiles)
        {
            Debug.LogError($"Failed to retrieve any valid profile");
            return false;
        }

        //Reassign current if it was the deleted profile
        if (current.Guid == guid)
            Select(profiles.First().Key);

        return !Directory.Exists(folder);
    }

    /// <summary>
    /// Selects the profile with the specified GUID as the current profile.
    /// </summary>
    /// <param name="guid">The GUID of the profile to select.</param>
    public bool Select(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            Debug.LogError($"An invalid GUID was specified: {guid}");
            return false;
        }

        if (!HasProfiles)
        {
            Debug.LogError($"Failed to retrieve any valid profile");
            return false;
        }

        if (!profiles.TryGetValue(guid, out Profile profile))
        {
            Debug.LogError($"Failed to retrieve specified profile from guid `{guid}`");
            return false;
        }

        current = profile;
        return true;
    }

    /// <summary>
    /// Saves an individual profile section to its JSON file.
    /// </summary>
    /// <typeparam name="T">The type of the profile section to save.</typeparam>
    /// <returns>True if the save was successful; otherwise, false.</returns>
    private bool SaveSection<T>() where T : class
    {
        string fileName = GetFileName<T>();
        ProfileSection section = GetSection<T>();

        if (current == null || string.IsNullOrWhiteSpace(current.Folder))
        {
            Debug.LogError("Current profile folder is invalid.");
            return false;
        }

        string filePath = Path.Combine(current.Folder, fileName);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Debug.LogError($"Invalid file path for `{fileName}`");
            return false;
        }

        string json = JsonConvert.SerializeObject(section);
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            Debug.LogError($"Failed to serialize section `{fileName}`");
            return false;
        }

        File.WriteAllText(filePath, json);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"{filePath} does not exist after saving.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Loads an individual profile section from its JSON file.
    /// </summary>
    /// <typeparam name="T">The type of the profile section to load.</typeparam>
    /// <param name="guid">The GUID of the profile.</param>
    /// <returns>The loaded section, or null if loading failed.</returns>
    private T LoadSection<T>(string guid) where T : class
    {
        if (string.IsNullOrWhiteSpace(guid))
            return null;

        string fileName = GetFileName<T>();
        string folder = Path.Combine(FolderHelper.Folders.Profiles, guid);
        string filePath = Path.Combine(folder, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"{filePath} does not exist.");
            return null;
        }

        string json = File.ReadAllText(filePath);
        T section = JsonConvert.DeserializeObject<T>(json);
        if (section == null)
        {
            Debug.LogError($"Failed to deserialize {fileName}.");
        }
        return section;
    }

    /// <summary>
    /// Determines the file name for the given profile section type.
    /// </summary>
    private string GetFileName<T>() where T : class
    {
        if (typeof(T) == typeof(GlobalSection))
            return "global.json";
        if (typeof(T) == typeof(SettingsSection))
            return "settings.json";
        if (typeof(T) == typeof(StageSection))
            return "stage.json";
        if (typeof(T) == typeof(PartySection))
            return "party.json";
        return null;
    }

    /// <summary>
    /// Retrieves the corresponding profile section from the current profile.
    /// </summary>
    private ProfileSection GetSection<T>() where T : class
    {
        if (typeof(T) == typeof(GlobalSection))
            return current.Global;
        if (typeof(T) == typeof(SettingsSection))
            return current.Settings;
        if (typeof(T) == typeof(StageSection))
            return current.Stage;
        if (typeof(T) == typeof(PartySection))
            return current.Party;
        return null;
    }
}
