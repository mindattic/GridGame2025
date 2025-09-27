using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Libraries
{
    public enum Map
    {
        Test,
        GreenValley,
    }

    public static class StageLibrary
    {
        private static Dictionary<string, Stage> stages;
        private static bool isLoaded = false;

        public static Dictionary<string, Stage> Stages
        {
            get
            {
                if (!isLoaded)
                    Load();
                return stages;
            }
        }

        private static void Load()
        {
            if (isLoaded) return;

            stages = new Dictionary<string, Stage>
            {

                { $"{Map.GreenValley}-00", new Stage
                    {
                        Name = $"{Map.GreenValley}-00",
                        Description = "DefeatAllEnemies",
                        CompletionCondition = "DefeatAllEnemies",
                        CompletionValue = 0,
                        Waves = GenerateWaves(1, new List<string> {
                            CharacterHelper.Slime00,
                            CharacterHelper.Slime01,
                            CharacterHelper.Slime02,
                            CharacterHelper.Slime03,
                        })
                    }
                },
                { $"{Map.GreenValley}-01", new Stage
                    {
                        Name = $"{Map.GreenValley}-01",
                        Description = "DefeatAllEnemies",
                        CompletionCondition = "DefeatAllEnemies",
                        CompletionValue = 0,
                        Waves = GenerateWaves(1, new List<string> {
                            CharacterHelper.Wolf00,
                            CharacterHelper.Wolf01,
                            CharacterHelper.Wolf02,
                            CharacterHelper.Wolf03,
                        })
                    }
                },
                { $"{Map.Test}-00", new Stage
                    {
                        Name = $"{Map.Test}-00",
                        Description = "Intro Battle",
                        CompletionCondition = "DefeatAllEnemies",
                        CompletionValue = 0,
                        Waves = new List<StageWave>
                        {
                            new StageWave
                            {
                                Actors = new List<StageActor>
                                {
                                    new StageActor { characterName = CharacterHelper.Soldier00, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier01, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier02, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier03, Team = Team.Enemy }
                                }
                            },
                            new StageWave
                            {
                                Actors = new List<StageActor>
                                {
                                    new StageActor { characterName = CharacterHelper.Soldier00, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier01, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier02, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier03, Team = Team.Enemy }
                                }
                            },
                            new StageWave
                            {
                                Actors = new List<StageActor>
                                {
                                    new StageActor { characterName = CharacterHelper.Soldier00, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier01, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier02, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Soldier03, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Captain00, Team = Team.Enemy },
                                }
                            },
                            new StageWave
                            {
                                Actors = new List<StageActor>
                                {
                                    new StageActor { characterName = CharacterHelper.Slime00, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Slime01, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Slime02, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Slime03, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Slime00, Team = Team.Enemy, SpawnTurn = 4 },
                                    new StageActor { characterName = CharacterHelper.Slime01, Team = Team.Enemy, SpawnTurn = 4 },
                                    new StageActor { characterName = CharacterHelper.Slime02, Team = Team.Enemy, SpawnTurn = 4 },
                                    new StageActor { characterName = CharacterHelper.Slime03, Team = Team.Enemy, SpawnTurn = 4 },
                                    new StageActor { characterName = CharacterHelper.Slime00, Team = Team.Enemy, SpawnTurn = 8 },
                                    new StageActor { characterName = CharacterHelper.Slime01, Team = Team.Enemy, SpawnTurn = 8 },
                                    new StageActor { characterName = CharacterHelper.Slime02, Team = Team.Enemy, SpawnTurn = 10 },
                                    new StageActor { characterName = CharacterHelper.Slime03, Team = Team.Enemy, SpawnTurn = 10 },
                                    new StageActor { characterName = CharacterHelper.Slime00, Team = Team.Enemy, SpawnTurn = 12 },
                                    new StageActor { characterName = CharacterHelper.Slime01, Team = Team.Enemy, SpawnTurn = 12 },
                                    new StageActor { characterName = CharacterHelper.Slime02, Team = Team.Enemy, SpawnTurn = 14 },
                                    new StageActor { characterName = CharacterHelper.Slime03, Team = Team.Enemy, SpawnTurn = 14 },
                                    new StageActor { characterName = CharacterHelper.Scorpion, Level = 10, Team = Team.Enemy, SpawnTurn = 16 },

                                }
                            },
                            new StageWave
                            {
                                Actors = new List<StageActor>
                                {
                                    new StageActor { characterName = CharacterHelper.Yeti, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Scorpion, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Captain00, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Bat, Team = Team.Enemy },
                                    new StageActor { characterName = CharacterHelper.Bat, Team = Team.Enemy },
                                }
                            },


                        }
                    }
                },
                { $"{Map.Test}-01", new Stage
                    {
                        Name = $"{Map.Test}-01",
                        Description = "DefeatAllEnemies",
                        CompletionCondition = "DefeatAllEnemies",
                        CompletionValue = 0,
                        Waves = GenerateWaves(4, new List<string> { CharacterHelper.Slime, CharacterHelper.Scorpion, CharacterHelper.Bat })
                    }
                },
            };

            isLoaded = true;
        }

        public static Stage Get(string name)
        {
            if (!isLoaded) Load();
            if (!stages.ContainsKey(name))
            {
                Debug.LogError($"Unable to retrieve stage for `{name}`");
                return null;
            }
            return new Stage(stages[name]);
        }

        private static List<StageWave> GenerateWaves(int waveCount, List<string> possibleEnemies)
        {
            List<StageWave> waves = new List<StageWave>();
            System.Random rng = new System.Random();

            for (int i = 0; i < waveCount; i++)
            {
                StageWave wave = new StageWave
                {
                    WaveID = i + 1,
                    Actors = new List<StageActor>(),
                    DottedLines = new List<StageDottedLine>()
                };

                int enemyCount = rng.Next(2, 6);
                for (int j = 0; j < enemyCount; j++)
                {
                    string randomEnemy = possibleEnemies[rng.Next(possibleEnemies.Count)];
                    wave.Actors.Add(new StageActor
                    {
                        characterName = randomEnemy,
                        Team = Team.Enemy
                    });
                }

                waves.Add(wave);
            }

            return waves;
        }
    }
}
