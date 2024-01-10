using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class WallInstaller : MonoInstaller<FoodInstaller>
    {
        [SerializeField] private Wall wallPrototype;
        
        [SerializeField] private WallProtocolConfig protocolConfig;
        [SerializeField] private WallConfig wallConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<WallConfig>()
                .FromScriptableObject(wallConfig)
                .AsSingle();
            
            Container
                .Bind<WallProtocolConfig>()
                .FromScriptableObject(protocolConfig)
                .AsSingle();
            
            Container
                .BindFactory<SceneObjectProtocol, Wall, Wall.Factory>()
                .FromMonoPoolableMemoryPool(
                    x => x.FromComponentInNewPrefab(wallPrototype));

            Container
                .Bind<WallController>()
                .AsSingle()
                .NonLazy();
        }
    }
}