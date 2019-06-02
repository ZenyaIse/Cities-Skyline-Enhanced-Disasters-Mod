using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;
using System;

namespace EnhancedDisastersMod
{
    public class EnhancedEarthquake : EnhancedDisaster
    {
        public class Data : SerializableDataCommon, IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.container.Earthquake;
                serializeCommonParameters(s, d);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.container.Earthquake;
                deserializeCommonParameters(s, d);
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("EnhancedEarthquake");
            }
        }
        //public class Data : IDataContainer
        //{
        //    public void Serialize(DataSerializer s)
        //    {
        //        EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.Earthquake;
        //        s.WriteInt32(d.CooldownCounter);
        //        s.WriteFloat(d.strainEnergy);
        //        s.WriteInt8(d.aftershocksCount);
        //        s.WriteInt8(d.aftershockMaxIntensity);
        //    }

        //    public void Deserialize(DataSerializer s)
        //    {
        //        EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.Earthquake;
        //        d.CooldownCounter = s.ReadInt32();
        //        d.strainEnergy = s.ReadFloat();
        //        d.aftershocksCount = (byte)s.ReadInt8();
        //        d.aftershockMaxIntensity = (byte)s.ReadInt8();

        //        Debug.Log(">>> EnhancedDisastersMod: Earthquake data loaded.");
        //    }

        //    public void AfterDeserialize(DataSerializer s)
        //    {
        //        // Empty
        //    }
        //}

        //public float StrainThreshold = 700; // Days
        //private float strainEnergy = 0; // Days
        private byte aftershocksCount = 0;
        private byte aftershockMaxIntensity = 0;

        public EnhancedEarthquake()
        {
            DType = DisasterType.Earthquake;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            BaseOccurrencePerYear = 1.0f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;

            calmDays = 360;
            probabilityWarmupDays = 360 * 3;
            intensityWarmupDays = 360;
        }

        //protected override void onSimulationFrame_local()
        //{
        //    strainEnergy += 1 / framesPerDay;
        //}

        protected override float getCurrentOccurrencePerYear_local()
        {
            if (aftershocksCount > 0)
            {
                return 12 * aftershocksCount;
            }

            return base.getCurrentOccurrencePerYear_local();
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

        //public override void OnDisasterStarted(byte intensity)
        //{
        //    float strainEnergy_old = strainEnergy;
        //    strainEnergy = strainEnergy * (1 - intensity / 100f);
        //    Debug.Log(string.Format(">>> EnhancedDisastersMod: Strain energy changed from {0} to {1}.", strainEnergy_old, strainEnergy));
        //}

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

        protected override void setDisasterAIParameters(DisasterAI dai, byte intensity)
        {
            EarthquakeAI ai = dai as EarthquakeAI;

            if (ai == null) return;

            DebugLogger.Log(string.Format("EnhancedDisastersMod: m_crackLength = {0}, m_crackWidth = {1}", ai.m_crackLength, ai.m_crackWidth));
            ai.m_crackLength = 0;
            ai.m_crackWidth = 0;
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as EarthquakeAI != null;
        }

        public override string GetName()
        {
            return "Earthquake";
        }
    }
}
