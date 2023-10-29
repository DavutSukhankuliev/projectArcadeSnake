using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeSnake
{
    public class UiPauseMenu : UICanvasWindow
    {
        [SerializeField] private Button _buttonUnPause;
        [SerializeField] private Button _buttonRetry;
        [SerializeField] private Button _buttonMainMenu;
        
        [SerializeField] protected internal Toggle _toggleAudioMute;
        
        public EventHandler OnUnPauseClickEvent
        {
            get;
            set;
        }
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

        public EventHandler<bool> OnAudioToggleValueChangedEvent
        {
            get;
            set;
        }

        private void OnUnPauseClickEventHandler()
        {
            OnUnPauseClickEvent?.Invoke(this,EventArgs.Empty);
        }
        private void OnRetryClickEventHandler()
        {
            OnRetryClickEvent?.Invoke(this,EventArgs.Empty);
        }
        private void OnMainMenuClickEventHandler()
        {
            OnMainMenuClickEvent?.Invoke(this,EventArgs.Empty);
        }
       
        private void OnAudioToggleValueChangedEventHandler(bool e)
        {
           OnAudioToggleValueChangedEvent?.Invoke(this, e);
        }
       
        public override void Show()
        {
            ShowEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(true);
            
            _buttonUnPause.onClick.AddListener(OnUnPauseClickEventHandler);
            _buttonRetry.onClick.AddListener(OnRetryClickEventHandler);
            _buttonMainMenu.onClick.AddListener(OnMainMenuClickEventHandler);
            _toggleAudioMute.onValueChanged.AddListener(OnAudioToggleValueChangedEventHandler);
        }

        public override void Hide()
        {
            HideEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(false);
            
            _buttonUnPause.onClick.RemoveListener(OnUnPauseClickEventHandler);
            _buttonRetry.onClick.RemoveListener(OnRetryClickEventHandler);
            _buttonMainMenu.onClick.RemoveListener(OnMainMenuClickEventHandler);
            _toggleAudioMute.onValueChanged.AddListener(OnAudioToggleValueChangedEventHandler);
        }
    }
    
}