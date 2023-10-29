using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class GridInstaller : MonoInstaller<GridInstaller>
    {
        [SerializeField] private GridCell _gridCellPrototype;
        [SerializeField] private GridConfig _config;
        public override void InstallBindings()
        {
            Container
                .Bind<GridConfig>()
                .FromInstance(_config)
                .AsSingle();
        
            Container
                .BindMemoryPool<GridCell, GridCell.Pool>()
                .WithInitialSize((_config.Width-1)*(_config.Height-1))
                .FromComponentInNewPrefab(_gridCellPrototype)
                .UnderTransformGroup("Grid");
      
            Container
                .Bind<GridController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
