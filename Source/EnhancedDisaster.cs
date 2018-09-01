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
        public OccurrenceAreas OccurrenceAfterUnlock = OccurrenceAreas.InnerArea;
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
            targetFound = findRandomTarget(out targetPosition, out angle, area);
            //targetFound = disasterInfo.m_disasterAI.FindRandomTarget(out targetPosition, out angle);

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

            setDisasterAIParameters(disasterInfo.m_disasterAI, intensity);

            disasterStarting(disasterInfo);

            dm.m_disasters.m_buffer[(int)disasterIndex].m_targetPosition = targetPosition;
            dm.m_disasters.m_buffer[(int)disasterIndex].m_angle = angle;
            dm.m_disasters.m_buffer[(int)disasterIndex].m_intensity = intensity;
            DisasterData[] expr_98_cp_0 = dm.m_disasters.m_buffer;
            ushort expr_98_cp_1 = disasterIndex;
            expr_98_cp_0[(int)expr_98_cp_1].m_flags = (expr_98_cp_0[(int)expr_98_cp_1].m_flags | DisasterData.Flags.SelfTrigger);
            disasterInfo.m_disasterAI.StartNow(disasterIndex, ref dm.m_disasters.m_buffer[(int)disasterIndex]);

            Debug.Log("Started by EnhancedDisastersMod");

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

        private bool findRandomTarget(out Vector3 target, out float angle, OccurrenceAreas area)
        {
            GameAreaManager gam = Singleton<GameAreaManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;
            if (gam.m_areaCount > 0)
            {
                int areaIndex = sm.m_randomizer.Int32(1, gam.m_areaCount);
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (gam.GetArea(j, i) == areaIndex)
                        {
                            float minX;
                            float minZ;
                            float maxX;
                            float maxZ;
                            gam.GetAreaBounds(j, i, out minX, out minZ, out maxX, out maxZ);
                            float minimumEdgeDistance = 100f; // TO DO: implement
                            if (!CanOccur(j - 1, i, area))
                            {
                                minX += minimumEdgeDistance;
                            }
                            if (!CanOccur(j, i - 1, area))
                            {
                                minZ += minimumEdgeDistance;
                            }
                            if (!CanOccur(j + 1, i, area))
                            {
                                maxX -= minimumEdgeDistance;
                            }
                            if (!CanOccur(j, i + 1, area))
                            {
                                maxZ -= minimumEdgeDistance;
                            }
                            float randX = (float)sm.m_randomizer.Int32(0, 10000) * 0.0001f;
                            float randZ = (float)sm.m_randomizer.Int32(0, 10000) * 0.0001f;
                            target.x = minX + (maxX - minX) * randX;
                            target.y = 0f;
                            target.z = minZ + (maxZ - minZ) * randZ;
                            clampDisasterTarget(ref target, area);
                            target.y = Singleton<TerrainManager>.instance.SampleRawHeightSmoothWithWater(target, false, 0f);
                            angle = (float)sm.m_randomizer.Int32(0, 10000) * 0.0006283185f;
                            return true;
                        }
                    }
                }
            }
            target = Vector3.zero;
            angle = 0f;
            return false;
        }

        private void clampDisasterTarget(ref Vector3 target, OccurrenceAreas area)
        {
            GameAreaManager instance = Singleton<GameAreaManager>.instance;
            float minimumEdgeDistance = 100f;
            int x;
            int z;
            instance.GetTileXZ(target, out x, out z);
            float minX;
            float minZ;
            float maxX;
            float maxZ;
            instance.GetAreaBounds(x, z, out minX, out minZ, out maxX, out maxZ);
            if (!CanOccur(x - 1, z, area))
            {
                float dx1 = target.x - minX;
                if (dx1 < minimumEdgeDistance)
                {
                    target.x += minimumEdgeDistance - dx1;
                }
            }
            if (!CanOccur(x, z - 1, area))
            {
                float dz1 = target.z - minZ;
                if (dz1 < minimumEdgeDistance)
                {
                    target.z += minimumEdgeDistance - dz1;
                }
            }
            if (!CanOccur(x + 1, z, area))
            {
                float dx2 = maxX - target.x;
                if (dx2 < minimumEdgeDistance)
                {
                    target.x += dx2 - minimumEdgeDistance;
                }
            }
            if (!CanOccur(x, z + 1, area))
            {
                float dz2 = maxZ - target.z;
                if (dz2 < minimumEdgeDistance)
                {
                    target.z += dz2 - minimumEdgeDistance;
                }
            }
            if (!CanOccur(x - 1, z - 1, area))
            {
                Vector3 vector = new Vector3(minX, target.y, minZ);
                Vector3 vector2 = target - vector;
                if (vector2.sqrMagnitude < minimumEdgeDistance * minimumEdgeDistance)
                {
                    if (vector2.sqrMagnitude < 1f)
                    {
                        vector2 = new Vector3(1f, 0f, 1f);
                    }
                    target = vector + vector2.normalized * minimumEdgeDistance;
                }
            }
            if (!CanOccur(x + 1, z - 1, area))
            {
                Vector3 vector3 = new Vector3(maxX, target.y, minZ);
                Vector3 vector4 = target - vector3;
                if (vector4.sqrMagnitude < minimumEdgeDistance * minimumEdgeDistance)
                {
                    if (vector4.sqrMagnitude < 1f)
                    {
                        vector4 = new Vector3(-1f, 0f, 1f);
                    }
                    target = vector3 + vector4.normalized * minimumEdgeDistance;
                }
            }
            if (!CanOccur(x - 1, z + 1, area))
            {
                Vector3 vector5 = new Vector3(minX, target.y, maxZ);
                Vector3 vector6 = target - vector5;
                if (vector6.sqrMagnitude < minimumEdgeDistance * minimumEdgeDistance)
                {
                    if (vector6.sqrMagnitude < 1f)
                    {
                        vector6 = new Vector3(1f, 0f, -1f);
                    }
                    target = vector5 + vector6.normalized * minimumEdgeDistance;
                }
            }
            if (!CanOccur(x + 1, z + 1, area))
            {
                Vector3 vector7 = new Vector3(maxX, target.y, maxZ);
                Vector3 vector8 = target - vector7;
                if (vector8.sqrMagnitude < minimumEdgeDistance * minimumEdgeDistance)
                {
                    if (vector8.sqrMagnitude < 1f)
                    {
                        vector8 = new Vector3(-1f, 0f, -1f);
                    }
                    target = vector7 + vector8.normalized * minimumEdgeDistance;
                }
            }
        }

        public bool CanOccur(int x, int z, OccurrenceAreas area)
        {
            GameAreaManager gam = Singleton<GameAreaManager>.instance;
            switch (area)
            {
                case OccurrenceAreas.InnerArea:
                    return x >= 0 && z >= 0 && x < 5 && z < 5 && gam.m_areaGrid[z * 5 + x] != 0; // Game default
                case OccurrenceAreas.OuterArea:
                    return x >= 0 && z >= 0 && x < 5 && z < 5 && gam.m_areaGrid[z * 5 + x] == 0;
                case OccurrenceAreas.Everywhere:
                    return x >= 0 && z >= 0 && x < 5 && z < 5;
                default:
                    return false;
            }
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
