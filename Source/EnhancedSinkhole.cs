using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

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

        public float GroundwaterCapacity = 360; // Rainy days
        private float groundwater = 0; // Rainy days count

        public EnhancedSinkhole()
        {
            DType = DisasterType.Sinkhole;
            CanOccurEverywhere = false;
            OccurrencePerYear = 1.0f; // When groundwater is full
            ProbabilityDistribution = ProbabilityDistributions.Uniform;
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

        public override void OnDisasterStarted(byte intensity)
        {
            float groundwater_old = groundwater;
            groundwater *= (100 - intensity) / 100f;
            Debug.Log(string.Format(">>> EnhancedDisastersMod: Groundwater changed from {0} to {1}", groundwater_old, groundwater));
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as SinkholeAI != null;
        }
    }
}
