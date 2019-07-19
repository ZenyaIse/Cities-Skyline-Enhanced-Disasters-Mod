using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

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

                s.WriteFloat(d.WarmupYears);

                s.WriteInt8(d.aftershocksCount);
                s.WriteInt8(d.aftershockMaxIntensity);

                s.WriteFloat(d.lastTargetPosition.x);
                s.WriteFloat(d.lastTargetPosition.y);
                s.WriteFloat(d.lastTargetPosition.z);
                s.WriteFloat(d.lastAngle);
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedEarthquake d = Singleton<EnhancedDisastersManager>.instance.container.Earthquake;
                deserializeCommonParameters(s, d);

                d.WarmupYears = s.ReadFloat();

                d.aftershocksCount = (byte)s.ReadInt8();
                d.aftershockMaxIntensity = (byte)s.ReadInt8();

                d.lastTargetPosition = new Vector3(s.ReadFloat(), s.ReadFloat(), s.ReadFloat());
                d.lastAngle = s.ReadFloat();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("EnhancedEarthquake");
            }
        }

        public bool AftershocksEnabled = true;
        private byte aftershocksCount = 0;
        private byte aftershockMaxIntensity = 0;
        private Vector3 lastTargetPosition = new Vector3();
        private float lastAngle = 0;

        public EnhancedEarthquake()
        {
            DType = DisasterType.Earthquake;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            BaseOccurrencePerYear = 1.0f;
            ProbabilityDistribution = ProbabilityDistributions.PowerLow;

            WarmupYears = 3;
        }

        [System.Xml.Serialization.XmlElement]
        public float WarmupYears
        {
            get
            {
                return probabilityWarmupDays / 360f;
            }

            set
            {
                probabilityWarmupDays = (int)(360 * value);
                intensityWarmupDays = probabilityWarmupDays / 2;
                calmDays = probabilityWarmupDays / 2;
            }
        }

        public override string GetProbabilityTooltip()
        {
            if (aftershocksCount > 0)
            {
                return "Expect " + aftershocksCount.ToString() + " more aftershocks";
            }

            return base.GetProbabilityTooltip();
        }

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
            if (!AftershocksEnabled)
            {
                aftershocksCount = 0;
                return;
            }

            if (aftershocksCount == 0)
            {
                aftershockMaxIntensity = (byte)(10 + (intensity - 10) * 3 / 4);
                if (intensity > 20)
                {
                    aftershocksCount = (byte)(1 + Singleton<SimulationManager>.instance.m_randomizer.Int32(1 + (uint)intensity / 20));
                }
            }
            else
            {
                aftershocksCount--;
                aftershockMaxIntensity = (byte)(10 + (aftershockMaxIntensity - 10) * 3 / 4);
            }

            if (aftershocksCount > 0)
            {
                calmCounter = (int)(framesPerDay * 15);
                probabilityWarmupCounter = 0;
                intensityWarmupCounter = 0;

                Debug.Log(string.Format(Mod.LogMsgPrefix + "{0} aftershocks are still going to happen.", aftershocksCount));
            }
        }

        protected override bool findTarget(DisasterInfo disasterInfo, out Vector3 targetPosition, out float angle)
        {
            if (aftershocksCount == 0)
            {
                bool result = base.findTarget(disasterInfo, out targetPosition, out angle);
                lastTargetPosition = targetPosition;
                lastAngle = angle;
                return result;
            }
            else
            {
                targetPosition = lastTargetPosition;
                angle = lastAngle;
                return true;
            }
        }

        protected override byte getRandomIntensity(byte maxIntensity)
        {
            if (aftershocksCount > 0)
            {
                return (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(10, aftershockMaxIntensity);
            }
            else
            {
                return base.getRandomIntensity(maxIntensity);
            }
        }

        protected override void setDisasterAIParameters(DisasterAI dai, byte intensity)
        {
            EarthquakeAI ai = dai as EarthquakeAI;

            if (ai == null) return;

            DebugLogger.Log(Mod.LogMsgPrefix + string.Format("m_crackLength = {0}, m_crackWidth = {1}", ai.m_crackLength, ai.m_crackWidth));
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

        public override void CopySettings(EnhancedDisaster disaster)
        {
            base.CopySettings(disaster);

            EnhancedEarthquake d = disaster as EnhancedEarthquake;
            if (d != null)
            {
                AftershocksEnabled = d.AftershocksEnabled;
                WarmupYears = d.WarmupYears;
            }
        }
    }
}
