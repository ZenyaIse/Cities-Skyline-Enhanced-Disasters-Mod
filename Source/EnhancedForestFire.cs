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
                EnhancedForestFire d = Singleton<EnhancedDisastersManager>.instance.container.ForestFire;
                s.WriteBool(d.Enabled);
                s.WriteFloat(d.OccurrencePerYear);
                s.WriteInt32(d.WarmupDays);
                s.WriteInt32(d.CooldownCounter);
                s.WriteUInt32(d.noRainFramesCount);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedForestFire d = Singleton<EnhancedDisastersManager>.instance.container.ForestFire;
                d.Enabled = s.ReadBool();
                d.OccurrencePerYear = s.ReadFloat();
                d.WarmupDays = s.ReadInt32();
                d.CooldownCounter = s.ReadInt32();
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
            DType = DisasterType.ForestFire;
            OccurrenceBeforeUnlock = OccurrenceAreas.OuterArea;
            OccurrenceAfterUnlock = OccurrenceAreas.Everywhere;
            OccurrencePerYear = 10.0f; // In case of dry weather
            ProbabilityDistribution = ProbabilityDistributions.Uniform;
            cooldownDays = 1;
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

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as ForestFireAI != null;
        }

        public override float GetMaximumOccurrencePerYear()
        {
            return OccurrencePerYear;
        }

        public override string GetName()
        {
            return "Forest Fire";
        }
    }
}
