using Game.Behaviors.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static class Random
{
    [ThreadStatic] public static System.Random rng = new System.Random();

    //Properties
    private static IQueryable<ActorInstance> players => GameManager.instance.players;
    private static IQueryable<ActorInstance> enemies => GameManager.instance.enemies;
    private static List<ActorInstance> actors => GameManager.instance.actors;
    private static List<TileInstance> tiles => GameManager.instance.tiles;
    private static int columnCount => GameManager.instance.board.columnCount;
    private static int rowCount => GameManager.instance.board.rowCount;

    public static int Int(int min, int max) => rng.Next(min, max + 1);

    public static float Float(float min = 0f, float max = 1f) => (float)rng.NextDouble() * (max - min) + min;

    public static float Percent => (float)rng.NextDouble();

    public static float Range(float amount) => (-amount * Percent) + (amount * Percent);

    public static bool Boolean => Int(1, 2) == 1;

    public static Direction Direction
    {
        get
        {
            var result = Int(1, 4);
            return result switch {
                1 => Direction.North,
                2 => Direction.East,
                3 => Direction.South,
                _ => Direction.West,
            };
        }
    }

    public static Color Color => new Color(Float(), Float(), Float(), 1f);

    public static AttackStrategy Strategy(params int[] ratios)
    {
        //int sum = Int(0, ratios.Sum());

        //int ratio0 = ratios[0];
        //int ratio1 = ratio0 + ratios[1];
        //int ratio2 = ratio1 + ratios[2];
        //int ratio3 = ratio2 + ratios[3];
        //int ratio4 = ratio3 + ratios[4];
        //int ratio5 = ratio4 + ratios[5];

        //int result = Int(0, sum);

        //if ((result -= ratio0) < 0) return Strategy.AttackClosest;

        //{
        //   do_something1();
        //}
        //else if ((x -= RATIO_CHANCE_B) < 0) //Test for B
        //{
        //   do_something2();
        //}
        ////... etc
        //else //No need for final if statement
        //{
        //   do_somethingN();
        //}


        //TODO: SpawnActor in weighted value so some attacks are more common that others...

        //int result = Int(0, ratios.Sum());

        /*
        int RATIO_CHANCE_A = 10;
        int RATIO_CHANCE_B = 30;
        int RATIO_CHANCE_C = 60;    
        int RATIO_TOTAL = RATIO_CHANCE_A + RATIO_CHANCE_B + RATIO_CHANCE_C;

        Random random = new Random();
        int x = random.Next(0, RATIO_TOTAL);

        if ((x -= RATIO_CHANCE_A) < 0) //Test for A
        { 
             do_something1();
        } 
        else if ((x -= RATIO_CHANCE_B) < 0) //Test for B
        { 
             do_something2();
        }
        //... etc
        else //No need for final if statement
        { 
             do_somethingN();
        }
        */




        //var result = Int(1, 5);
        //return result switch
        //{
        //   1 => Strategy.MoveAnywhere,
        //   2 => Strategy.AttackClosest,
        //   3 => Strategy.AttackWeakest,
        //   4 => Strategy.AttackStrongest,
        //   5 => Strategy.AttackRandom,
        //   Attack => Strategy.MoveAnywhere,
        //};

        var result = Int(1, 2);
        return result switch
        {
            1 => AttackStrategy.AttackClosest,
            2 => AttackStrategy.AttackRandom,
            _ => AttackStrategy.AttackClosest,
        };

    }





    public static ActorInstance Player => players.Where(x => x.isPlaying).Shuffle().First();

    public static ActorInstance Enemy => enemies.Where(x => x.isPlaying).Shuffle().First();

    public static TileInstance Tile => tiles.Shuffle().First();

    public static TileInstance UnoccupiedTile => tiles.Where(x => !x.IsOccupied).Shuffle().First();

    public static Vector2Int Location => new Vector2Int(Int(1, columnCount), Int(1, rowCount));

    public static Vector2Int UnoccupiedLocation => UnoccupiedTile.location;


    public static T EnumValue<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));  
        return (T)values.GetValue(Int(0, values.Length - 1));
    }

    public static WeaponType WeaponType()
    {
        return EnumValue<WeaponType>();
    }

    public static float ShakeIntensityLevel()
    {
        //Randomly pick between High, Medium, and Low
        int choice = Int(1, 3); //Generate a random number between 1 and 3
        return choice switch
        {
            1 => ShakeIntensity.High,
            2 => ShakeIntensity.Medium,
            3 => ShakeIntensity.Low,
            _ => ShakeIntensity.Low //Default to Low as a fallback
        };
    }

}
