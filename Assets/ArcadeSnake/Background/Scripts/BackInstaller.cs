using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class BackInstaller : MonoInstaller<BackInstaller>
    {
        [SerializeField] private Back _backPrototype;

        public override void InstallBindings()
        {
            Container
                .BindMemoryPool<Back, Back.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(_backPrototype)
                .UnderTransformGroup("BackGround");

            Container
                .Bind<BackController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
