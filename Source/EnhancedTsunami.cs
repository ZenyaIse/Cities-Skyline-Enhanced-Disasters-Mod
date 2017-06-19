using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

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
            Type = DisasterType.Tsunami;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            CooldownDays = 60;
        }

        protected override void onSimulationFrame_local()
        {
            hiddenEnergy += 1 / framesPerDay;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            return base.getCurrentProbabilityPerFrame() * hiddenEnergy / HiddenEnergyThreshold;
        }

        protected override void afterDisasterStarted(byte intensity)
        {
            if (intensity > 100) intensity = 100;

            float strainEnergy_old = hiddenEnergy;

            hiddenEnergy *= (100 - intensity) / 100f;

            Debug.Log(string.Format(">>> EnhancedDisastersMod: Tsunami hidden energy changed from {0} to {1}.", strainEnergy_old, hiddenEnergy));
        }
    }
}

