using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class StageDottedLine
    {
        public StageDottedLine() { }

        public StageDottedLine(StageDottedLine other)
        {
            Segment = other.Segment;
            Location = other.Location;
        }

        public DottedLineSegment Segment;
        public Vector2Int Location;
    }
}
