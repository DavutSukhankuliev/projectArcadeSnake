using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class SnakeInstaller : MonoInstaller<SnakeInstaller>
    {
        [SerializeField] private Snake _snakeHeadPrototype;
        [SerializeField] private Snake _snakeBodyPrototype;
        [SerializeField] private SnakeConfig _config;
        [SerializeField] private SnakeProtocolConfig _protocolConfig;
    
        public override void InstallBindings()
        {
            Container
                .Bind<SnakeConfig>()
                .FromInstance(_config)
                .AsSingle();
      
            Container
                .Bind<SnakeProtocolConfig>()
                .FromScriptableObject(_protocolConfig)
                .AsSingle();

            Container
                .BindFactory<SceneObjectProtocol, Snake, Snake.Factory>()
                .FromMonoPoolableMemoryPool(
                    x => x.FromComponentInNewPrefab(_snakeHeadPrototype));
        
            Container
                .BindFactory<SceneObjectProtocol, BodyScript, BodyScript.Factory>()
                .FromMonoPoolableMemoryPool(
                    x => x.FromComponentInNewPrefab(_snakeBodyPrototype));

            Container
                .BindMemoryPool<Snake, Snake.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(_snakeBodyPrototype)
                .UnderTransformGroup("BodyParts");

            Container
                .BindInterfacesAndSelfTo<SnakeMovement>()
                .AsSingle()
                .NonLazy();
        }
    }
}
