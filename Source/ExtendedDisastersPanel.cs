using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace EnhancedDisastersMod
{
    public class ExtendedDisastersPanel : UIPanel
    {
        private UILabel[] labels = new UILabel[7];
        private UIProgressBar[] progressBars = new UIProgressBar[7];
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

            for (int i = 0; i < 7; i++)
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

            for (int i = 0; i < 7; i++)
            {
                EnhancedDisaster d = edm.AllDisasters[i];
                float p = d.GetCurrentOccurrencePerYear();
                labels[i].text = string.Format("{0}: {1:0.00}", d.GetName(), p);
                progressBars[i].value = p / d.GetMaximumOccurrencePerYear();
            }
        }
    }
}
