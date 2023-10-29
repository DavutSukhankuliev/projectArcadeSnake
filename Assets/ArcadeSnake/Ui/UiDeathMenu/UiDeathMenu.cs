using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeSnake
{
    public class UiDeathMenu : UICanvasWindow
    {
        [SerializeField] private Button _buttonRetry;
        [SerializeField] private Button _buttonMainMenu;
        
        public EventHandler OnRetryClickEvent
        {
            get;
            set;
        }
        public EventHandler OnMainMenuClickEvent
        {
            get;
            set;
        }
        
        private void OnRetryClickEventHandler()
        {
            OnRetryClickEvent?.Invoke(this,EventArgs.Empty);
        }
        private void OnMainMenuClickEventHandler()
        {
            OnMainMenuClickEvent?.Invoke(this,EventArgs.Empty);
        }
        
        public override void Show()
        {
            ShowEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(true);
            
            _buttonMainMenu.onClick.AddListener(OnMainMenuClickEventHandler);
            _buttonRetry.onClick.AddListener(OnRetryClickEventHandler);
        }

        public override void Hide()
        {
            HideEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(false);
            
            _buttonMainMenu.onClick.RemoveListener(OnMainMenuClickEventHandler);
            _buttonRetry.onClick.RemoveListener(OnRetryClickEventHandler);
        }
    }
    
}