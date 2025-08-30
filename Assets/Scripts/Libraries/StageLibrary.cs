using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;


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
                            //DottedLines = new List<StageDottedLine>
                            //{
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 3) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 4) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 5) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(2, 6) },
                            //    new StageDottedLine { Segment = DottedLineSegment.TurnBottomRight, Location = new Vector2Int(2, 2) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Horizontal, Location = new Vector2Int(3, 2) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Horizontal, Location = new Vector2Int(4, 2) },
                            //    new StageDottedLine { Segment = DottedLineSegment.TurnBottomLeft, Location = new Vector2Int(5, 2) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 3) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 4) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 5) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(5, 6) },
                            //    new StageDottedLine { Segment = DottedLineSegment.TurnTopRight, Location = new Vector2Int(5, 7) },
                            //    new StageDottedLine { Segment = DottedLineSegment.TurnTopLeft, Location = new Vector2Int(6, 7) },
                            //    new StageDottedLine { Segment = DottedLineSegment.Vertical, Location = new Vector2Int(6, 6) },
                            //    new StageDottedLine { Segment = DottedLineSegment.ArrowUp, Location = new Vector2Int(6, 5) }
                            //}
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
                        }
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
        if (!Stages.ContainsKey(name))
        {
            Debug.LogError($"Unable to retrieve stage for `{name}`");
            return null;
        }

        return new Stage(Stages[name]);  // Return a copy
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
