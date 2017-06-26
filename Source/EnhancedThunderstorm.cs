using System;
using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedThunderstorm: EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedThunderstorm d = Singleton<EnhancedDisastersManager>.instance.Thunderstorm;
                s.WriteInt32(d.cooldownCounter);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedThunderstorm d = Singleton<EnhancedDisastersManager>.instance.Thunderstorm;
                d.cooldownCounter = s.ReadInt32();

                Debug.Log(">>> EnhancedDisastersMod: Thunderstorm data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        public float RainFactor = 3.0f;
        public int MaxProbabilityMonth = 7;

        public EnhancedThunderstorm()
        {
            DType = DisasterType.ThunderStorm;
            CanOccurEverywhere = true;
            OccurrencePerYear = 1.0f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            CooldownDays = 30;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            DateTime dt = Singleton<SimulationManager>.instance.m_currentGameTime;
            int delta_month = Math.Abs(dt.Month - MaxProbabilityMonth);
            if (delta_month > 6) delta_month = 12 - delta_month;

            float probability = base.getCurrentProbabilityPerFrame() * (1f - delta_month / 6f);

            WeatherManager wm = Singleton<WeatherManager>.instance;
            if (wm.m_currentRain > 0)
            {
                probability *= RainFactor;
            }

            return probability;
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as ThunderStormAI != null;
        }
    }
}
