using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeSnake
{
    public class UiHud : UICanvasWindow
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] protected internal Text TextScore;
        [SerializeField] protected internal Text TextHighscore;
        
        protected internal readonly string ScoreName = "Score:";
        protected internal readonly string HighscoreName = "Highscore: ";

        public EventHandler OnPauseClickEvent
        {
            get;
            set;
        }

        private void OnPauseClickEventHandler()
        {
            OnPauseClickEvent?.Invoke(this,EventArgs.Empty);
        }

        public override void Show()
        {
            ShowEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(true);
            
            pauseButton.onClick.AddListener(OnPauseClickEventHandler);
        }

        public override void Hide()
        {
            HideEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(false);
            
            pauseButton.onClick.RemoveListener(OnPauseClickEventHandler);
        }
    }
}