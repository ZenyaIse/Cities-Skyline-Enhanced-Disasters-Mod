using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedEarthquake : EnhancedDisaster
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.Earthquake;
                s.WriteInt32(d.cooldownCounter);
                s.WriteFloat(d.strainEnergy);
                s.WriteInt8(d.aftershockCount);
                s.WriteInt8(d.mainShockIntensity);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.Earthquake;
                d.cooldownCounter = s.ReadInt32();
                d.strainEnergy = s.ReadFloat();
                d.aftershockCount = (byte)s.ReadInt8();
                d.mainShockIntensity = (byte)s.ReadInt8();

                Debug.Log(">>> EnhancedDisastersMod: Earthquake data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        public float StrainThreshold = 700; // Days
        private float strainEnergy = 0; // Days
        private byte aftershockCount = 0;
        private byte mainShockIntensity = 0;

        public EnhancedEarthquake()
        {
            Type = DisasterType.Earthquake;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            CooldownDays = 1;
        }

        protected override void onSimulationFrame_local()
        {
            strainEnergy += 1 / framesPerDay;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            if (aftershockCount > 0)
            {
                return base.getCurrentProbabilityPerFrame() * 20 * aftershockCount;
            }

            return base.getCurrentProbabilityPerFrame() * strainEnergy / StrainThreshold;
        }

        protected override void afterDisasterStarted(byte intensity)
        {
            mainShockIntensity = intensity;

            if (intensity > 100) intensity = 100;

            float strainEnergy_old = strainEnergy;

            strainEnergy *= (100 - intensity) / 100f;

            if (aftershockCount == 0)
            {
                aftershockCount = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)intensity / 10);
            }
            else
            {
                aftershockCount--;
            }

            Debug.Log(string.Format(">>> EnhancedDisastersMod: Strain energy changed from {0} to {1}. New aftershock count is {2}.", strainEnergy_old, strainEnergy, aftershockCount));
        }

        protected override byte getRandomIntensity()
        {
            if (aftershockCount > 0)
            {
                return (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(10, mainShockIntensity);
            }
            else
            {
                return base.getRandomIntensity();
            }
        }
    }
}
