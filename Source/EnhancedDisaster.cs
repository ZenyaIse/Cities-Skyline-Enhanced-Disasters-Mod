using System;
using ICities;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public abstract class EnhancedDisaster
    {
        public class SerializableDataCommon
        {
            public void serializeCommonParameters(DataSerializer s, EnhancedDisaster disaster)
            {
                s.WriteBool(disaster.Enabled);
                s.WriteFloat(disaster.BaseOccurrencePerYear);
                s.WriteInt32(disaster.calmCounter);
                s.WriteInt32(disaster.probabilityWarmupCounter);
                s.WriteInt32(disaster.intensityWarmupCounter);
            }

            public void deserializeCommonParameters(DataSerializer s, EnhancedDisaster disaster)
            {
                disaster.Enabled = s.ReadBool();
                disaster.BaseOccurrencePerYear = s.ReadFloat();
                disaster.calmCounter = s.ReadInt32();
                disaster.probabilityWarmupCounter = s.ReadInt32();
                disaster.intensityWarmupCounter = s.ReadInt32();
            }

            public void afterDeserializeLog(string className)
            {
                Debug.Log(Mod.LogMsgPrefix + className + " data loaded.");
            }
        }


        // Constants
        protected const uint randomizerRange = 67108864u;

        // Cooldown variables
        protected int calmDays = 0;
        protected int calmCounter = 0;
        protected int probabilityWarmupDays = 0;
        protected int probabilityWarmupCounter = 0;
        protected int intensityWarmupDays = 0;
        protected int intensityWarmupCounter = 0;

        // Disaster properties
        protected DisasterType DType = DisasterType.Empty;
        protected ProbabilityDistributions ProbabilityDistribution = ProbabilityDistributions.Uniform;
        protected int FullIntensityPopulation = 20000;
        protected OccurrenceAreas OccurrenceAreaBeforeUnlock = OccurrenceAreas.Nowhere;
        protected OccurrenceAreas OccurrenceAreaAfterUnlock = OccurrenceAreas.UnlockedAreas;
        protected bool unlocked = false;

        // Disaster public properties (to be saved in xml)
        public bool Enabled = true;
        public float BaseOccurrencePerYear = 1.0f;


        // Public

        public abstract string GetName();

        public float GetCurrentOccurrencePerYear()
        {
            if (calmCounter > 0)
            {
                return 0f;
            }

            return scaleProbability(getCurrentOccurrencePerYear_local());
        }

        public virtual byte GetMaximumIntensity()
        {
            byte intensity = 100;

            intensity = scaleByWarmups(intensity);

            intensity = scaleByPopulation(intensity);

            return intensity;
        }

        public void OnSimulationFrame()
        {
            if (!Enabled)
            {
                return;
            }

            if (!unlocked && OccurrenceAreaBeforeUnlock == OccurrenceAreas.Nowhere)
            {
                return;
            }

            onSimulationFrame_local();

            if (calmCounter > 0)
            {
                calmCounter--;
                return;
            }

            float framesPerDay = Helper.FramesPerDay;

            if (probabilityWarmupCounter > 0)
            {
                if (probabilityWarmupCounter > framesPerDay * probabilityWarmupDays)
                {
                    probabilityWarmupCounter = (int)(framesPerDay * probabilityWarmupDays);
                }

                probabilityWarmupCounter--;
            }

            if (intensityWarmupCounter > 0)
            {
                if (intensityWarmupCounter > framesPerDay * intensityWarmupDays)
                {
                    intensityWarmupCounter = (int)(framesPerDay * intensityWarmupDays);
                }

                intensityWarmupCounter--;
            }

            float occurrencePerYear = GetCurrentOccurrencePerYear();

            if (occurrencePerYear == 0)
            {
                return;
            }

            SimulationManager sm = Singleton<SimulationManager>.instance;
            if (sm.m_randomizer.Int32(randomizerRange) < (uint)(randomizerRange * occurrencePerYear / (framesPerDay * 365)))
            {
                byte intensity = getRandomIntensity(GetMaximumIntensity());

                startDisaster(intensity);
            }
        }

        public void Unlock()
        {
            unlocked = true;
        }

        public virtual string GetProbabilityTooltip()
        {
            if (!unlocked)
            {
                return "Not unlocked yet";
            }

            if (calmCounter > 0)
            {
                return "No " + GetName() + " for another " + formatDate(calmCounter);
            }

            if (probabilityWarmupCounter > 0)
            {
                return "Decreased because " + GetName() + " occured recently.";
            }

            return "";
        }

        public virtual string GetIntensityTooltip()
        {
            if (!unlocked)
            {
                return "Not unlocked yet";
            }

            if (calmCounter > 0)
            {
                return "No " + GetName() + " for another " + formatDate(calmCounter);
            }

            string result = "";

            if (intensityWarmupCounter > 0)
            {
                result = "Decreased because " + GetName() + " occured recently.";
            }

            if (getPopulation() < FullIntensityPopulation)
            {
                if (result != "") result += Environment.NewLine;
                result += "Decreased because of low population.";
            }

            return result;
        }

        public virtual void CopySettings(EnhancedDisaster disaster)
        {
            Enabled = disaster.Enabled;
            BaseOccurrencePerYear = disaster.BaseOccurrencePerYear;
        }


        // Utilities

        protected virtual float getCurrentOccurrencePerYear_local()
        {
            return BaseOccurrencePerYear;
        }

        protected virtual byte getRandomIntensity(byte maxIntensity)
        {
            byte intensity;

            if (ProbabilityDistribution == ProbabilityDistributions.PowerLow)
            {
                float randomValue = Singleton<SimulationManager>.instance.m_randomizer.Int32(1000, 10000) / 10000.0f; // from range 0.1 - 1.0

                // See Gutenberg–Richter law.
                // a, b = 0.11
                intensity = (byte)(10 * (0.11 - Math.Log10(randomValue)) / 0.11);

                if (intensity > 100)
                {
                    intensity = 100;
                }
            }
            else
            {
                intensity = (byte)Singleton<SimulationManager>.instance.m_randomizer.Int32(10, 100);
            }

            if (maxIntensity < 100)
            {
                intensity = (byte)(10 + (intensity - 10) * maxIntensity / 100);
            }

            return intensity;
        }

        protected byte scaleByWarmups(byte intensity)
        {
            float framesPerDay = Helper.FramesPerDay;

            if (intensityWarmupCounter > 0 && intensityWarmupDays > 0)
            {
                if (intensityWarmupCounter >= framesPerDay * intensityWarmupDays)
                {
                    intensity = 10;
                }
                else
                {
                    intensity = (byte)(10 + (intensity - 10) * (1 - intensityWarmupCounter / (framesPerDay * intensityWarmupDays)));
                }
            }

            return intensity;
        }

        protected byte scaleByPopulation(byte intensity)
        {
            if (Singleton<EnhancedDisastersManager>.instance.container.ScaleMaxIntensityWithPopilation)
            {
                int population = getPopulation();
                if (population < FullIntensityPopulation)
                {
                    intensity = (byte)(10 + (intensity - 10) * population / FullIntensityPopulation);
                }
            }

            return intensity;
        }

        private float scaleProbability(float probability)
        {
            float framesPerDay = Helper.FramesPerDay;

            if (!unlocked && OccurrenceAreaBeforeUnlock == OccurrenceAreas.Nowhere)
            {
                return 0;
            }

            if (probabilityWarmupCounter > 0 && probabilityWarmupDays > 0)
            {
                if (probabilityWarmupCounter >= framesPerDay * probabilityWarmupDays)
                {
                    probability = 0;
                }
                else
                {
                    probability *= 1 - probabilityWarmupCounter / (framesPerDay * probabilityWarmupDays);
                }
            }

            return probability;
        }

        protected int getPopulation()
        {
            if (Singleton<DistrictManager>.exists)
            {
                return (int)Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_populationData.m_finalCount;
            }
            return 0;
        }

        protected string getDebugStr()
        {
            return DType.ToString() + ", " + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString() + ", ";
        }

        protected string formatDate(int frames)
        {
            int days = (int)(frames / Helper.FramesPerDay);
            return days.ToString() + " days";
        }


        // Disaster

        protected virtual void onSimulationFrame_local()
        {

        }

        //public virtual void OnDisasterCreated(byte intensity)
        //{
            // Empty
        //}

        public virtual void OnDisasterStarted(byte intensity)
        {
            float framesPerDay = Helper.FramesPerDay;

            calmCounter = (int)(framesPerDay * calmDays * intensity / 100);
            probabilityWarmupCounter = (int)(framesPerDay * probabilityWarmupDays);
            intensityWarmupCounter = (int)(framesPerDay * intensityWarmupDays);
        }

        protected virtual void disasterStarting(DisasterInfo disasterInfo)
        {

        }

        public abstract bool CheckDisasterAIType(object disasterAI);

        protected void startDisaster(byte intensity)
        {
            DisasterInfo disasterInfo = EnhancedDisastersManager.GetDisasterInfo(DType);

            if (disasterInfo == null)
            {
                return;
            }

            Vector3 targetPosition;
            float angle;
            bool targetFound = findTarget(disasterInfo, out targetPosition, out angle);

            if (!targetFound)
            {
                DebugLogger.Log(getDebugStr() + "target not found");
                return;
            }

            DisasterManager dm = Singleton<DisasterManager>.instance;

            ushort disasterIndex;
            bool disasterCreated = dm.CreateDisaster(out disasterIndex, disasterInfo);
            if (!disasterCreated)
            {
                DebugLogger.Log(getDebugStr() + "could not create disaster");
                return;
            }

            DisasterLogger.StartedByMod = true;

            disasterStarting(disasterInfo);

            dm.m_disasters.m_buffer[(int)disasterIndex].m_targetPosition = targetPosition;
            dm.m_disasters.m_buffer[(int)disasterIndex].m_angle = angle;
            dm.m_disasters.m_buffer[(int)disasterIndex].m_intensity = intensity;
            DisasterData[] expr_98_cp_0 = dm.m_disasters.m_buffer;
            ushort expr_98_cp_1 = disasterIndex;
            expr_98_cp_0[(int)expr_98_cp_1].m_flags = (expr_98_cp_0[(int)expr_98_cp_1].m_flags | DisasterData.Flags.SelfTrigger);
            disasterInfo.m_disasterAI.StartNow(disasterIndex, ref dm.m_disasters.m_buffer[(int)disasterIndex]);

            DebugLogger.Log(getDebugStr() + string.Format("disaster intensity: {0}, area: {1}", intensity, unlocked ? OccurrenceAreaAfterUnlock : OccurrenceAreaBeforeUnlock));
        }

        protected virtual bool findTarget(DisasterInfo disasterInfo, out Vector3 targetPosition, out float angle)
        {
            OccurrenceAreas area = unlocked ? OccurrenceAreaAfterUnlock : OccurrenceAreaBeforeUnlock;
            switch (area)
            {
                case OccurrenceAreas.LockedAreas:
                    return findRandomTargetInLockedAreas(out targetPosition, out angle);
                case OccurrenceAreas.Everywhere:
                    return findRandomTargetEverywhere(out targetPosition, out angle);
                case OccurrenceAreas.UnlockedAreas: // Vanilla default
                    return disasterInfo.m_disasterAI.FindRandomTarget(out targetPosition, out angle);
                default:
                    targetPosition = new Vector3();
                    angle = 0;
                    return false;
            }
        }

        private bool findRandomTargetEverywhere(out Vector3 target, out float angle)
        {
            GameAreaManager gam = Singleton<GameAreaManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;
            int i = sm.m_randomizer.Int32(0, 4);
            int j = sm.m_randomizer.Int32(0, 4);
            float minX;
            float minZ;
            float maxX;
            float maxZ;
            gam.GetAreaBounds(i, j, out minX, out minZ, out maxX, out maxZ);

            float randX = (float)sm.m_randomizer.Int32(0, 10000) * 0.0001f;
            float randZ = (float)sm.m_randomizer.Int32(0, 10000) * 0.0001f;
            target.x = minX + (maxX - minX) * randX;
            target.y = 0f;
            target.z = minZ + (maxZ - minZ) * randZ;
            target.y = Singleton<TerrainManager>.instance.SampleRawHeightSmoothWithWater(target, false, 0f);
            angle = (float)sm.m_randomizer.Int32(0, 10000) * 0.0006283185f;
            return true;
        }

        private bool findRandomTargetInLockedAreas(out Vector3 target, out float angle)
        {
            GameAreaManager gam = Singleton<GameAreaManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            // No locked areas
            if (gam.m_areaCount >= 25)
            {
                target = Vector3.zero;
                angle = 0f;
                return false;
            }

            int lockedAreaCounter = sm.m_randomizer.Int32(1, 25 - gam.m_areaCount);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (isUnlocked(i, j))
                    {
                        continue;
                    }

                    if (--lockedAreaCounter == 0)
                    {
                        float minX;
                        float minZ;
                        float maxX;
                        float maxZ;
                        gam.GetAreaBounds(j, i, out minX, out minZ, out maxX, out maxZ);
                        float minimumEdgeDistance = 100f;
                        if (isUnlocked(j - 1, i))
                        {
                            minX += minimumEdgeDistance;
                        }
                        if (isUnlocked(j, i - 1))
                        {
                            minZ += minimumEdgeDistance;
                        }
                        if (isUnlocked(j + 1, i))
                        {
                            maxX -= minimumEdgeDistance;
                        }
                        if (isUnlocked(j, i + 1))
                        {
                            maxZ -= minimumEdgeDistance;
                        }

                        float randX = (float)sm.m_randomizer.Int32(0, 10000) * 0.0001f;
                        float randZ = (float)sm.m_randomizer.Int32(0, 10000) * 0.0001f;
                        target.x = minX + (maxX - minX) * randX;
                        target.y = 0f;
                        target.z = minZ + (maxZ - minZ) * randZ;
                        target.y = Singleton<TerrainManager>.instance.SampleRawHeightSmoothWithWater(target, false, 0f);
                        angle = (float)sm.m_randomizer.Int32(0, 10000) * 0.0006283185f;
                        DebugLogger.Log(string.Format("findRandomTargetInLockedAreas, j = {0}, i = {1}, areaCount = {2}", j, i, gam.m_areaCount));
                        return true;
                    }
                }
            }

            target = Vector3.zero;
            angle = 0f;
            return false;
        }

        private bool isUnlocked(int x, int z)
        {
            return x >= 0 && z >= 0 && x < 5 && z < 5 && Singleton<GameAreaManager>.instance.m_areaGrid[z * 5 + x] != 0;
        }
    }
}
