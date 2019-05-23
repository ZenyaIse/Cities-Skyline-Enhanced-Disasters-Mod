using ICities;
using ColossalFramework;
using ColossalFramework.IO;

namespace EnhancedDisastersMod
{
    public class EnhancedTsunami : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedTsunami d = Singleton<EnhancedDisastersManager>.instance.container.Tsunami;
                serializeCommonParameters(s, d);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedTsunami d = Singleton<EnhancedDisastersManager>.instance.container.Tsunami;
                deserializeCommonParameters(s, d);
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("Tsunami");
            }
        }

        public EnhancedTsunami()
        {
            DType = DisasterType.Tsunami;
            OccurrencePerYear = 0.4f;
            ProbabilityDistribution = ProbabilityDistributions.Uniform;

            calmDays = 180;
            probabilityWarmupDays = 180;
            intensityWarmupDays = 360 * 3;
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as TsunamiAI != null;
        }

        public override string GetName()
        {
            return "Tsunami";
        }
    }
}

