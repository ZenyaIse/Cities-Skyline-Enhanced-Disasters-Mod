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
        public EnhancedEarthquake Earthquake;
        public EnhancedMeteorStrike MeteorStrike;
        public EnhancedTornado Tornado;
        public EnhancedTsunami Tsunami;
        public EnhancedSinkhole Sinkhole;

        private int framesCount = 0;

        public EnhancedDisastersManager()
        {
            ForestFire = new EnhancedForestFire();
            Thunderstorm = new EnhancedThunderstorm();
            Tornado = new EnhancedTornado();
            MeteorStrike = new EnhancedMeteorStrike();
            Earthquake = new EnhancedEarthquake();
            Tsunami = new EnhancedTsunami();
            Sinkhole = new EnhancedSinkhole();

            //Tsunami.Enabled = false;
            //Earthquake.Enabled = false;
        }

        public void OnSimulationFrame()
        {
            ForestFire.OnSimulationFrame();
            Thunderstorm.OnSimulationFrame();
            Tornado.OnSimulationFrame();
            MeteorStrike.OnSimulationFrame();
            Earthquake.OnSimulationFrame();
            Tsunami.OnSimulationFrame();
            Sinkhole.OnSimulationFrame();

            if (--framesCount <= 0)
            {
                framesCount = 5000;
                Debug.Log(">>> EnhancedDisastersMod: " + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString()
                     + "\nForestFire: " + ForestFire.GetCurrentOccurrencePerYear().ToString()
                     + "\nThunderstorm: " + Thunderstorm.GetCurrentOccurrencePerYear().ToString()
                     + "\nTornado: " + Tornado.GetCurrentOccurrencePerYear().ToString()
                     + "\nMeteorStrike: " + MeteorStrike.GetCurrentOccurrencePerYear().ToString()
                     + "\nEarthquake: " + Earthquake.GetCurrentOccurrencePerYear().ToString()
                     + "\nTsunami: " + Tsunami.GetCurrentOccurrencePerYear().ToString()
                     + "\nSinkhole: " + Sinkhole.GetCurrentOccurrencePerYear().ToString()
                    );
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
