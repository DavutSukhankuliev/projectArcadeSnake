using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class FoodInstaller : MonoInstaller<FoodInstaller>
    {
       [SerializeField] private FoodProtocolConfig protocolConfig;
        
        public override void InstallBindings()
        {
            Container
                .Bind<FoodProtocolConfig>()
                .FromScriptableObject(protocolConfig)
                .AsSingle();

            Container
                .Bind<FoodController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
