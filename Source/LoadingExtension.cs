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
            Debug.Log("EnhancedDisastersMod Loaded: 2017/07/08");

            createExtendedDisasterPanel();

            Singleton<EnhancedDisastersManager>.instance.CheckUnlocks();
        }

        private void createExtendedDisasterPanel()
        {
            UIView v = UIView.GetAView();

            GameObject obj = new GameObject("ExtendedDisastersPanel");
            obj.transform.parent = v.cachedTransform;
            dPanel = obj.AddComponent<ExtendedDisastersPanel>();
            dPanel.absolutePosition = new Vector3(v.fixedWidth - 180, 150);

            GameObject toggleButtonObject = new GameObject("ExtendedDisastersPanelButton");
            toggleButtonObject.transform.parent = v.transform;
            toggleButtonObject.transform.localPosition = Vector3.zero;
            UIButton toggleButton = toggleButtonObject.AddComponent<UIButton>();
            toggleButton.normalBgSprite = "InfoIconBasePressed";
            toggleButton.normalFgSprite = "InfoIconElectricity";
            toggleButton.width = 30f;
            toggleButton.height = 30f;
            toggleButton.absolutePosition = new Vector3(v.fixedWidth - 90, 120);
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
