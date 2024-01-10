using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class GridController
    {
        private readonly IInstantiator _instantiator;
        private readonly GridConfig _config;

        public GridController(
            IInstantiator instantiator,
            GridConfig config)
        {
            _instantiator = instantiator;
            _config = config;


            for (int i = 0; i < _config.Width; i++)
            {
                for (int j = 0; j < _config.Height; j++)
                {
                    var protocol = new SceneObjectProtocol(new Vector3(i,j));
                    var command = _instantiator.Instantiate<GridCreateCommand>(new object[]{protocol});
                    command.Execute();
                }
            }
        }
    }
}


