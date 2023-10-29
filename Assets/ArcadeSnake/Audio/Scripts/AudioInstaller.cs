using UnityEngine;
using Zenject;

namespace ArcadeSnake {
    public class AudioInstaller : MonoInstaller<AudioInstaller>
    {
        [SerializeField] private AudioSourceController _audioSourceController;
        [SerializeField] private AudioConfig _audioConfig;
    
        public override void InstallBindings()
        {
            Container
                .Bind<AudioConfig>()
                .FromScriptableObject(_audioConfig)
                .AsSingle();
        
            Container
                .BindFactory<AudioSourceController, AudioSourceController.Factory>()
                .FromComponentInNewPrefab(_audioSourceController);
        
            Container
                .Bind<AudioPlayerController>()
                .AsSingle();
        }
    }
}
