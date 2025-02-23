using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class ConvertString
    {
        public static int ToInt(string value)
        {
            return int.TryParse(value, out var i) ? i : 0;
        }

        public static Vector3 ToVector3(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Vector3.zero; //Null fallback

            //Remove parentheses and split by commas
            value = value.Trim('(', ')');
            string[] split = value.Split(',');

            //Parse each component to float
            if (split.Length >= 3
                && float.TryParse(split[0].Trim(), out float x) &&
                float.TryParse(split[1].Trim(), out float y) &&
                float.TryParse(split[2].Trim(), out float z))
            {
                return new Vector3(x, y, z);
            }

            return Vector3.zero; //Default fallback
        }

        public static Vector2Int ToVector2Int(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Random.UnoccupiedLocation; //Null fallback

            //Remove parentheses and split by commas
            value = value.Trim('(', ')');
            string[] split = value.Split(',');

            //Parse each component to int
            if (split.Length >= 2
                && int.TryParse(split[0].Trim(), out int x)
                && int.TryParse(split[1].Trim(), out int y))
            {
                return new Vector2Int(x, y);
            }

            return GameManager.instance.board.NowhereLocation; //Default fallback
        }

        public static Character ToCharacter(string value)
        {
            return Enum.TryParse<Character>(value, true, out var character)
                ? character
                : Character.Unknown;  //Default fallback
        }

        public static Team ToTeam(string value)
        {
            return Enum.TryParse<Team>(value, true, out var team)
                ? team
                : Team.Neutral; //Default fallback
        }

        public static DottedLineSegment ToDottedLineSegment(string value)
        {
            return Enum.TryParse<DottedLineSegment>(value, true, out var segment)
                ? segment
                : DottedLineSegment.None; //Default fallback
        }
    }

}
