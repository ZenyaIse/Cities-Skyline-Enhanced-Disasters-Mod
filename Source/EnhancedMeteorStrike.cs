using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedMeteorStrike : EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedMeteorStrike d = Singleton<EnhancedDisastersManager>.instance.MeteorStrike;
                s.WriteInt32(d.cooldownCounter);
                s.WriteInt8(d.meteorsCount);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedMeteorStrike d = Singleton<EnhancedDisastersManager>.instance.MeteorStrike;
                d.cooldownCounter = s.ReadInt32();
                d.meteorsCount = (byte)s.ReadInt8();

                Debug.Log(">>> EnhancedDisastersMod: Tornado data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        private byte meteorsCount = 0;

        public EnhancedMeteorStrike()
        {
            Type = DisasterType.MeteorStrike;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            CooldownDays = 60;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            if (meteorsCount > 0)
            {
                return base.getCurrentProbabilityPerFrame() * 100;
            }

            return base.getCurrentProbabilityPerFrame();
        }

        protected override void afterDisasterStarted(byte intensity)
        {
            if (intensity > 100) intensity = 100;

            if (meteorsCount == 0)
            {
                meteorsCount = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(3);
            }
            else
            {
                meteorsCount--;
            }

            if (meteorsCount > 0)
            {
                cooldownCounter = 0;
            }

            Debug.Log(string.Format(">>> EnhancedDisastersMod: Group of {0} meteors was created.", meteorsCount + 1));
        }
    }
}
