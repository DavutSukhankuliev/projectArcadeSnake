using UnityEngine;
using Zenject;

namespace ArcadeSnake {
    public class InputInstaller : MonoInstaller<InputInstaller>
    {

        [SerializeField] private MiographInputConfig miographInputConfig;
        public override void InstallBindings()
        {
            Container
                .Bind<MiographInputConfig>()
                .FromScriptableObject(miographInputConfig)
                .AsSingle();
            
            Container
                .Bind<InputState>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<MiographInputHandler>()
                .AsSingle()
                .NonLazy();
        }
    }
}
