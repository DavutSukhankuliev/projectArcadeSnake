using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class BackController
    {
        private readonly IInstantiator _instantiator;

        public BackController(
            IInstantiator instantiator)
        {
            _instantiator = instantiator;

            var protocol = new SceneObjectProtocol(new Vector3(8,4,0));
            var command = _instantiator.Instantiate<BackCreateCommand>(new object[]{protocol});
            command.Execute();
       
        }
    }
}
