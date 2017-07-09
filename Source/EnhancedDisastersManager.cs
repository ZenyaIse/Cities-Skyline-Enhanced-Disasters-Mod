using System.Text;
using System.Collections.Generic;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class EnhancedDisastersManager : Singleton<EnhancedDisastersManager>
    {
        public static bool IsDebug = true;

        public EnhancedForestFire ForestFire;
        public EnhancedThunderstorm Thunderstorm;
        public EnhancedTornado Tornado;
        public EnhancedEarthquake Earthquake;
        public EnhancedTsunami Tsunami;
        public EnhancedSinkhole Sinkhole;
        public EnhancedMeteorStrike MeteorStrike;
        public List<EnhancedDisaster> AllDisasters = new List<EnhancedDisaster>();
#if DEBUG
        private int framesCount = 0;
#endif

        public EnhancedDisastersManager()
        {
            AllDisasters.Add(ForestFire = new EnhancedForestFire());
            AllDisasters.Add(Thunderstorm = new EnhancedThunderstorm());
            AllDisasters.Add(Tornado = new EnhancedTornado());
            AllDisasters.Add(Earthquake = new EnhancedEarthquake());
            AllDisasters.Add(Tsunami = new EnhancedTsunami());
            AllDisasters.Add(Sinkhole = new EnhancedSinkhole());
            AllDisasters.Add(MeteorStrike = new EnhancedMeteorStrike());
        }

        public void OnSimulationFrame()
        {
            AllDisasters.ForEach(x => x.OnSimulationFrame());
#if DEBUG
            if (--framesCount <= 0)
            {
                framesCount = 4096; // One week

                StringBuilder sb = new StringBuilder();
                sb.Append(">>> EnhancedDisastersMod: " + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString());
                AllDisasters.ForEach(x => sb.Append("\n" + x.GetType().Name + ": " + x.GetCurrentOccurrencePerYear().ToString()));

                Debug.Log(sb.ToString());
            }
#endif
        }

        public void OnDisasterCreated(DisasterAI dai, byte intensity)
        {
            foreach (EnhancedDisaster ed in AllDisasters)
            {
                if (ed.CheckDisasterAIType(dai))
                {
                    ed.OnDisasterCreated(intensity);
                    return;
                }
            }
        }

        public void OnDisasterStarted(DisasterAI dai, byte intensity)
        {
            foreach (EnhancedDisaster ed in AllDisasters)
            {
                if (ed.CheckDisasterAIType(dai))
                {
                    ed.OnDisasterStarted(intensity);
                    return;
                }
            }
        }

        public static DisasterInfo GetDisasterInfo(DisasterType disasterType)
        {
            int prefabCount = PrefabCollection<DisasterInfo>.PrefabCount();

            for (int i = 0; i < prefabCount; i++)
            {
                DisasterInfo disasterInfo = PrefabCollection<DisasterInfo>.GetPrefab((uint)i);
                if (disasterInfo != null)
                {
                    switch (disasterType)
                    {
                        case DisasterType.Earthquake:
                            if (disasterInfo.m_disasterAI as EarthquakeAI != null) return disasterInfo;
                            break;
                        case DisasterType.ForestFire:
                            if (disasterInfo.m_disasterAI as ForestFireAI != null) return disasterInfo;
                            break;
                        case DisasterType.MeteorStrike:
                            if (disasterInfo.m_disasterAI as MeteorStrikeAI != null) return disasterInfo;
                            break;
                        case DisasterType.ThunderStorm:
                            if (disasterInfo.m_disasterAI as ThunderStormAI != null) return disasterInfo;
                            break;
                        case DisasterType.Tornado:
                            if (disasterInfo.m_disasterAI as TornadoAI != null) return disasterInfo;
                            break;
                        case DisasterType.Tsunami:
                            if (disasterInfo.m_disasterAI as TsunamiAI != null) return disasterInfo;
                            break;
                        case DisasterType.StructureCollapse:
                            if (disasterInfo.m_disasterAI as StructureCollapseAI != null) return disasterInfo;
                            break;
                        case DisasterType.StructureFire:
                            if (disasterInfo.m_disasterAI as StructureFireAI != null) return disasterInfo;
                            break;
                        case DisasterType.Sinkhole:
                            if (disasterInfo.m_disasterAI as SinkholeAI != null) return disasterInfo;
                            break;
                    }
                }
            }

            return null;
        }
    }
}
