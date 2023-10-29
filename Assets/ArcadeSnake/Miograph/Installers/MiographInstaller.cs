using Miograph.Animation;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class MiographInstaller : MonoInstaller
    {        
        [SerializeField] private AnimSettingsObject animSettingsObject;        

        public override void InstallBindings()
        {            
            Container
                .Bind<AnimSettingsObject>()
                .FromScriptableObject(animSettingsObject)
                .AsSingle();

            Container
                .BindFactory<int, int, IMiographDataSource, MiographSceneController, MiographSceneController.Factory>();
        }
    }
}