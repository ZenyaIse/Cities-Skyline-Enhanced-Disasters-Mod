using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

namespace EnhancedDisastersMod
{
    public class EnhancedTsunami : EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedTsunami d = Singleton<EnhancedDisastersManager>.instance.Tsunami;
                s.WriteInt32(d.cooldownCounter);
                s.WriteFloat(d.hiddenEnergy);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedTsunami d = Singleton<EnhancedDisastersManager>.instance.Tsunami;
                d.cooldownCounter = s.ReadInt32();
                d.hiddenEnergy = s.ReadFloat();

                Debug.Log(">>> EnhancedDisastersMod: Tsunami data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        public float HiddenEnergyThreshold = 1500; // Days
        private float hiddenEnergy = 0; // Days

        public EnhancedTsunami()
        {
            DType = DisasterType.Tsunami;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            cooldownDays = 60;
        }

        protected override void onSimulationFrame_local()
        {
            hiddenEnergy += 1 / framesPerDay;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            return base.getCurrentProbabilityPerFrame() * hiddenEnergy / HiddenEnergyThreshold;
        }

        public override void OnDisasterStarted(byte intensity)
        {
            float strainEnergy_old = hiddenEnergy;
            hiddenEnergy = hiddenEnergy * (1 - intensity / 100f);
            Debug.Log(string.Format(">>> EnhancedDisastersMod: Tsunami hidden energy changed from {0} to {1}.", strainEnergy_old, hiddenEnergy));
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as TsunamiAI != null;
        }

        public override float GetMaximumOccurrencePerYear()
        {
            return OccurrencePerYear * 3;
        }

        public override string GetName()
        {
            return "Tsunami";
        }
    }
}

