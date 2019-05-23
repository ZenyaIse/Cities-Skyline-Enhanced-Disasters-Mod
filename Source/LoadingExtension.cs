using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private ExtendedDisastersPanel dPanel;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
            {
                createExtendedDisasterPanel();
                Singleton<EnhancedDisastersManager>.instance.CheckUnlocks();

                setDisasterProperties();
            }
        }

        private void setDisasterProperties()
        {
            int prefabsCount = PrefabCollection<DisasterInfo>.PrefabCount();
            Debug.Log("Disaster prebabsCount = " + prefabsCount);

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

        private void createExtendedDisasterPanel()
        {
            if (dPanel != null) return;

            UIView v = UIView.GetAView();

            GameObject obj = new GameObject("ExtendedDisastersPanel");
            obj.transform.parent = v.cachedTransform;
            dPanel = obj.AddComponent<ExtendedDisastersPanel>();
            dPanel.absolutePosition = new Vector3(v.fixedWidth - 420, 110);

            GameObject toggleButtonObject = new GameObject("ExtendedDisastersPanelButton");
            toggleButtonObject.transform.parent = v.transform;
            toggleButtonObject.transform.localPosition = Vector3.zero;
            UIButton toggleButton = toggleButtonObject.AddComponent<UIButton>();
            toggleButton.normalBgSprite = "InfoIconBasePressed";
            toggleButton.normalFgSprite = "InfoIconElectricity";
            toggleButton.width = 30f;
            toggleButton.height = 30f;
            toggleButton.absolutePosition = new Vector3(v.fixedWidth - 50, 70);
            toggleButton.tooltip = "Extended Disasters";
            toggleButton.eventClick += ToggleButton_eventClick;
        }

        private void ToggleButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            dPanel.isVisible = !dPanel.isVisible;

            if (dPanel.isVisible)
            {
                dPanel.Counter = 0;
            }
        }
    }
}
