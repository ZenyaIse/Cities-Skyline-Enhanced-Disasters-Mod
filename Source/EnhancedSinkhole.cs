using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedSinkhole : EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedSinkhole d = Singleton<EnhancedDisastersManager>.instance.Sinkhole;
                s.WriteInt32(d.cooldownCounter);
                s.WriteFloat(d.groundwater);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedSinkhole d = Singleton<EnhancedDisastersManager>.instance.Sinkhole;
                d.cooldownCounter = s.ReadInt32();
                d.groundwater = s.ReadFloat();

                Debug.Log(">>> EnhancedDisastersMod: Sinkhole data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        public float GroundwaterCapacity = 180; // Rainy days
        private float groundwater = 0; // Rainy days count

        public EnhancedSinkhole()
        {
            Type = DisasterType.Sinkhole;
            CanOccurEverywhere = false;
            OccurrencePerYear = 1.0f; // When groundwater is full
            ProbabilityDistribution = ProbabilityDistributions.Linear;
            CooldownDays = 1;
        }

        protected override void onSimulationFrame_local()
        {
            WeatherManager wm = Singleton<WeatherManager>.instance;
            if (wm.m_currentRain > 0)
            {
                groundwater += 1 / framesPerDay;
            }
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            return base.getCurrentProbabilityPerFrame() * groundwater / GroundwaterCapacity;
        }

        protected override void afterDisasterStarted(byte intensity)
        {
            if (intensity > 100) intensity = 100;

            float groundwater_old = groundwater;

            groundwater *= (100 - intensity) / 100f;

            Debug.Log(string.Format(">>> EnhancedDisastersMod: Groundwater changed from {0} to {1}", groundwater_old, groundwater));
        }
    }
}
