using Assets.Scripts.Store;
using Game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using GlobalSection = Game.Models.ProfileGlobalSection;
using PartySection = Game.Models.ProfilePartySection;
using SettingsSection = Game.Models.ProfileSettingsSection;
using StageSection = Game.Models.ProfileStageSection;

[CreateAssetMenu(fileName = "ProfileStore", menuName = "Stores/ProfileStore")]
public class ProfileStore : ScriptableObject
{
    //Singleton
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

    //Initialize
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

    //Fields
    [SerializeField] public Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
    [SerializeField] public Profile current = null;

    private void OnEnable()
    {
        //var sw = Stopwatch.StartNew();

        //Validate folder structure
        if (!HasValidFolderStructure())
        {
            Debug.LogError($"Folder structure is invalid.");
            return;
        }

        //Retrieve existing profile profileFolders
        var profileFolders = Directory.GetDirectories(FolderHelper.Folders.Profiles).ToList();

        //If no profile profileFolders found...
        if (profileFolders == null || profileFolders.Count < 1)
        {
            //...create a new profile folder with associated JSON files...
            var wasSuccess = Create();
            if (!wasSuccess)
            {
                Debug.LogError($"Failed to create a new profile.");
                return;
            }

            //Retrieve newly created profile profileFolders
            profileFolders = Directory.GetDirectories(FolderHelper.Folders.Profiles).ToList();
        }

        //Validate profile profileFolders exist
        if (profileFolders == null || profileFolders.Count < 1)
        {
            Debug.LogError($"Failed to retrieve any profile profileFolders from: {FolderHelper.Folders.Profiles}");
            return;
        }

        //Retrive each profile object
        foreach (var folder in profileFolders)
        {
            //Retrieve GUID from folder name
            string guid = new DirectoryInfo(folder).Name;
            if (string.IsNullOrWhiteSpace(guid))
                continue;

            //Retrieve profile from GUID
            var profile = Load(guid);
            if (profile == null || !profile.IsValid())
                continue;

            profiles.Add(guid, profile);
        }

        //Validate profiles exist
        if (profiles == null || profiles.Count < 1)
        {
            Debug.LogError($"Failed to load any valid profiles.");
            return;
        }

        //sw.Stop();
        //Debug.LogWarning($"Loaded current save file in {sw.ElapsedMilliseconds} ms.");

        //TODO: Have user select profile, for now just use first profile
        Assign(profiles.First().Key);
    }

    ///<summary>
    ///Method which is used to create a new folder with GUID containing JSON files
    ///</summary>
    private bool Create()
    {
        //Generate a new GUID
        string guid;

        //Ensure the generated GUID is unique in profileFolders
        do guid = Guid.NewGuid().ToString("N");
        while (Directory.Exists(Path.Combine(FolderHelper.Folders.Profiles, guid)));

        //Instantiate current profile with the generated GUID; create folder
        current = new Profile(guid);

        //Save the individual JSON files
        bool globalSaved = SaveSection<GlobalSection>();
        bool settingsSaved = SaveSection<SettingsSection>();
        bool stageSaved = SaveSection<StageSection>();
        bool partySaved = SaveSection<PartySection>();

        if (!globalSaved || !settingsSaved || !stageSaved || !partySaved)
        {
            Debug.LogError($"Failed to create new profile with GUID: {guid}");
            return false;
        }

        Debug.Log($"Created new profile with GUID: {guid}");
        return true;
    }

    ///<summary>
    ///Method which is used to save individual section to separate JSON file
    ///</summary>
    private bool SaveSection<T>() where T : class
    {
        //var sw = Stopwatch.StartNew();

        //Determine the file name and section based on the generic type
        string fileName = GetFileName<T>();
        ProfileSection section = GetSection<T>();

        var filePath = Path.Combine(current.Folder, fileName);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Debug.LogError($"Invalid file path for: {filePath}");
            return false;
        }

        string json = JsonConvert.SerializeObject(section);
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            Debug.LogError($"Failed to serialize {json}.");
            return false;
        }

        File.WriteAllText(filePath, json);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"{filePath} does not exist after saving.");
            return false;
        }

        //sw.Stop();
        //Debug.Log($"Saved {fileName} successfully in {sw.ElapsedMilliseconds} ms.");

        return true;
    }

    ///<summary>
    ///Method which is used to load individual json file in a seperate profile section
    ///</summary>
    private T LoadSection<T>(string guid) where T : class
    {
        //var sw = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(guid))
            return null;

        //Determine the file name based on the generic type
        string fileName = null;
        if (typeof(T) == typeof(GlobalSection))
            fileName = "global.json";
        else if (typeof(T) == typeof(SettingsSection))
            fileName = "settings.json";
        else if (typeof(T) == typeof(StageSection))
            fileName = "stage.json";
        else if (typeof(T) == typeof(PartySection))
            fileName = "party.json";

        var folder = Path.Combine(FolderHelper.Folders.Profiles, guid);
        var filePath = Path.Combine(folder, fileName);
        if (!File.Exists(filePath))
        {
            //TODO: Emergency rescue the .json here by writing from defaults...
            Debug.LogError($"{filePath} does not exist.");
            return null;
        }

        string json = File.ReadAllText(filePath);
        T section = JsonConvert.DeserializeObject<T>(json);
        if (section == null)
        {
            Debug.LogError($"Failed to deserialize {fileName}.");
        }

        //sw.Stop();
        //Debug.Log($"Loaded {fileName} successfully in {sw.ElapsedMilliseconds} ms.");

        return section;
    }

    public bool Save()
    {
        //var sw = Stopwatch.StartNew();

        if (current == null || !current.IsValid())
        {
            Debug.LogError($"An invalid save file was specified.");
            return false;
        }

        bool globalSaved = SaveSection<GlobalSection>();
        bool settingsSaved = SaveSection<SettingsSection>();
        bool stageSaved = SaveSection<StageSection>();
        bool partySaved = SaveSection<PartySection>();

        //sw.Stop();

        if (!globalSaved || !settingsSaved || !stageSaved || !partySaved)
        {
            Debug.LogError($"Failed to save one or more components.");
            return false;
        }

        //Debug.LogWarning($"Saved all components successfully in {sw.ElapsedMilliseconds} ms.");
        return true;
    }

    private Profile Load(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            Debug.LogError($"An invalid GUID was specified: {guid}");
            return null;
        }

        var profile = new Profile(guid);

        profile.Global = LoadSection<GlobalSection>(guid);
        profile.Settings = LoadSection<SettingsSection>(guid);
        profile.Stage = LoadSection<StageSection>(guid);
        profile.Party = LoadSection<PartySection>(guid);

        if (!profile.IsValid())
        {
            Debug.LogError($"Failed to instantiate profile: {guid}");
            return null;
        }

        return profile;
    }

    public void Assign(string guid)
    {
        if (!HasProfiles())
            return;

        if (!profiles.TryGetValue(guid, out Profile profile))
            return;

        current = profile;
    }

    public bool HasProfiles()
    {
        return profiles != null && profiles.Count > 0;
    }

    private bool HasValidFolderStructure()
    {
        //Verify profiles folder can be created
        if (string.IsNullOrWhiteSpace(FolderHelper.Folders.Profiles))
        {
            Debug.LogError($"FolderHelper.Folders.Profiles is null or whitespace.");
            return false;
        }

        //Create profiles folder (if applicable)
        if (!Directory.Exists(FolderHelper.Folders.Profiles))
            Directory.CreateDirectory(FolderHelper.Folders.Profiles);

        return Directory.Exists(FolderHelper.Folders.Profiles);
    }


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
