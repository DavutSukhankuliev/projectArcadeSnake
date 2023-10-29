using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeSnake
{
    public class InitUICommand : Command
    {
        private readonly IUIRoot _uIRoot;
        private readonly IUIService _uIService;
        private readonly UiHudController _uiController;
        private readonly UiDeathMenuController _uiDeathMenuController;
        private readonly UiCalibrationMenuController _uiCalibrationMenuController;

        public InitUICommand(IUIRoot uIRoot, IUIService uIService, UiHudController uiController, UiDeathMenuController uiDeathMenuController, UiCalibrationMenuController uiCalibrationMenuController)
        {
            _uIRoot = uIRoot;
            _uIService = uIService;
            _uiController = uiController;
            _uiDeathMenuController = uiDeathMenuController;
            _uiCalibrationMenuController = uiCalibrationMenuController;
        }

        public override void Execute()
        {
            _uIService.LoadWindows();
            var container = new GameObject("DeactivatedWindows");
            var containerRect = container.AddComponent<RectTransform>();
            containerRect.SetParent(_uIRoot.Container);
            containerRect.localScale = Vector3.one;
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            container.gameObject.SetActive(false);

            _uIService.InitWindows(containerRect);

            _uiController.InitHud();
            _uiDeathMenuController.InitDeathMenu();
            _uiCalibrationMenuController.InitCalibrationMenu();
            
            OnDone();
        }

       
    }
}
