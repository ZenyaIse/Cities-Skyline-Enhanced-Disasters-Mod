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
        private List<EnhancedDisaster> allDisasters = new List<EnhancedDisaster>();

        private int framesCount = 0;

        public EnhancedDisastersManager()
        {
            allDisasters.Add(ForestFire = new EnhancedForestFire());
            allDisasters.Add(Thunderstorm = new EnhancedThunderstorm());
            allDisasters.Add(Tornado = new EnhancedTornado());
            allDisasters.Add(MeteorStrike = new EnhancedMeteorStrike());
            allDisasters.Add(Earthquake = new EnhancedEarthquake());
            allDisasters.Add(Tsunami = new EnhancedTsunami());
            allDisasters.Add(Sinkhole = new EnhancedSinkhole());
        }

        public void OnSimulationFrame()
        {
            allDisasters.ForEach(x => x.OnSimulationFrame());

            if (--framesCount <= 0)
            {
                framesCount = 4096; // One week

                StringBuilder sb = new StringBuilder();
                sb.Append(">>> EnhancedDisastersMod: " + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString());
                allDisasters.ForEach(x => sb.Append("\n" + x.GetType().Name + ": " + x.GetCurrentOccurrencePerYear().ToString()));

                Debug.Log(sb.ToString());
            }
        }

        public void OnDisasterCreated(DisasterAI dai, byte intensity)
        {
            foreach (EnhancedDisaster ed in allDisasters)
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
            foreach (EnhancedDisaster ed in allDisasters)
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
