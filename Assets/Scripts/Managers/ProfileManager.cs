using Game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using UnityEngine;
using Debug = UnityEngine.Debug;
using Global = Game.Models.ProfileGlobalSection;
using Party = Game.Models.ProfilePartySection;
using Section = Game.Models.ProfileSection;
using Stage = Game.Models.ProfileStageSection;

public class ProfileManager : MonoBehaviour
{

    //Internal properties
    public string currentStage => currentProfile?.Stage.CurrentStage ?? "Stage 1";

    //Fields
    public Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
    public Profile currentProfile = null;

    public void Initialize()
    {
        var sw = Stopwatch.StartNew();

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
            var successful = CreateProfile();
            if (!successful)
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
            var profile = GetProfile(guid);
            if (profile == null || !profile.IsValid())
                continue;

            profiles.Add(guid, profile);
        }

        if (profiles == null || profiles.Count < 1)
        {
            Debug.LogError($"Failed to load any valid profiles.");
            return;
        }

        sw.Stop();
        //Debug.LogWarning($"Loaded current save file in {sw.ElapsedMilliseconds} ms.");

        //TODO: Have user select profile, for now just use first profile
        Load(profiles.First().Key);       
    }

    ///<summary>
    ///Method which is used to create a new folder with GUID containing JSON files
    ///</summary>
    private bool CreateProfile()
    {
        //Generate a new GUID
        string guid;

        // Ensure the generated GUID is unique in profileFolders
        do guid = Guid.NewGuid().ToString("N");
        while (Directory.Exists(Path.Combine(FolderHelper.Folders.Profiles, guid)));

        //Instantiate current profile with the generated GUID; create folder
        currentProfile = new Profile(guid);

        //Save the individual JSON files
        bool globalSaved = Save<Global>();
        bool stageSaved = Save<Stage>();
        bool partySaved = Save<Party>();

        if (!globalSaved || !stageSaved || !partySaved)
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
    private bool Save<T>() where T : class
    {
        var sw = Stopwatch.StartNew();

        //Determine the file name and section based on the generic type
        string fileName = GetFileName<T>();
        Section section = GetSection<T>();

        var filePath = Path.Combine(currentProfile.Folder, fileName);
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

        sw.Stop();
        Debug.Log($"Saved {fileName} successfully in {sw.ElapsedMilliseconds} ms.");

        return true;
    }



    ///<summary>
    ///Method which is used to load individual json file in a seperate profile section
    ///</summary>
    private T Load<T>(string guid) where T : class
    {
        var sw = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(guid))
            return null;

        //Determine the file name based on the generic type
        string fileName = null;
        if (typeof(T) == typeof(Global))
            fileName = "global.json";
        else if (typeof(T) == typeof(Stage))
            fileName = "stage.json";
        else if (typeof(T) == typeof(Party))
            fileName = "party.json";

        var folder = Path.Combine(FolderHelper.Folders.Profiles, guid);
        var filePath = Path.Combine(folder, fileName);
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

        sw.Stop();
        Debug.Log($"Loaded {fileName} successfully in {sw.ElapsedMilliseconds} ms.");

        return section;
    }

    private bool SaveProfile()
    {
        var sw = Stopwatch.StartNew();

        if (currentProfile == null || !currentProfile.IsValid())
        {
            Debug.LogError($"An invalid save file was specified.");
            return false;
        }

        bool globalSaved = Save<Global>();
        bool stageSaved = Save<Stage>();
        bool partySaved = Save<Party>();

        sw.Stop();

        if (!globalSaved || !stageSaved || !partySaved)
        {
            Debug.LogError($"Failed to save one or more components.");
            return false;
        }

        Debug.LogWarning($"Saved all components successfully in {sw.ElapsedMilliseconds} ms.");
        return true;
    }

    public bool QuickSave()
    {
        return SaveProfile();
    }

    private Profile GetProfile(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            Debug.LogError($"An invalid GUID was specified: {guid}");
            return null;
        }

        var profile = new Profile(guid);

        profile.Global = Load<Global>(guid);
        profile.Stage = Load<Stage>(guid);
        profile.Party = Load<Party>(guid);

        if (!profile.IsValid())
        {
            Debug.LogError($"Failed to instantiate profile: {guid}");
            return null;
        }

        return profile;
    }


    public void Load(string guid)
    {
        if (!HasProfiles())
            return;

        if (!profiles.TryGetValue(guid, out Profile profile))
            return;

        currentProfile = profile;
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
        if (typeof(T) == typeof(Global))
            return "global.json";

        if (typeof(T) == typeof(Stage))
            return "stage.json";

        if (typeof(T) == typeof(Party))
            return "party.json";

        return null;
    }

    private Section GetSection<T>() where T : class
    {
        if (typeof(T) == typeof(Global))
            return currentProfile.Global;

        if (typeof(T) == typeof(Stage))
            return currentProfile.Stage;

        if (typeof(T) == typeof(Party))
            return currentProfile.Party;

        return null;
    }


}
