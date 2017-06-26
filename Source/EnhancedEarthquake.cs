using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

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
                s.WriteInt8(d.aftershocksCount);
                s.WriteInt8(d.aftershockMaxIntensity);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.Earthquake;
                d.cooldownCounter = s.ReadInt32();
                d.strainEnergy = s.ReadFloat();
                d.aftershocksCount = (byte)s.ReadInt8();
                d.aftershockMaxIntensity = (byte)s.ReadInt8();

                Debug.Log(">>> EnhancedDisastersMod: Earthquake data loaded.");
            }

            public void AfterDeserialize(DataSerializer s)
            {
                // Empty
            }
        }

        public float StrainThreshold = 700; // Days
        private float strainEnergy = 0; // Days
        private byte aftershocksCount = 0;
        private byte aftershockMaxIntensity = 0;

        public EnhancedEarthquake()
        {
            DType = DisasterType.Earthquake;
            CanOccurEverywhere = false;
            OccurrencePerYear = 0.5f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;
            CooldownDays = 10;
        }

        protected override void onSimulationFrame_local()
        {
            strainEnergy += 1 / framesPerDay;
        }

        protected override float getCurrentProbabilityPerFrame()
        {
            if (aftershocksCount > 0)
            {
                return base.getCurrentProbabilityPerFrame() * 20 * aftershocksCount;
            }

            return base.getCurrentProbabilityPerFrame() * strainEnergy / StrainThreshold;
        }

        public override void OnDisasterCreated(byte intensity)
        {
            if (aftershocksCount == 0)
            {
                aftershockMaxIntensity = (byte)(10 + (intensity - 10) * 3 / 4);
                aftershocksCount = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)intensity / 10);
            }
            else
            {
                aftershocksCount--;
                aftershockMaxIntensity = (byte)(10 + (aftershockMaxIntensity - 10) * 3 / 4);
            }

            Debug.Log(string.Format(">>> EnhancedDisastersMod: {0} aftershocks are still going to happen.", aftershocksCount));
        }

        public override void OnDisasterStarted(byte intensity)
        {
            float strainEnergy_old = strainEnergy;
            strainEnergy = strainEnergy * (1 - intensity / 100f);
            Debug.Log(string.Format(">>> EnhancedDisastersMod: Strain energy changed from {0} to {1}.", strainEnergy_old, strainEnergy));
        }

        protected override byte getRandomIntensity()
        {
            if (aftershocksCount > 0)
            {
                return (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(10, aftershockMaxIntensity);
            }
            else
            {
                return base.getRandomIntensity();
            }
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as EarthquakeAI != null;
        }
    }
}
