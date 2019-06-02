using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

namespace EnhancedDisastersMod
{
    public class EnhancedMeteorStrike : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedMeteorStrike d = Singleton<EnhancedDisastersManager>.instance.container.MeteorStrike;
                serializeCommonParameters(s, d);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedMeteorStrike d = Singleton<EnhancedDisastersManager>.instance.container.MeteorStrike;
                deserializeCommonParameters(s, d);
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("EnhancedMeteorStrike");
            }
        }

        //private byte meteorsCount = 0;

        public EnhancedMeteorStrike()
        {
            DType = DisasterType.MeteorStrike;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            BaseOccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;

            calmDays = 360 * 2;
            probabilityWarmupDays = 180;
            intensityWarmupDays = 360;
        }

        //protected override float getCurrentProbabilityPerFrame()
        //{
        //    if (meteorsCount > 0)
        //    {
        //        return 50 / framesPerYear; // Every 7 days
        //    }

        //    return base.getCurrentProbabilityPerFrame();
        //}

        //public override void OnDisasterCreated(byte intensity)
        //{
        //    if (meteorsCount == 0)
        //    {
        //        meteorsCount = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(3);

        //        Debug.Log(string.Format(">>> EnhancedDisastersMod: Group of {0} meteors was created.", meteorsCount + 1));
        //    }
        //    else
        //    {
        //        meteorsCount--;
        //    }

        //    if (meteorsCount > 0)
        //    {
        //        calmCounter = 0;
        //    }
        //}

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as MeteorStrikeAI != null;
        }

        public override string GetName()
        {
            return "Meteor Strike";
        }
    }
}
