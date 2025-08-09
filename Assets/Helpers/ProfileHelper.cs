using Game.Models.Profile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    }



}
