using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class StageWaveData
    {
        public StageWaveData() { }

        public StageWaveData(StageWaveData other)
        {
            WaveID = other.WaveID;
            Actors = other.Actors != null ? new List<StageActor>(other.Actors) : new List<StageActor>();
            DottedLines = other.DottedLines != null ? new List<StageDottedLine>(other.DottedLines) : new List<StageDottedLine>();
        }

        public int WaveID; // Unique identifier for this wave
        public List<StageActor> Actors; // Actors present in this wave
        public List<StageDottedLine> DottedLines; // Dotted lines for wave visualization
    }

}
