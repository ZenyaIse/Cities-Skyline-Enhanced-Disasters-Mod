using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedSinkhole : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedSinkhole d = Singleton<EnhancedDisastersManager>.instance.container.Sinkhole;
                serializeCommonParameters(s, d);
                s.WriteFloat(d.GroundwaterCapacity);
                s.WriteFloat(d.groundwaterAmount);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedSinkhole d = Singleton<EnhancedDisastersManager>.instance.container.Sinkhole;
                deserializeCommonParameters(s, d);
                d.GroundwaterCapacity = s.ReadFloat();
                d.groundwaterAmount = s.ReadFloat();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("Sinkhole");
            }
        }

        public float GroundwaterCapacity = 50;
        private float groundwaterAmount = 0; // groundwaterAmount=1 means rain of intensity 1 during 1 day

        public EnhancedSinkhole()
        {
            DType = DisasterType.Sinkhole;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            OccurrencePerYear = 1f; // When groundwater is full
            ProbabilityDistribution = ProbabilityDistributions.Uniform;

            calmDays = 30;
            probabilityWarmupDays = 0;
            intensityWarmupDays = 0;
        }

        protected override void onSimulationFrame_local()
        {
            WeatherManager wm = Singleton<WeatherManager>.instance;
            if (wm.m_currentRain > 0)
            {
                groundwaterAmount += wm.m_currentRain / framesPerDay;
            }

            groundwaterAmount -= groundwaterAmount / GroundwaterCapacity / framesPerDay;

            if (groundwaterAmount < 0)
            {
                groundwaterAmount = 0;
            }
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            return base.getCurrentProbabilityPerFrame() * groundwaterAmount / GroundwaterCapacity;
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
