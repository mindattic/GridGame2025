using Assets.Scripts.Models;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class StageData
    {
        public StageData() { }

        public StageData(StageData other)
        {
            Name = other.Name;
            Description = other.Description;
            CompletionCondition = other.CompletionCondition;
            CompletionValue = other.CompletionValue;
            NextStage = other.NextStage;

            Tutorials = other.Tutorials != null ? new List<string>(other.Tutorials) : new List<string>();
            Waves = other.Waves != null ? new List<StageWaveData>(other.Waves) : new List<StageWaveData>(); // Now contains waves
        }

        public string Name;
        public string Description;
        public string CompletionCondition;
        public int CompletionValue;
        public string NextStage = "Stage 2";
        public List<string> Tutorials;
        public List<StageWaveData> Waves; // Replacing Actors and DottedLines with waves
    }

}