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
            }

            public void AfterDeserialize(DataSerializer s)
            {
                afterDeserializeLog("EnhancedMeteorStrike");
            }
        }

        private struct MeteorEvent
        {
            public int PeriodFrames;
            public byte MaxIntensity;
            public int FramesUntilNextEvent;
            public int MeteorsFallen;

            public MeteorEvent(int periodFrames, byte maxIntensity, int framesUntilNextEvent)
            {
                PeriodFrames = periodFrames;
                MaxIntensity = maxIntensity;
                FramesUntilNextEvent = framesUntilNextEvent;
                MeteorsFallen = 0;
            }

            public static MeteorEvent Init(float periodYears, byte maxIntensity)
            {
                SimulationManager sm = Singleton<SimulationManager>.instance;

                return new MeteorEvent(
                    (int)(periodYears * framesPerYear + sm.m_randomizer.Int32((uint)(framesPerYear / 2))),
                    maxIntensity,
                    (int)(periodYears * framesPerYear / 4 + sm.m_randomizer.Int32((uint)(periodYears * framesPerYear * 3 / 4)))
                    );
            }

            public float GetProbabilityMultiplier()
            {
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
                if (FramesUntilNextEvent < framesPerDay * 60)
                {
                    return MaxIntensity;
                }

                return 1;
            }

            public void OnSimulationFrame()
            {
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
        }

        private MeteorEvent[] meteorEvents;

        public EnhancedMeteorStrike()
        {
            DType = DisasterType.MeteorStrike;
            OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
            BaseOccurrencePerYear = 3.0f;
            ProbabilityDistribution = ProbabilityDistributions.Uniform;

            meteorEvents = new MeteorEvent[] {
                MeteorEvent.Init(9, 100),
                MeteorEvent.Init(5, 70),
                MeteorEvent.Init(2, 30)
            };
        }

        protected override void onSimulationFrame_local()
        {
            for (int i = 0; i < meteorEvents.Length; i++)
            {
                meteorEvents[i].OnSimulationFrame();
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
