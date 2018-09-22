using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework.Plugins;
using System.Reflection;

namespace EnhancedDisastersMod
{
    public class Mod : IUserMod
    {
        private bool freezeUI = false;

        private UICheckBox ForestFireEnabledUI;
        private UISlider ForestFireMaxProbabilityUI;
        private UISlider ForestFireWarmupDaysUI;

        private UICheckBox ThunderstormEnabledUI;
        private UISlider ThunderstormMaxProbabilityUI;
        private UIDropDown ThunderstormMaxProbabilityMonthUI;
        private UISlider ThunderstormRainFactorUI;

        private UICheckBox UI_Sinkhole_Enabled;
        private UISlider UI_Sinkhole_MaxProbability;
        private UISlider UI_Sinkhole_GroundwaterCapacity;

        public string Name
        {
            get { return "Enhanced Disasters Mod"; }
        }

        public string Description
        {
            get { return "More natural behavior of natural disasters. Ver. 2018/08/27"; }
        }

        #region Options UI

        public static void UpdateUI()
        {
            foreach (PluginManager.PluginInfo current in Singleton<PluginManager>.instance.GetPluginsInfo())
            {
                if (current.isEnabled)
                {
                    IUserMod[] instances = current.GetInstances<IUserMod>();
                    MethodInfo method = instances[0].GetType().GetMethod("EnhancedDisastersOptionsUpdateUI", BindingFlags.Instance | BindingFlags.Public);
                    if (method != null)
                    {
                        method.Invoke(instances[0], new object[] { });
                        return;
                    }
                }
            }
        }

        private void EnhancedDisastersOptionsUpdateUI()
        {
            if (ForestFireEnabledUI == null) return;

            DisastersContainer c = Singleton<EnhancedDisastersManager>.instance.container;

            freezeUI = true;

            ForestFireEnabledUI.isChecked = c.ForestFire.Enabled;
            ForestFireMaxProbabilityUI.value = c.ForestFire.OccurrencePerYear;
            ForestFireWarmupDaysUI.value = c.ForestFire.WarmupDays;

            ThunderstormEnabledUI.isChecked = c.Thunderstorm.Enabled;
            ThunderstormMaxProbabilityUI.value = c.Thunderstorm.OccurrencePerYear;
            ThunderstormMaxProbabilityMonthUI.selectedIndex = c.Thunderstorm.MaxProbabilityMonth - 1;
            ThunderstormRainFactorUI.value = c.Thunderstorm.RainFactor;

            UI_Sinkhole_Enabled.isChecked = c.Sinkhole.Enabled;
            UI_Sinkhole_MaxProbability.value = c.Sinkhole.OccurrencePerYear;
            UI_Sinkhole_GroundwaterCapacity.value = c.Sinkhole.GroundwaterCapacity;

            freezeUI = false;
        }

        private void addLabelToSlider(object obj)
        {
            addLabelToSlider(obj, "");
        }

        private void addLabelToSlider(object obj, string postfix)
        {
            UISlider uISlider = obj as UISlider;
            if (uISlider == null) return;

            UILabel label = uISlider.parent.AddUIComponent<UILabel>();
            label.text = uISlider.value.ToString() + postfix;
            label.textScale = 1f;
            (uISlider.parent as UIPanel).autoLayout = false;
            label.position = new Vector3(uISlider.position.x + uISlider.width + 15, uISlider.position.y);

            UILabel titleLabel = (uISlider.parent as UIPanel).Find<UILabel>("Label");
            titleLabel.anchor = UIAnchorStyle.None;
            titleLabel.position = new Vector3(titleLabel.position.x, titleLabel.position.y + 3);

            uISlider.eventValueChanged += new PropertyChangedEventHandler<float>(delegate (UIComponent component, float value)
            {
                label.text = uISlider.value.ToString() + postfix;
            });
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            DisastersContainer c = Singleton<EnhancedDisastersManager>.instance.container;

            #region ForestFire
            UIHelperBase forestFireGroup = helper.AddGroup("Forest Fire disaster");
            ForestFireEnabledUI = (UICheckBox)forestFireGroup.AddCheckbox("Enable", c.ForestFire.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.ForestFire.Enabled = isChecked;
            });
            addLabelToSlider(ForestFireMaxProbabilityUI = (UISlider)forestFireGroup.AddSlider("Max probability", 1, 20, 1, c.ForestFire.OccurrencePerYear, delegate (float val)
            {
                if (!freezeUI) c.ForestFire.OccurrencePerYear = val;
            }), " times per year");
            addLabelToSlider(ForestFireWarmupDaysUI = (UISlider)forestFireGroup.AddSlider("Warmup period", 10, 360, 10, c.ForestFire.WarmupDays, delegate (float val)
            {
                if (!freezeUI) c.ForestFire.WarmupDays = (int)val;
            }), " days");

            helper.AddSpace(20);
            #endregion

            #region Thunderstorm
            UIHelperBase thunderstormGroup = helper.AddGroup("Thunderstorm disaster");
            ThunderstormEnabledUI = (UICheckBox)thunderstormGroup.AddCheckbox("Enable", c.Thunderstorm.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.Thunderstorm.Enabled = isChecked;
            });
            addLabelToSlider(ThunderstormMaxProbabilityUI = (UISlider)thunderstormGroup.AddSlider("Max probability", 0.1f, 5f, 0.1f, c.Thunderstorm.OccurrencePerYear, delegate (float val)
            {
                if (!freezeUI) c.Thunderstorm.OccurrencePerYear = val;
            }), " times per year");
            ThunderstormMaxProbabilityMonthUI = (UIDropDown)thunderstormGroup.AddDropdown("Most stormy month",
                new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },
                c.Thunderstorm.MaxProbabilityMonth - 1,
                delegate (int sel)
                {
                    if (!freezeUI) c.Thunderstorm.MaxProbabilityMonth = sel + 1;
                });
            addLabelToSlider(ThunderstormRainFactorUI = (UISlider)thunderstormGroup.AddSlider("During rain factor", 1f, 5f, 0.2f, c.Thunderstorm.RainFactor, delegate (float val)
            {
                if (!freezeUI) c.Thunderstorm.RainFactor = val;
            }));

            helper.AddSpace(20);
            #endregion

            #region Sinkhole
            UIHelperBase sinkholeGroup = helper.AddGroup("Sinkhole disaster");
            UI_Sinkhole_Enabled = (UICheckBox)sinkholeGroup.AddCheckbox("Enable", c.Sinkhole.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.Sinkhole.Enabled = isChecked;
            });
            addLabelToSlider(UI_Sinkhole_MaxProbability = (UISlider)sinkholeGroup.AddSlider("Max probability", 0.2f, 5, 0.2f, c.Sinkhole.OccurrencePerYear, delegate (float val)
            {
                if (!freezeUI) c.Sinkhole.OccurrencePerYear = val;
            }), " times per year");
            addLabelToSlider(UI_Sinkhole_GroundwaterCapacity = (UISlider)sinkholeGroup.AddSlider("Groundwater capacity", 1, 100, 1, c.Sinkhole.GroundwaterCapacity, delegate (float val)
            {
                if (!freezeUI) c.Sinkhole.GroundwaterCapacity = val;
            }));

            helper.AddSpace(20);
            #endregion

            // Save button
            helper.AddButton("Save", delegate ()
            {
                c.Save();
            });
        }

        #endregion
    }
}
