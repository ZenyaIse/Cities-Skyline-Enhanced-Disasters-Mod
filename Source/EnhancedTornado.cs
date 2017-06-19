using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedTornado : EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.Tornado;
                s.WriteInt32(d.cooldownCounter);
                s.WriteInt8(d.tornadosCount);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedTornado d = Singleton<EnhancedDisastersManager>.instance.Tornado;
                d.cooldownCounter = s.ReadInt32();
                d.tornadosCount = (byte)s.ReadInt8();

                Debug.Log(">>> EnhancedDisastersMod: Tornado data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        private byte tornadosCount = 0;

        public EnhancedTornado()
        {
            Type = DisasterType.Tornado;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            CooldownDays = 15;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            if (tornadosCount > 0)
            {
                return base.getCurrentProbabilityPerFrame() * 1000;
            }

            return base.getCurrentProbabilityPerFrame();
        }

        protected override void afterDisasterStarted(byte intensity)
        {
            if (intensity > 100) intensity = 100;

            if (tornadosCount == 0)
            {
                tornadosCount = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(3);
            }
            else
            {
                tornadosCount--;
            }

            if (tornadosCount > 0)
            {
                cooldownCounter = 0;
            }

            Debug.Log(string.Format(">>> EnhancedDisastersMod: Group of {0} tornados was created.", tornadosCount + 1));
        }
    }
}
