using System;
using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedForestFire : EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedForestFire d = Singleton<EnhancedDisastersManager>.instance.ForestFire;
                s.WriteInt32(d.cooldownCounter);
                s.WriteUInt32(d.noRainFramesCount);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedForestFire d = Singleton<EnhancedDisastersManager>.instance.ForestFire;
                d.cooldownCounter = s.ReadInt32();
                d.noRainFramesCount = s.ReadUInt32();

                Debug.Log(">>> EnhancedDisastersMod: ForestFire data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        public int WarmupDays = 180;
        private uint noRainFramesCount = 0;

        public EnhancedForestFire()
        {
            Type = DisasterType.ForestFire;
            CanOccurEverywhere = true;
            OccurrencePerYear = 4.0f; // In case of dry weather
            ProbabilityDistribution = ProbabilityDistributions.Linear;
            CooldownDays = 1;
        }

        protected override void onSimulationFrame_local()
        {
            WeatherManager wm = Singleton<WeatherManager>.instance;
            if (wm.m_currentRain > 0)
            {
                noRainFramesCount = 0;
            }
            else
            {
                noRainFramesCount++;
            }
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            float daysWithoutRain = noRainFramesCount / framesPerDay;

            return base.getCurrentProbabilityPerFrame() * Math.Min(1f, daysWithoutRain / WarmupDays);
        }
    }
}
