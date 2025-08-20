using Game.Models.Profile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using g = Assets.Helpers.GameHelper;

namespace Assets.Helpers
{
    public static class ProfileHelper
    {
        // ---------------------------------------------------------------------
        // Defaults
        // ---------------------------------------------------------------------

        public const string SettingsFileName = "Settings.json";

        public static ProfileSettings DefaultSettings = new ProfileSettings()
        {
            ActorPanMultiplier = 0.05f,
            GameFocus = 1.0f,
        };

        public static GlobalSaveData DefaultGlobal = new GlobalSaveData()
        {
            TotalCoins = 0,
        };

        public static StageSaveData DefaultStage = new StageSaveData()
        {
            CurrentStage = "Stage 1",
            CurrentWave = 0,
        };

        public static RosterSaveData DefaultRoster = new RosterSaveData()
        {
            Members = new List<CharacterLevelPair>()
            {
                new CharacterLevelPair(CharacterHelper.Paladin),
                new CharacterLevelPair(CharacterHelper.Barbarian),
                new CharacterLevelPair(CharacterHelper.Cleric),
                new CharacterLevelPair(CharacterHelper.GreenNinja),
                new CharacterLevelPair(CharacterHelper.Pugilist),
                new CharacterLevelPair(CharacterHelper.RedNinja),
                new CharacterLevelPair(CharacterHelper.Ronin),
                new CharacterLevelPair(CharacterHelper.Sellsword),
                new CharacterLevelPair(CharacterHelper.Thief),
                new CharacterLevelPair(CharacterHelper.Vampire),
            }
        };

        public static PartySaveData DefaultParty = new PartySaveData()
        {
            Members = new List<CharacterLevelPair>()
            {
                new CharacterLevelPair(CharacterHelper.Paladin),
                new CharacterLevelPair(CharacterHelper.Barbarian),
                new CharacterLevelPair(CharacterHelper.Cleric),
            }
        };

        // ---------------------------------------------------------------------
        // Backing Fields
        // ---------------------------------------------------------------------

        private static List<string> folders = new List<string>();
        private static Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
        private static string currentProfileKey;

        // ---------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------

        // True if any profile folders were discovered.
        public static bool HasFolders => folders.Any();

        // True if a current profile key is set and exists in the profiles map.
        public static bool HasCurrentProfile =>
            HasProfiles() &&
            !string.IsNullOrWhiteSpace(currentProfileKey) &&
            profiles.ContainsKey(currentProfileKey);

        // True if there is a current profile and it has a selected save.
        public static bool HasCurrentSave =>
            HasCurrentProfile &&
            CurrentProfile.HasSaves &&
            CurrentProfile.CurrentSave != null;

        // The active profile or null if not available.
        public static Profile CurrentProfile => HasCurrentProfile ? profiles[currentProfileKey] : null;

        // All loaded profiles keyed by profile name.
        public static Dictionary<string, Profile> Profiles => profiles;

        // ---------------------------------------------------------------------
        // Load and Presence Checks
        // ---------------------------------------------------------------------

        /// <summary>
        /// Returns true if at least one profile exists.
        /// Attempts to load profiles if not already in memory.
        /// If none are found, navigates to ProfileCreate and returns false.
        /// </summary>
        public static bool HasProfiles()
        {
            if (profiles.Any())
                return true;

            Load();

            if (profiles.Any())
                return true;

            // No profiles available. Send the user to create one.
            SceneManager.LoadScene(SceneHelper.ProfileCreate);
            return false;
        }

