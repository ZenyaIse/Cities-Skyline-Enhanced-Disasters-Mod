using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

namespace EnhancedDisastersMod
{
    public class EnhancedTornado : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.container.Tornado;
                serializeCommonParameters(s, d);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.container.Tornado;
                deserializeCommonParameters(s, d);
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("Tornado");
            }
        }

        private byte tornadosCount = 0;

        public EnhancedTornado()
        {
            DType = DisasterType.Tornado;
            OccurrencePerYear = 0.4f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;

            calmDays = 180;
            probabilityWarmupDays = 180;
            intensityWarmupDays = 360;
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as TornadoAI != null;
        }

        public override string GetName()
        {
            return "Tornado";
        }
    }
}
