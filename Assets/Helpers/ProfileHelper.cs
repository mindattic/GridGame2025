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
            Members = new List<CharacterLevelPair>() {
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
            Members = new List<CharacterLevelPair>() {
            new CharacterLevelPair(CharacterHelper.Paladin),
            new CharacterLevelPair(CharacterHelper.Barbarian),
            new CharacterLevelPair(CharacterHelper.Cleric),
            }
        };



        // Private Fields
        private static List<string> folders = new List<string>();
        private static Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
        private static string currentProfileKey;

        // Public Properties (No Lazy Loading)
        public static bool HasFolders => folders.Any();

        public static bool HasCurrentProfile => HasProfiles() && !string.IsNullOrWhiteSpace(currentProfileKey) && profiles.ContainsKey(currentProfileKey);
        public static bool HasCurrentSave => HasCurrentProfile && CurrentProfile.HasSaves && CurrentProfile.CurrentSave != null;
        public static Profile CurrentProfile => HasCurrentProfile ? profiles[currentProfileKey] : null;

        public static Dictionary<string, Profile> Profiles => profiles;



        public static bool HasProfiles()
        {
            if (profiles.Any())
                return true;

            Load();

            if (profiles.Any())
                return true;

            SceneManager.LoadScene(SceneHelper.ProfileCreate);
            return false;
        }


        /// <summary>
        /// Call this once at startup to initialize profiles.
        /// </summary>
        public static bool Load()
        {
            if (!Directory.Exists(FolderHelper.Folder.Profiles))
            {
                Debug.LogWarning($"[ProfileRepo] Profiles folder not found. Creating: {FolderHelper.Folder.Profiles}");
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
                    profiles[key] = profile; ;
                }
                else
                {
                    Debug.LogWarning($"[ProfileRepo] Failed to load profile: {key}");
                }
            }

            if (!profiles.Any())
            {
                Debug.LogWarning("[ProfileRepo] No valid profiles loaded.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(currentProfileKey) || !profiles.ContainsKey(currentProfileKey))
                currentProfileKey = profiles.Keys.First();

            return true;
        }


        public static void Reload()
        {
            Load();
        }

        public static string CreateProfile(string value)
        {
            string key = !string.IsNullOrWhiteSpace(value) ? value : $"{Guid.NewGuid():N}";
            string folder = Path.Combine(FolderHelper.Folder.Profiles, key);

            int i = 1;
            while (Directory.Exists(folder))
            {
                key = $"{value}{i:D3}";
                folder = Path.Combine(FolderHelper.Folder.Profiles, key);
                i++;
            }

            Directory.CreateDirectory(folder);

            var newProfile = new Profile
            {
                Key = key,
                Folder = folder,
                SaveStates = new List<SaveState>()
            };

            var newSave = new SaveState(
                1,
                DateTime.UtcNow,
                new GlobalSaveData(ProfileHelper.DefaultGlobal),
                new StageSaveData(ProfileHelper.DefaultStage),
                new RosterSaveData(ProfileHelper.DefaultRoster),
                new PartySaveData(ProfileHelper.DefaultParty));

            newProfile.SaveStates.Add(newSave);
            profiles[key] = newProfile;

            SelectProfile(key);

            string savesPath = Path.Combine(newProfile.Folder, "Saves");
            Directory.CreateDirectory(savesPath);
            File.WriteAllText(Path.Combine(savesPath, newSave.FileName), JsonConvert.SerializeObject(newSave, Formatting.Indented));

            newProfile.Settings = LoadSettings(newProfile);
            return key;
        }

        public static bool DeleteProfile(string key)
        {
            if (!profiles.ContainsKey(key))
                return false;

            string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            profiles.Remove(key);

            if (currentProfileKey == key)
                currentProfileKey = profiles.Keys.FirstOrDefault();

            return true;
        }

        public static bool SelectProfile(string key)
        {
            if (!profiles.ContainsKey(key))
                return false;

            currentProfileKey = key;
            return true;
        }

        public static Profile GetProfile(string key)
        {
            string folder = Path.Combine(FolderHelper.Folder.Profiles, key);
            if (!Directory.Exists(folder))
                return null;

            var profile = new Profile
            {
                Key = key,
                Folder = folder,
                SaveStates = new List<SaveState>()
            };

            string savesPath = Path.Combine(folder, "Saves");
            if (Directory.Exists(savesPath))
            {
                var saveFiles = Directory.GetFiles(savesPath, "*.json");
                foreach (var file in saveFiles)
                {
                    try
                    {
                        var save = JsonConvert.DeserializeObject<SaveState>(File.ReadAllText(file));
                        if (save != null)
                            profile.SaveStates.Add(save);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error reading save `{file}`: {ex.Message}");
                    }
                }

                profile.SaveStates = profile.SaveStates.OrderByDescending(x => x.Timestamp).ToList();
            }

            profile.Settings = LoadSettings(profile);

            if (profile.HasSaves && profile.CurrentSave == null)
                profile.CurrentSave = profile.LatestSave;

            return profile;
        }

        private static ProfileSettings LoadSettings(Profile profile)
        {
            string path = Path.Combine(profile.Folder, ProfileHelper.SettingsFileName);
            if (File.Exists(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<ProfileSettings>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load settings for `{profile.Key}`: {ex.Message}");
                }
            }

            SaveSettings(profile, ProfileHelper.DefaultSettings);
            return ProfileHelper.DefaultSettings;
        }

        private static void SaveSettings(Profile profile, ProfileSettings settings)
        {
            string path = Path.Combine(profile.Folder, ProfileHelper.SettingsFileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        // --- Save Management ---

        public static bool Save(bool overwrite = false)
        {
            return overwrite ? OverwriteSave() : CreateSave();
        }

        private static bool CreateSave()
        {
            if (!HasCurrentSave) return false;

            var newSave = new SaveState(
                CurrentProfile.SaveStates.Count,
                DateTime.UtcNow,
                CurrentProfile.CurrentSave.Global,
                CurrentProfile.CurrentSave.Stage,
                CurrentProfile.CurrentSave.Roster,
                CurrentProfile.CurrentSave.Party);

            string path = Path.Combine(CurrentProfile.Folder, "Saves", newSave.FileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(newSave, Formatting.Indented));

            CurrentProfile.SaveStates.Add(newSave);
            CurrentProfile.CurrentSave = newSave;
            return true;
        }

        private static bool OverwriteSave()
        {
            if (!HasCurrentSave) return false;

            var existingSave = new SaveState(CurrentProfile.CurrentSave);
            string path = Path.Combine(CurrentProfile.Folder, "Saves", existingSave.FileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(existingSave, Formatting.Indented));

            CurrentProfile.SaveStates.Remove(CurrentProfile.CurrentSave);
            CurrentProfile.SaveStates.Insert(0, existingSave);
            CurrentProfile.CurrentSave = existingSave;
            return true;
        }

        // --- Party Management ---

        public static void AddToParty(string character)
        {
            if (!HasCurrentSave) return;

            var party = CurrentProfile.CurrentSave.Party.Members;
            if (party.Any(hero => hero.Character == character))
                return;

            party.Add(new CharacterLevelPair(character, 1));
            Save(true);
        }

        public static void RemoveFromParty(string character)
        {
            if (!HasCurrentSave) return;

            var party = CurrentProfile.CurrentSave.Party.Members;
            if (party.RemoveAll(hero => hero.Character == character) > 0)
                Save(true);
        }


    }



}
