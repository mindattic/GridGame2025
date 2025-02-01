using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Location
    {
        private Vector2Int position;

        public int x
        {
            get => position.x;
            set => position.x = value;
        }

        public int y
        {
            get => position.y;
            set => position.y = value;
        }

        public Location(int x, int y)
        {
            position = new Vector2Int(x, y);
        }

        public Vector2Int ToVector2Int()
        {
            return position;
        }

        public override string ToString()
        {
            return position.ToString();
        }
    }
}