        /// <summary>
        /// Initializes the profiles list from disk.
        /// Ensures the Profiles folder exists. Loads each profile folder found.
        /// Sets a default current profile if none is chosen.
        /// </summary>
        public static bool Load()
        {
            try
            {
                if (!Directory.Exists(FolderHelper.Folder.Profiles))
                {
                    Debug.LogWarning($"[ProfileHelper] Profiles folder not found. Creating: {FolderHelper.Folder.Profiles}");
                    Directory.CreateDirectory(FolderHelper.Folder.Profiles);
                }

                folders = Directory.GetDirectories(FolderHelper.Folder.Profiles).ToList();

                profiles.Clear();

                foreach (var folder in folders)
                {
                    string key = new DirectoryInfo(folder).Name;
                    var profile = GetProfile(key);
                    if (profile != null)
                    {
                        profiles[key] = profile;
                    }
                    else
                    {
                        Debug.LogWarning($"[ProfileHelper] Failed to load profile: {key}");
                    }
                }

                if (!profiles.Any())
                {
                    Debug.LogWarning("[ProfileHelper] No valid profiles loaded.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(currentProfileKey) || !profiles.ContainsKey(currentProfileKey))
                    currentProfileKey = profiles.Keys.First();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProfileHelper] Load failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reloads profiles from disk. Simple wrapper for Load.
        /// </summary>
        public static void Reload()
        {
            Load();
        }

        // ---------------------------------------------------------------------
        // Create / Delete / Select
        // ---------------------------------------------------------------------

        /// <summary>
        /// Creates a new profile with an initial save and default settings.
        /// Returns the profile key.
        /// </summary>
        public static string CreateProfile(string value)
        {
            // Derive a usable key. If the input is empty, use a GUID.
            string baseKey = !string.IsNullOrWhiteSpace(value) ? value.Trim() : $"{Guid.NewGuid():N}";
            string key = baseKey;
            string folder = Path.Combine(FolderHelper.Folder.Profiles, key);

            // Ensure uniqueness by appending a numeric suffix if needed.
            int i = 1;
            while (Directory.Exists(folder))
            {
                key = $"{baseKey}{i:D3}";
                folder = Path.Combine(FolderHelper.Folder.Profiles, key);
                i++;
            }

            try
            {
                Directory.CreateDirectory(folder);

                var newProfile = new Profile
                {
                    Key = key,
                    Folder = folder,
                    SaveStates = new List<SaveState>()
                };

                // Create an initial save using defaults.
                var newSave = new SaveState(
                    1,
                    DateTime.UtcNow,
                    new GlobalSaveData(ProfileHelper.DefaultGlobal),
                    new StageSaveData(ProfileHelper.DefaultStage),
                    new RosterSaveData(ProfileHelper.DefaultRoster),
                    new PartySaveData(ProfileHelper.DefaultParty));

                newProfile.SaveStates.Add(newSave);
                profiles[key] = newProfile;

                // Select the new profile.
                SelectProfile(key);

                // Persist save to disk.
                string savesPath = Path.Combine(newProfile.Folder, "Saves");
                Directory.CreateDirectory(savesPath);
                File.WriteAllText(Path.Combine(savesPath, newSave.FileName), JsonConvert.SerializeObject(newSave, Formatting.Indented));

                // Load or create settings for the profile.
                newProfile.Settings = LoadSettings(newProfile);

                return key;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProfileHelper] CreateProfile failed for '{key}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deletes a profile by key. Removes from disk and memory.
        /// Returns true if removed.
        /// </summary>
        public static bool DeleteProfile(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            if (!profiles.ContainsKey(key))
                return false;

            try
            {
                string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);

                profiles.Remove(key);

                if (currentProfileKey == key)
                    currentProfileKey = profiles.Keys.FirstOrDefault();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProfileHelper] DeleteProfile failed for '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Selects an existing profile by key. Returns true if successful.
        /// </summary>
        public static bool SelectProfile(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            if (!profiles.ContainsKey(key))
                return false;

            currentProfileKey = key;
            return true;
        }

        // ---------------------------------------------------------------------
        // Profile Load
        // ---------------------------------------------------------------------

        /// <summary>
        /// Builds a Profile object from disk for the given key.
        /// Returns null if the folder is missing or the load fails.
        /// </summary>
        public static Profile GetProfile(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
            if (!Directory.Exists(folder))
                return null;

            var profile = new Profile
            {
                Key = key,
                Folder = folder,
                SaveStates = new List<SaveState>()
            };

            try
            {
                string savesPath = Path.Combine(folder, "Saves");
                if (Directory.Exists(savesPath))
                {
                    var saveFiles = Directory.GetFiles(savesPath, "*.json");
                    foreach (var file in saveFiles)
                    {
                        try
                        {
                            var text = File.ReadAllText(file);
                            if (string.IsNullOrWhiteSpace(text))
                                continue;

                            var save = JsonConvert.DeserializeObject<SaveState>(text);
                            if (save != null)
                                profile.SaveStates.Add(save);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error reading save '{file}': {ex.Message}");
                        }
                    }

                    // Most recent first.
                    profile.SaveStates = profile.SaveStates.OrderByDescending(x => x.Timestamp).ToList();
                }

                // Load settings or fall back to defaults.
                profile.Settings = LoadSettings(profile);

                // Ensure a current save is selected if any exist.
                if (profile.HasSaves && profile.CurrentSave == null)
                    profile.CurrentSave = profile.LatestSave;

                return profile;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProfileHelper] GetProfile failed for '{key}': {ex.Message}");
                return null;
            }
        }

        // ---------------------------------------------------------------------
        // Settings
        // ---------------------------------------------------------------------

        /// <summary>
        /// Loads settings for a profile. If none exist or load fails, writes defaults and returns them.
        /// </summary>
        private static ProfileSettings LoadSettings(Profile profile)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.Folder))
                return ProfileHelper.DefaultSettings;

            try
            {
                string path = Path.Combine(profile.Folder, ProfileHelper.SettingsFileName);
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(text))
                        return JsonConvert.DeserializeObject<ProfileSettings>(text) ?? ProfileHelper.DefaultSettings;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load settings for '{profile?.Key}': {ex.Message}");
            }

