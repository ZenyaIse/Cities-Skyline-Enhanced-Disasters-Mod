using ICities;
using ColossalFramework;

namespace EnhancedDisastersMod
{
    public class EnhancedDisastersManager : Singleton<EnhancedDisastersManager>
    {
        public DisastersContainer container;

        private EnhancedDisastersManager()
        {
            container = DisastersContainer.CreateFromFile();
            if (container == null)
            {
                container = new DisastersContainer();
            }

            container.CheckObjects();
        }

        public void OnSimulationFrame()
        {
            CheckUnlocks();

            foreach (EnhancedDisaster ed in container.AllDisasters)
            {
                ed.OnSimulationFrame();
            }
        }

        public void OnDisasterCreated(DisasterAI dai, byte intensity)
        {
            foreach (EnhancedDisaster ed in container.AllDisasters)
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
            foreach (EnhancedDisaster ed in container.AllDisasters)
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

        public void CheckUnlocks()
        {
            string currentMilestoneName = Singleton<UnlockManager>.instance.GetCurrentMilestone().name;

            int milestoneNum;
            if (!int.TryParse(currentMilestoneName.Substring(9), out milestoneNum))
            {
                milestoneNum = 99; // Unlock all disasters if can not read the milestone number
            }

            if (milestoneNum >= 3) container.ForestFire.Unlock();
            if (milestoneNum >= 3) container.Thunderstorm.Unlock();
            if (milestoneNum >= 4) container.Sinkhole.Unlock();
        }
    }
}
