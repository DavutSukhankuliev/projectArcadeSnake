using UnityEngine;
using Zenject;

namespace ArcadeSnake {
    public class UIFrameworkInstaller : MonoInstaller
    {
        [SerializeField] private UIRoot uiRootPrefab;

        public override void InstallBindings()
        {
            Container
                .Bind<IUIRoot>()
                .To<UIRoot>()
                .FromComponentInNewPrefab(uiRootPrefab)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IUIService>()
                .To<UIService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<UIController>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<UiHudController>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<UiDeathMenuController>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<UiMainMenuController>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<UiCalibrationMenuController>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<UiPauseMenuController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
