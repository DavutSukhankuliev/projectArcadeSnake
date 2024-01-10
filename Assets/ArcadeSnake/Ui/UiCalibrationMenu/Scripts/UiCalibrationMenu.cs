using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeSnake
{
    public class UiCalibrationMenu : UICanvasWindow
    {
        [SerializeField] protected internal Text TextInstruction;
        [SerializeField] protected internal Image ImageHandLeft;
        [SerializeField] protected internal Image ImageHandRight;
        [SerializeField] protected internal Text TextHandLeft;
        [SerializeField] protected internal Text TextHandRight;
        [SerializeField] protected internal Toggle[] Marks = new Toggle[6];

        [SerializeField] protected internal Transform WindowStartCalibration;
        [SerializeField] private Button _buttonMainMenu;
        [SerializeField] private Button _buttonStartCalibration;

        [SerializeField] protected internal Transform WindowSaveNewCalibration;
        [SerializeField] private Button _buttonYes;
        [SerializeField] private Button _buttonNo;
        
        public EventHandler OnStartCalibrationClickEvent
        {
            get;
            set;
        }
        public EventHandler OnMainMenuClickEvent
        {
            get;
            set;
        }
        public EventHandler OnYesClickEvent
        {
            get;
            set;
        }
        public EventHandler OnNoClickEvent
        {
            get;
            set;
        }

        private void OnStartCalibrationClickEventHandler()
        {
            OnStartCalibrationClickEvent?.Invoke(this,EventArgs.Empty);
        }
        private void OnMainMenuClickEventHandler()
        {
            OnMainMenuClickEvent?.Invoke(this, EventArgs.Empty);
        }
        private void OnYesClickEventHandler()
        {
            OnYesClickEvent?.Invoke(this,EventArgs.Empty);
        }
        private void OnNoClickEventHandler()
        {
            OnNoClickEvent?.Invoke(this,EventArgs.Empty);
        }

        public override void Show()
        {
            ShowEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(true);
            
            _buttonStartCalibration.onClick.AddListener(OnStartCalibrationClickEventHandler);
            _buttonMainMenu.onClick.AddListener(OnMainMenuClickEventHandler);
            _buttonYes.onClick.AddListener(OnYesClickEventHandler);
            _buttonNo.onClick.AddListener(OnNoClickEventHandler);
        }

        public override void Hide()
        {
            HideEvent?.Invoke(this,EventArgs.Empty);
            gameObject.SetActive(false);
            
            _buttonStartCalibration.onClick.RemoveListener(OnStartCalibrationClickEventHandler);
            _buttonMainMenu.onClick.RemoveListener(OnMainMenuClickEventHandler);
            _buttonYes.onClick.RemoveListener(OnYesClickEventHandler);
            _buttonNo.onClick.RemoveListener(OnNoClickEventHandler);
        }

        public void WindowShow(Transform transformGameObject)
        { 
            transformGameObject.gameObject.SetActive(true);
        }
        public void WindowHide(Transform transformGameObject)
        { 
            transformGameObject.gameObject.SetActive(false);
        }
    }
}