            // Persist defaults if missing or failed.
            SaveSettings(profile, ProfileHelper.DefaultSettings);
            return ProfileHelper.DefaultSettings;
        }

        /// <summary>
        /// Persists settings for a profile to disk.
        /// </summary>
        private static void SaveSettings(Profile profile, ProfileSettings settings)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.Folder) || settings == null)
                return;

            try
            {
                string path = Path.Combine(profile.Folder, ProfileHelper.SettingsFileName);
                File.WriteAllText(path, JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save settings for '{profile?.Key}': {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------
        // Save Management
        // ---------------------------------------------------------------------

        /// <summary>
        /// Creates a new save or overwrites the current one.
        /// </summary>
        public static bool Save(bool overwrite = false)
        {
            return overwrite ? OverwriteSave() : CreateSave();
        }

        /// <summary>
        /// Creates a new save based on the current save data.
        /// </summary>
        private static bool CreateSave()
        {
            if (!HasCurrentSave)
                return false;

            try
            {
                var newSave = new SaveState(
                    CurrentProfile.SaveStates.Count,
                    DateTime.UtcNow,
                    CurrentProfile.CurrentSave.Global,
                    CurrentProfile.CurrentSave.Stage,
                    CurrentProfile.CurrentSave.Roster,
                    CurrentProfile.CurrentSave.Party);

                string savesDir = Path.Combine(CurrentProfile.Folder, "Saves");
                Directory.CreateDirectory(savesDir);

                string path = Path.Combine(savesDir, newSave.FileName);
                File.WriteAllText(path, JsonConvert.SerializeObject(newSave, Formatting.Indented));

                CurrentProfile.SaveStates.Add(newSave);
                CurrentProfile.CurrentSave = newSave;
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProfileHelper] CreateSave failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Overwrites the current save on disk and updates it to the front of the list.
        /// </summary>
        private static bool OverwriteSave()
        {
            if (!HasCurrentSave)
                return false;

            try
            {
                var existingSave = new SaveState(CurrentProfile.CurrentSave);

                string savesDir = Path.Combine(CurrentProfile.Folder, "Saves");
                Directory.CreateDirectory(savesDir);

                string path = Path.Combine(savesDir, existingSave.FileName);
                File.WriteAllText(path, JsonConvert.SerializeObject(existingSave, Formatting.Indented));

                CurrentProfile.SaveStates.Remove(CurrentProfile.CurrentSave);
                CurrentProfile.SaveStates.Insert(0, existingSave);
                CurrentProfile.CurrentSave = existingSave;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProfileHelper] OverwriteSave failed: {ex.Message}");
                return false;
            }
        }

        // ---------------------------------------------------------------------
        // Party Management
        // ---------------------------------------------------------------------

        /// <summary>
        /// Adds a character to the party if not already present. Saves on success.
        /// </summary>
        public static void AddToParty(string character)
        {
            if (!HasCurrentSave || string.IsNullOrWhiteSpace(character))
                return;

            var party = CurrentProfile.CurrentSave.Party?.Members;
            if (party == null)
                return;

            if (party.Any(hero => hero.Character == character))
                return;

            party.Add(new CharacterLevelPair(character, 1));
            Save(true);
        }

        /// <summary>
        /// Removes a character from the party if present. Saves on success.
        /// </summary>
        public static void RemoveFromParty(string character)
        {
            if (!HasCurrentSave || string.IsNullOrWhiteSpace(character))
                return;

            var party = CurrentProfile.CurrentSave.Party?.Members;
            if (party == null)
                return;

            if (party.RemoveAll(hero => hero.Character == character) > 0)
                Save(true);
        }
    }
}
