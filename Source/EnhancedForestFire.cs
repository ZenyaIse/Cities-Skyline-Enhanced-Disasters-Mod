using System;
using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedForestFire : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedForestFire d = Singleton<EnhancedDisastersManager>.instance.container.ForestFire;
                serializeCommonParameters(s, d);
                s.WriteInt32(d.WarmupDays);
                s.WriteInt32(d.noRainFramesCount);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedForestFire d = Singleton<EnhancedDisastersManager>.instance.container.ForestFire;
                deserializeCommonParameters(s, d);
                d.WarmupDays = s.ReadInt32();
                d.noRainFramesCount = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("ForestFire");
            }
        }

        public int WarmupDays = 180;
        private int noRainFramesCount = 0;

        public EnhancedForestFire()
        {
            DType = DisasterType.ForestFire;
            OccurrenceAreaBeforeUnlock = OccurrenceAreas.LockedAreas;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.Everywhere;
            BaseOccurrencePerYear = 10.0f; // In case of dry weather
            ProbabilityDistribution = ProbabilityDistributions.Uniform;

            calmDays = 7;
            probabilityWarmupDays = 0;
            intensityWarmupDays = 0;
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

        public override string GetProbabilityTooltip()
        {
            string tooltip = "";

            if (!unlocked)
            {
                tooltip = "Not unlocked yet (occurs only outside of your area)." + Environment.NewLine;
            }

            if (calmCounter == 0)
            {
                if (noRainFramesCount == 0)
                {
                    return tooltip + "No " + GetName() + " during rain.";
                }
                else
                {
                    int daysWithoutRain = (int)(noRainFramesCount / framesPerDay);

                    if (daysWithoutRain >= WarmupDays)
                    {
                        return tooltip + "Maximum because there was no rain for more than " + WarmupDays.ToString() + " days.";
                    }

                    return tooltip + "Increasing because there was no rain for " + daysWithoutRain.ToString() + " days.";
                }
            }

            return base.GetProbabilityTooltip();
        }

        protected override float getCurrentOccurrencePerYear_local()
        {
            float daysWithoutRain = noRainFramesCount / framesPerDay;

            return base.getCurrentOccurrencePerYear_local() * Math.Min(1f, daysWithoutRain / WarmupDays);
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as ForestFireAI != null;
        }

        public override string GetName()
        {
            return "Forest Fire";
        }

        public override void CopySettings(EnhancedDisaster disaster)
        {
            base.CopySettings(disaster);

            EnhancedForestFire d = disaster as EnhancedForestFire;
            if (d != null)
            {
                WarmupDays = d.WarmupDays;
            }
        }
    }
}
