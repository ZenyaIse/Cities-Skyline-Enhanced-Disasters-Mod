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
                EnhancedThunderstorm d = Singleton<EnhancedDisastersManager>.instance.container.Thunderstorm;
                s.WriteBool(d.Enabled);
                s.WriteFloat(d.OccurrencePerYear);
                s.WriteInt32(d.MaxProbabilityMonth);
                s.WriteFloat(d.RainFactor);
                s.WriteInt32(d.CooldownCounter);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedThunderstorm d = Singleton<EnhancedDisastersManager>.instance.container.Thunderstorm;
                d.Enabled = s.ReadBool();
                d.OccurrencePerYear = s.ReadFloat();
                d.MaxProbabilityMonth = s.ReadInt32();
                d.RainFactor = s.ReadFloat();
                d.CooldownCounter = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(">>> EnhancedDisastersMod: Thunderstorm data loaded.");
            }
        }

        public float RainFactor = 2.0f;
        public int MaxProbabilityMonth = 7;

        public EnhancedThunderstorm()
        {
            DType = DisasterType.ThunderStorm;
            OccurrenceBeforeUnlock = OccurrenceAreas.LockedAreas;
            OccurrenceAfterUnlock = OccurrenceAreas.Everywhere;
            OccurrencePerYear = 1.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            cooldownDays = 60;
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

        public override string GetName()
        {
            return "Thunderstorm";
        }

        protected override void setDisasterAIParameters(DisasterAI dai, byte intensity)
        {
            ThunderStormAI ai = dai as ThunderStormAI;

            if (ai != null)
            {
                uint activeDuration_original = ai.m_activeDuration;
                uint activeDuration_new = activeDuration_original / 2 + (activeDuration_original / 2) * intensity / 100;
                DebugLogger.Log(string.Format("Thunderstorm: m_activeDuration {0} -> {1}", ai.m_activeDuration, activeDuration_new));
                ai.m_activeDuration = activeDuration_new;
            }
        }
    }
}
