using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class MiographNetworkInstaller : MonoInstaller
    {
        [SerializeField] private MiographSettings _miographSettings;

        public override void InstallBindings()
        {
            Container
                .Bind<MiographSettings>()
                .FromScriptableObject(_miographSettings)
                .AsSingle();

            Container
                .Bind<MiographNetworkController>()
                .AsSingle()
                .NonLazy();
        }
    }
}