using Assets.Helpers;
using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public static class StageRepo
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
            { "Stage 1", new Stage
                {
                    Name = "Stage 1",
                    Description = "Intro Battle",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 2",
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
            { "Stage 2", new Stage
                {
                    Name = "Stage 2",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 3",
                    Waves = GenerateWaves(4, new List<string> { CharacterHelper.Slime, CharacterHelper.Scorpion, CharacterHelper.Bat })
                }
            },
            { "Stage 3", new Stage
                {
                    Name = "Stage 3",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 4",
                    Waves = GenerateWaves(5, new List<string> { CharacterHelper.Slime, CharacterHelper.Yeti, CharacterHelper.Scorpion, CharacterHelper.Bat })
                }
            },
            { "Stage 4", new Stage
                {
                    Name = "Stage 4",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 5",
                    Waves = GenerateWaves(3, new List<string> { CharacterHelper.Yeti, CharacterHelper.Slime, CharacterHelper.Bat, CharacterHelper.Scorpion })
                }
            },
            { "Stage 5", new Stage
                {
                    Name = "Stage 5",
                    Description = "DefeatAllEnemies",
                    CompletionCondition = "DefeatAllEnemies",
                    CompletionValue = 0,
                    NextStage = "Stage 6",
                    Waves = GenerateWaves(5, new List<string> { CharacterHelper.Yeti, CharacterHelper.Slime, CharacterHelper.Scorpion, CharacterHelper.Bat })
                }
            }
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
                    Team = Team.Enemy,
                    Location = new Vector2Int(rng.Next(1, 6), rng.Next(1, 6))
                });
            }

            waves.Add(wave);
        }

        return waves;
    }
}
