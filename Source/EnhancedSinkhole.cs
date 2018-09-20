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
                EnhancedSinkhole d = Singleton<EnhancedDisastersManager>.instance.container.Sinkhole;
                s.WriteBool(d.Enabled);
                s.WriteFloat(d.OccurrencePerYear);

                s.WriteFloat(d.OccurrencePerYearMax);
                s.WriteInt32(d.RainDurationDays);
                s.WriteFloat(d.groundwaterCounter);

                s.WriteInt32(d.calmCounter);
                s.WriteInt32(d.probabilityWarmupCounter);
                s.WriteInt32(d.intensityWarmupCounter);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedSinkhole d = Singleton<EnhancedDisastersManager>.instance.container.Sinkhole;
                d.Enabled = s.ReadBool();
                d.OccurrencePerYear = s.ReadFloat();

                d.OccurrencePerYearMax = s.ReadFloat();
                d.RainDurationDays = s.ReadInt32();
                d.groundwaterCounter = s.ReadFloat();

                d.calmCounter = s.ReadInt32();
                d.probabilityWarmupCounter = s.ReadInt32();
                d.intensityWarmupCounter = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(">>> EnhancedDisastersMod: Sinkhole data loaded.");
            }
        }

        public float OccurrencePerYearMax = 2f;
        public int RainDurationDays = 180; // Rainy days
        private float groundwaterCounter = 0; // Rainy days count

        public EnhancedSinkhole()
        {
            DType = DisasterType.Sinkhole;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            OccurrencePerYear = 0.2f; // When groundwater is 0
            ProbabilityDistribution = ProbabilityDistributions.Uniform;

            calmDays = 14;
            probabilityWarmupDays = 0;
            intensityWarmupDays = 0;
        }

        protected override void onSimulationFrame_local()
        {
            WeatherManager wm = Singleton<WeatherManager>.instance;
            if (wm.m_currentRain > 0)
            {
                groundwaterCounter += 1 / framesPerDay;
            }
            else
            {
                groundwaterCounter -= 1 / framesPerDay;
            }

            groundwaterCounter = Mathf.Clamp(groundwaterCounter, 0, RainDurationDays);
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            float minProbability = base.getCurrentProbabilityPerFrame();
            float maxProbability = OccurrencePerYearMax / framesPerYear;
            return minProbability + (maxProbability - minProbability) * groundwaterCounter / RainDurationDays;
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as SinkholeAI != null;
        }

        public override string GetName()
        {
            return "Sinkhole";
        }
    }
}
