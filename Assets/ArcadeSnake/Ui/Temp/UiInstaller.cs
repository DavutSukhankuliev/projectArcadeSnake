using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class UiInstaller : MonoInstaller<UiInstaller>
    {
        [SerializeField] private UiConfig _uiConfig;
        public override void InstallBindings()
        {
            Container
                .Bind<ArcadeMachineSystem>()
                .AsSingle()
                .NonLazy();
        
            Container
                .Bind<UiConfig>()
                .FromScriptableObject(_uiConfig)
                .AsSingle();

            Container
                .BindFactory<UiProtocol, UiController, UiController.Factory>();
        }
    }
}