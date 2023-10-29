using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class SettingsInstaller : MonoInstaller
    {
        [SerializeField] private GameSettingsConfig _gameSettingsConfig;
        [SerializeField] private GlobalFoodSettings _globalFoodSettings;

        public override void InstallBindings()
        {
            Container
                .Bind<GameSettingsConfig>()
                .FromScriptableObject(_gameSettingsConfig)
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<GlobalFoodSettings>()
                .FromScriptableObject(_globalFoodSettings)
                .AsSingle()
                .NonLazy();
        }
    }
}