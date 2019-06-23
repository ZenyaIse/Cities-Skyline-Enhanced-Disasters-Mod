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

                for (int i = 0; i < d.meteorEvents.Length; i++)
                {
                    s.WriteBool(d.meteorEvents[i].Enabled);
                    s.WriteInt32(d.meteorEvents[i].PeriodFrames);
                    s.WriteInt8(d.meteorEvents[i].MaxIntensity);
                    s.WriteInt32(d.meteorEvents[i].FramesUntilNextEvent);
                    s.WriteInt32(d.meteorEvents[i].MeteorsFallen);
                }
            }

            public void Deserialize(DataSerializer s)
            {
                EnhancedMeteorStrike d = Singleton<EnhancedDisastersManager>.instance.container.MeteorStrike;
                deserializeCommonParameters(s, d);

                for (int i = 0; i < d.meteorEvents.Length; i++)
                {
                    d.meteorEvents[i].Enabled = s.ReadBool();
                    d.meteorEvents[i].PeriodFrames = s.ReadInt32();
                    d.meteorEvents[i].MaxIntensity = (byte)s.ReadInt8();
                    d.meteorEvents[i].FramesUntilNextEvent = s.ReadInt32();
                    d.meteorEvents[i].MeteorsFallen = s.ReadInt32();
                }
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("EnhancedMeteorStrike");
            }
        }

        private struct MeteorEvent
        {
            public string Name;
            public int PeriodFrames;
            public byte MaxIntensity;
            public int FramesUntilNextEvent;
            public int MeteorsFallen;
            public bool Enabled;

            public MeteorEvent(string name, int periodFrames, byte maxIntensity, int framesUntilNextEvent)
            {
                Name = name;
                PeriodFrames = periodFrames;
                MaxIntensity = maxIntensity;
                FramesUntilNextEvent = framesUntilNextEvent;
                MeteorsFallen = 0;
                Enabled = true;
            }

            public static MeteorEvent Init(string name, float periodYears, byte maxIntensity)
            {
                SimulationManager sm = Singleton<SimulationManager>.instance;

                return new MeteorEvent(
                    name,
                    (int)(periodYears * framesPerYear + sm.m_randomizer.Int32((uint)(framesPerYear / 2))),
                    maxIntensity,
                    (int)(periodYears * framesPerYear / 4 + sm.m_randomizer.Int32((uint)(periodYears * framesPerYear * 3 / 4)))
                    );
            }

            public float GetProbabilityMultiplier()
            {
                if (!Enabled) return 0;

                if (MeteorsFallen > 0)
                {
                    return 0;
                }

                float fallPeriod_half = framesPerDay * 30;

                float framesDiffFromPeak = Mathf.Abs(FramesUntilNextEvent - fallPeriod_half);

                float multiplier = Mathf.Max(0, 1f - framesDiffFromPeak / fallPeriod_half);

                return multiplier;
            }

            public byte GetActualMaxIntensity()
            {
                if (!Enabled) return 1;

                if (FramesUntilNextEvent < framesPerDay * 60)
                {
                    return MaxIntensity;
                }

                return 1;
            }

            public void OnSimulationFrame()
            {
                if (!Enabled) return;

                FramesUntilNextEvent--;

                if (FramesUntilNextEvent == 0)
                {
                    FramesUntilNextEvent = PeriodFrames;
                    MeteorsFallen = 0;
                }
            }

            public bool OnMeteorFallen(byte intensity)
            {
                if (intensity > MaxIntensity) return false;

                if (MeteorsFallen > 0) return false;

                MeteorsFallen++;

                return true;
            }

            public override string ToString()
            {
                return string.Format("Period {0} years, max intensity {1}, next meteor after {2} years",
                    PeriodFrames / framesPerYear, MaxIntensity, FramesUntilNextEvent / framesPerYear);
            }

            public string GetStateDescription()
            {
                if (!Enabled) return "";

                if (MeteorsFallen > 0)
                {
                    return Name + " already fallen.";
                }

                if (FramesUntilNextEvent <= framesPerDay * 60)
                {
                    return Name + " is approaching.";
                }
                else
                {
                    return Name + " will be close in " + ((int)(FramesUntilNextEvent / framesPerDay / 30) - 1).ToString() + " months.";
                }
            }
        }

        private MeteorEvent[] meteorEvents;

        public EnhancedMeteorStrike()
        {
            DType = DisasterType.MeteorStrike;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            BaseOccurrencePerYear = 10.0f;
            ProbabilityDistribution = ProbabilityDistributions.Uniform;

            meteorEvents = new MeteorEvent[] {
                MeteorEvent.Init("Long period meteor", 9, 100),
                MeteorEvent.Init("Medium period meteor", 5, 70),
                MeteorEvent.Init("Short period meteor", 2, 30)
            };
        }

        [System.Xml.Serialization.XmlElement]
        public bool Meteor1Enabled
        {
            get
            {
                return GetEnabled(0);
            }

            set
            {
                SetEnabled(0, value);
            }
        }

        [System.Xml.Serialization.XmlElement]
        public bool Meteor2Enabled
        {
            get
            {
                return GetEnabled(1);
            }

            set
            {
                SetEnabled(1, value);
            }
        }

        [System.Xml.Serialization.XmlElement]
        public bool Meteor3Enabled
        {
            get
            {
                return GetEnabled(2);
            }

            set
            {
                SetEnabled(2, value);
            }
        }

        public bool GetEnabled(int index)
        {
            return meteorEvents[index].Enabled;
        }

        public void SetEnabled(int index, bool value)
        {
            meteorEvents[index].Enabled = value;
        }

        private int countTmp = 0;
        protected override void onSimulationFrame_local()
        {
            for (int i = 0; i < meteorEvents.Length; i++)
            {
                meteorEvents[i].OnSimulationFrame();
            }

            if (--countTmp <= 0)
            {
                countTmp = 3000;
                for (int i = 0; i < meteorEvents.Length; i++)
                {
                    Debug.Log("Meteor " + i.ToString() + ". " + meteorEvents[i].ToString());
                }
            }
        }

        protected override float getCurrentOccurrencePerYear_local()
        {
            float baseValue = base.getCurrentOccurrencePerYear_local();

            float result = 0;

            for (int i = 0; i < meteorEvents.Length; i++)
            {
                result += baseValue * meteorEvents[i].GetProbabilityMultiplier();
            }

            return result;
        }

        public override byte GetMaximumIntensity()
        {
            byte result = 10;

            for (int i = 0; i < meteorEvents.Length; i++)
            {
                result = Math.Max(result, meteorEvents[i].GetActualMaxIntensity());
            }

            return scaleIntensity(result);
        }

        public override void OnDisasterCreated(byte intensity)
        {
            for (int i = 0; i < meteorEvents.Length; i++)
            {
                if (meteorEvents[i].OnMeteorFallen(intensity)) return;
            }
        }

        public override bool CheckDisasterAIType(object disasterAI)
        {
            return disasterAI as MeteorStrikeAI != null;
        }

        public override string GetName()
        {
            return "Meteor Strike";
        }

        public override string GetProbabilityTooltip()
        {
            string result = "";

            for (int i = 0; i < meteorEvents.Length; i++)
            {
                result += meteorEvents[i].GetStateDescription() + Environment.NewLine;
            }

            return result;
        }
    }
}
