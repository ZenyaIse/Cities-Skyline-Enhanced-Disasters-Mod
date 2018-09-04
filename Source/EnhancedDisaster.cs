using System;
using ICities;
using UnityEngine;
using ColossalFramework;
using System.Xml.Serialization;

namespace EnhancedDisastersMod
{
    public abstract class EnhancedDisaster
    {
        protected const float framesPerDay = 585.142f; // See m_timePerFrame from SimulationManager.Awake()
        protected const float framesPerYear = framesPerDay * 365f;
        protected const uint randomizerRange = 67108864u;
        protected int cooldownDays = 7;

        [XmlIgnore]
        public int CooldownCounter = (int)framesPerDay * 7; // Init value for a new game
        [XmlIgnore]
        public DisasterType DType = DisasterType.Empty;
        [XmlIgnore]
        public ProbabilityDistributions ProbabilityDistribution = ProbabilityDistributions.Uniform;
        [XmlIgnore]
        public int FullIntensityDisasterPopulation = 20000;
        [XmlIgnore]
        public OccurrenceAreas OccurrenceBeforeUnlock = OccurrenceAreas.Nowhere;
        [XmlIgnore]
        public OccurrenceAreas OccurrenceAfterUnlock = OccurrenceAreas.UnlockedAreas;
        [XmlIgnore]
        public bool Unlocked = false;

        public bool Enabled = true;
        public float OccurrencePerYear = 1.0f;

        public void OnSimulationFrame()
        {
            if (!Enabled)
            {
                return;
            }

            if (!Unlocked && OccurrenceBeforeUnlock == OccurrenceAreas.Nowhere)
            {
                return;
            }

            onSimulationFrame_local();

            if (CooldownCounter > 0)
            {
                CooldownCounter--;
                return;
            }

            float probability = getCurrentProbabilityPerFrame();

            if (probability == 0)
            {
                return;
            }

            //probability *= 5;

            SimulationManager sm = Singleton<SimulationManager>.instance;
            if (sm.m_randomizer.Int32(randomizerRange) < (uint)(randomizerRange * probability))
            {
                byte intensity = getRandomIntensity();
                byte scaled_intensity = scaleIntensityByPopulation(intensity);

                startDisaster(scaled_intensity);

                CooldownCounter = (int)framesPerDay * cooldownDays;

                //afterDisasterStarted(intensity);
            }
        }

        public abstract string GetName();

        protected virtual void onSimulationFrame_local()
        {

        }

        protected virtual float getCurrentProbabilityPerFrame()
        {
            return OccurrencePerYear / framesPerYear;
        }

        public float GetCurrentOccurrencePerYear()
        {
            return getCurrentProbabilityPerFrame() * framesPerYear;
        }

        public virtual void OnDisasterCreated(byte intensity)
        {
            // Empty
        }

        public virtual void OnDisasterStarted(byte intensity)
        {
            // Empty
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
            DisasterManager dm = Singleton<DisasterManager>.instance;

            bool targetFound = false;
            OccurrenceAreas area = Unlocked ? OccurrenceAfterUnlock : OccurrenceBeforeUnlock;
            switch (area)
            {
                case OccurrenceAreas.LockedAreas:
                    targetFound = findRandomTargetInLockedAreas(out targetPosition, out angle);
                    break;
                case OccurrenceAreas.Everywhere:
                    targetFound = findRandomTargetEverywhere(out targetPosition, out angle);
                    break;
                default: // Vanilla default
                    targetFound = disasterInfo.m_disasterAI.FindRandomTarget(out targetPosition, out angle);
                    break;
            }

            if (!targetFound)
            {
                if (EnhancedDisastersManager.IsDebug)
                {
                    Debug.Log(getDebugStr() + "target not found");
                }
                return;
            }

            ushort disasterIndex;
            bool disasterCreated = dm.CreateDisaster(out disasterIndex, disasterInfo);
            if (!disasterCreated)
            {
                if (EnhancedDisastersManager.IsDebug)
                {
                    Debug.Log(getDebugStr() + "could not create disaster");
                }
                return;
            }

            Debug.Log("Started by EnhancedDisastersMod");
            DisasterLogger.StartedByMod = true;

            setDisasterAIParameters(disasterInfo.m_disasterAI, intensity);

            disasterStarting(disasterInfo);

            dm.m_disasters.m_buffer[(int)disasterIndex].m_targetPosition = targetPosition;
            dm.m_disasters.m_buffer[(int)disasterIndex].m_angle = angle;
            dm.m_disasters.m_buffer[(int)disasterIndex].m_intensity = intensity;
            DisasterData[] expr_98_cp_0 = dm.m_disasters.m_buffer;
            ushort expr_98_cp_1 = disasterIndex;
            expr_98_cp_0[(int)expr_98_cp_1].m_flags = (expr_98_cp_0[(int)expr_98_cp_1].m_flags | DisasterData.Flags.SelfTrigger);
            disasterInfo.m_disasterAI.StartNow(disasterIndex, ref dm.m_disasters.m_buffer[(int)disasterIndex]);

            if (EnhancedDisastersManager.IsDebug)
            {
                Debug.Log(getDebugStr() + string.Format("disaster intensity: {0}", intensity));
            }
        }

        protected virtual void setDisasterAIParameters(DisasterAI dai, byte intensity)
        {

        }

        protected string getDebugStr()
        {
            return ">>> EnhancedDisastersMod: " + DType.ToString() + ", " + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString() + ", ";
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
                    if (IsUnlocked(i, j))
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
                        if (IsUnlocked(j - 1, i))
                        {
                            minX += minimumEdgeDistance;
                        }
                        if (IsUnlocked(j, i - 1))
                        {
                            minZ += minimumEdgeDistance;
                        }
                        if (IsUnlocked(j + 1, i))
                        {
                            maxX -= minimumEdgeDistance;
                        }
                        if (IsUnlocked(j, i + 1))
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
                        return true;
                    }
                }
            }

            target = Vector3.zero;
            angle = 0f;
            return false;
        }

        public bool IsUnlocked(int x, int z)
        {
            return x >= 0 && z >= 0 && x < 5 && z < 5 && Singleton<GameAreaManager>.instance.m_areaGrid[z * 5 + x] != 0;
        }

        protected virtual byte getRandomIntensity()
        {
            int intensity;

            if (ProbabilityDistribution == ProbabilityDistributions.PowerLow)
            {
                float randomValue = Singleton<SimulationManager>.instance.m_randomizer.Int32(1000, 10000) / 10000.0f; // from range 0.1 - 1.0

                // See Gutenberg–Richter law.
                // a, b = 0.11
                intensity = (int)(10 * (0.11 - Math.Log10(randomValue)) / 0.11);

                if (intensity > 100)
                {
                    intensity = 100;
                }
            }
            else
            {
                intensity = Singleton<SimulationManager>.instance.m_randomizer.Int32(10, 100);
            }

            return (byte)intensity;
        }

        private byte scaleIntensityByPopulation(byte intensity)
        {
            int population = getPopulation();

            if (population < FullIntensityDisasterPopulation)
            {
                intensity = (byte)(10 + (intensity - 10) * population / FullIntensityDisasterPopulation);
            }

            return intensity;
        }

        protected int getPopulation()
        {
            if (Singleton<DistrictManager>.exists)
            {
                return (int)Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_populationData.m_finalCount;
            }
            return 0;
        }

        public abstract float GetMaximumOccurrencePerYear();
    }
}
