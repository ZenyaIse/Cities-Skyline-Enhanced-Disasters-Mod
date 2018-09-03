using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

namespace EnhancedDisastersMod
{
    public class EnhancedTornado : EnhancedDisaster
    {
        //public class Data : IDataContainer
        //{
        //    public void Serialize(DataSerializer s)
        //    {
        //        EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.Tornado;
        //        s.WriteInt32(d.CooldownCounter);
        //        s.WriteInt8(d.tornadosCount);
        //    }

        //    public void Deserialize(DataSerializer s)
        //    {
        //        EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.Tornado;
        //        d.CooldownCounter = s.ReadInt32();
        //        d.tornadosCount = (byte)s.ReadInt8();

        //        Debug.Log(">>> EnhancedDisastersMod: Tornado data loaded.");
        //    }

        //    public void AfterDeserialize(DataSerializer s)
        //    {
        //        // Empty
        //    }
        //}

        private byte tornadosCount = 0;

        public EnhancedTornado()
        {
            DType = DisasterType.Tornado;
            OccurrenceAfterUnlock = OccurrenceAreas.UnlockedAreas;
            OccurrencePerYear = 0.4f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            cooldownDays = 15;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            if (tornadosCount > 0)
            {
                return 100 / framesPerYear; // Every 3 days
            }

            return base.getCurrentProbabilityPerFrame();
        }

        public override void OnDisasterCreated(byte intensity)
        {
            if (tornadosCount == 0)
            {
                tornadosCount = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(3);

                Debug.Log(string.Format(">>> EnhancedDisastersMod: Group of {0} tornados was created.", tornadosCount + 1));
            }
            else
            {
                tornadosCount--;
            }

            if (tornadosCount > 0)
            {
                CooldownCounter = 0;
            }
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as TornadoAI != null;
        }

        public override float GetMaximumOccurrencePerYear()
        {
            return OccurrencePerYear * 2;
        }

        public override string GetName()
        {
            return "Tornado";
        }
    }
}
