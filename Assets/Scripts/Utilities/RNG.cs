using Game.Behaviors.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

static class RNG
{
    [ThreadStatic] public static System.Random rng = new System.Random();

    public static ActorInstance Hero => g.Actors.Heroes.Where(x => x.isPlaying).Shuffle().First();

    public static ActorInstance Enemy => g.Actors.Enemies.Where(x => x.isPlaying).Shuffle().First();

    public static TileInstance Tile => g.Tiles.Shuffle().First();

    public static TileInstance UnoccupiedTile => g.Tiles.Where(x => !x.IsOccupied).Shuffle().FirstOrDefault();

    public static Vector2Int Location => new Vector2Int(Int(1, g.Board.columnCount), Int(1, g.Board.rowCount));

    public static Vector2Int UnoccupiedLocation => UnoccupiedTile == null ? LocationHelper.Nowhere : UnoccupiedTile.location;


    public static Vector2Int UnoccupiedInteriorLocation
    {
        get
        {
            // pick any unoccupied tile that isn't on the board's edge
            var tile = g.Tiles
                .Where(t =>
                    !t.IsOccupied &&
                    t.location.x > 1 && t.location.x < g.Board.columnCount &&
                    t.location.y > 1 && t.location.y < g.Board.rowCount
                )
                .Shuffle()
                .FirstOrDefault();

            return tile == null
                ? LocationHelper.Nowhere
                : tile.location;
        }
    }

    public static int Int(int min, int max) => rng.Next(min, max + 1);

    public static float Float(float min = 0f, float max = 1f) => (float)rng.NextDouble() * (max - min) + min;

    public static float Percent => (float)rng.NextDouble();

    public static float Range(float amount) => (-amount * Percent) + (amount * Percent);

    public static bool Boolean => Int(1, 2) == 1;

    public static Direction AdjacentDirection
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

    public static Direction Direction
    {
        get
        {
            // Pick a random integer from 1 to 8 inclusive
            var result = Int(1, 8);
            return result switch
            {
                1 => Direction.North,
                2 => Direction.NorthEast,
                3 => Direction.East,
                4 => Direction.SouthEast,
                5 => Direction.South,
                6 => Direction.SouthWest,
                7 => Direction.West,
                _ => Direction.NorthWest
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

        //int attackResult = Int(0, sum);

        //if ((attackResult -= ratio0) < 0) return Strategy.AttackClosest;

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


        //TODO: SpawnActor in weighted value so some attackResults are more common that others...

        //int attackResult = Int(0, ratios.Sum());

        /*
        int RATIO_CHANCE_A = 10;
        int RATIO_CHANCE_B = 30;
        int RATIO_CHANCE_C = 60;    
        int RATIO_TOTAL = RATIO_CHANCE_A + RATIO_CHANCE_B + RATIO_CHANCE_C;

        RNG random = new RNG();
        int x = random.None(0, RATIO_TOTAL);

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




        //var attackResult = Int(1, 5);
        //return attackResult switch
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

    public static Sprite Background()
    {
        var keys = SpriteRepo.Backgrounds.Keys.ToList();
        string key = keys[Int(0, keys.Count - 1)];
        return SpriteRepo.Backgrounds[key];
    }

}
