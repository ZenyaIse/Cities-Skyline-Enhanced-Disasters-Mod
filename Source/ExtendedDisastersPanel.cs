using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace EnhancedDisastersMod
{
    public class ExtendedDisastersPanel : UIPanel
    {
        private UILabel[] labels;
        private UIProgressBar[] progressBars;
        public int Counter = 0;

        public override void Awake()
        {
            base.Awake();

            //this.backgroundSprite = "GenericPanel";
            //this.color = new Color32(255, 0, 0, 100);
            this.backgroundSprite = "MenuPanel";
            this.canFocus = true;
            //this.isInteractive = true;

            height = 300;
            width = 170;

            isVisible = false;
        }

        public override void Start()
        {
            base.Start();

            UILabel lTitle = this.AddUIComponent<UILabel>();
            lTitle.position = new Vector3(10, -5);
            lTitle.text = "Disasters\nProbability";

            int y = -50;
            int h = -35;

            int disasterCount = Singleton<EnhancedDisastersManager>.instance.container.AllDisasters.Count;
            labels = new UILabel[disasterCount];
            progressBars = new UIProgressBar[disasterCount];
            for (int i = 0; i < disasterCount; i++)
            {
                labels[i] = addLabel(y);
                progressBars[i] = addProgressBar(y - 11);
                y += h;
            }

            UIButton btn = this.AddUIComponent<UIButton>();
            btn.position = new Vector3(130, -5);
            btn.size = new Vector2(30, 30);
            btn.normalFgSprite = "buttonclose";
            btn.eventClick += Btn_eventClick;
        }

        private void Btn_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.Hide();
        }

        private UILabel addLabel(int y)
        {
            UILabel l = this.AddUIComponent<UILabel>();
            l.position = new Vector3(10, y);
            l.textScale = 0.7f;

            return l;
        }

        private UIProgressBar addProgressBar(int y)
        {
            UIProgressBar b = this.AddUIComponent<UIProgressBar>();
            b.backgroundSprite = "LevelBarBackground";
            b.progressSprite = "LevelBarForeground";
            b.progressColor = Color.red;
            b.position = new Vector3(10, y);
            b.width = 150;
            b.value = 0.5f;

            return b;
        }

        public override void Update()
        {
            base.Update();

            if (!isVisible) return;

            if (--Counter > 0) return;
            Counter = 300;

            EnhancedDisastersManager edm = Singleton<EnhancedDisastersManager>.instance;
            int disasterCount = edm.container.AllDisasters.Count;

            for (int i = 0; i < disasterCount; i++)
            {
                EnhancedDisaster d = edm.container.AllDisasters[i];
                float p = d.CooldownCounter > 0 ? 0 : d.GetCurrentOccurrencePerYear();
                string text;
                if (d.Enabled)
                {
                    if (!d.Unlocked && d.OccurrenceBeforeUnlock == OccurrenceAreas.OuterArea)
                    {
                        text = string.Format("{0}: ({1:0.00})", d.GetName(), p);
                    }
                    else
                    {
                        text = string.Format("{0}: {1:0.00}", d.GetName(), p);
                    }
                }
                else
                {
                    text = "Disabled";
                }
                labels[i].text = text;
                progressBars[i].value = p / d.GetMaximumOccurrencePerYear();
            }
        }
    }
}
