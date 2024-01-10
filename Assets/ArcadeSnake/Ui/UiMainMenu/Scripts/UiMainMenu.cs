using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeSnake
{
    public class UiMainMenu : UICanvasWindow
    {
        [SerializeField] public Button _buttonPlay;
        [SerializeField] private Button _buttonCalibration;
        [SerializeField] protected internal Toggle _toggleAudioMute;

        public EventHandler OnPlayClickEvent
        {
            get;
            set;
        }

        public EventHandler OnCalibrationClickEvent
        {
            get; 
            set;
        }

        public EventHandler<bool> OnAudioToggleValueChangedEvent
        {
            get; 
            set;
        }

        private void OnPlayClickEventHandler()
        {
            OnPlayClickEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnCalibrationClickEventHandler()
        {
            OnCalibrationClickEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnAudioToggleValueChangedEventHandler(bool e)
        {
            OnAudioToggleValueChangedEvent?.Invoke(this, e);
        }
        public override void Show()
        {
            ShowEvent?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(true);

            _buttonPlay.onClick.AddListener(OnPlayClickEventHandler);
            _buttonCalibration.onClick.AddListener(OnCalibrationClickEventHandler);
            _toggleAudioMute.onValueChanged.AddListener(OnAudioToggleValueChangedEventHandler);
        }

        public override void Hide()
        {
            HideEvent?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);

            _buttonPlay.onClick.RemoveListener(OnPlayClickEventHandler);
            _buttonCalibration.onClick.RemoveListener(OnCalibrationClickEventHandler);
            _toggleAudioMute.onValueChanged.RemoveListener(OnAudioToggleValueChangedEventHandler);
        }
    }
}