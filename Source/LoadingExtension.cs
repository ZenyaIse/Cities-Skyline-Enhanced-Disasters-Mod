using ColossalFramework;
using ICities;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
            {
                Singleton<EnhancedDisastersManager>.instance.CreateExtendedDisasterPanel();
                Singleton<EnhancedDisastersManager>.instance.CheckUnlocks();

                setDisasterProperties();
            }
        }

        private void setDisasterProperties()
        {
            int prefabsCount = PrefabCollection<DisasterInfo>.PrefabCount();

            for (uint i = 0; i < prefabsCount; i++)
            {
                DisasterInfo di = PrefabCollection<DisasterInfo>.GetPrefab(i);
                if (di == null) continue;

                if (di.m_disasterAI as EarthquakeAI != null)
                {
                    Debug.Log(string.Format("m_crackLength = {0}, m_crackWidth = {1}", ((EarthquakeAI)di.m_disasterAI).m_crackLength, ((EarthquakeAI)di.m_disasterAI).m_crackWidth));
                    ((EarthquakeAI)di.m_disasterAI).m_crackLength = 0;
                    ((EarthquakeAI)di.m_disasterAI).m_crackWidth = 0;
                }
            }
        }
    }
}
