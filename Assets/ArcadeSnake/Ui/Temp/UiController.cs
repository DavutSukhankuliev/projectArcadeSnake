using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class UiController : ITickable
    {
        private IInstantiator _instantiator;
        private UiView _view;
        private TickableManager _tickableManager;
        private bool _followCamera;
           
        private Vector3 _velocity = Vector3.zero;
        
        public UiController(UiProtocol protocol, IInstantiator instantiator, TickableManager tickableManager)
        {        
            _view.Position = protocol.Position;
            _tickableManager = tickableManager;
            _tickableManager.Add(this);
            _instantiator = instantiator;        
            _view = _instantiator.InstantiatePrefabForComponent<UiView>(protocol.ViewPrefab);
            
            _followCamera = protocol.FollowCamera;
        }
        public void Tick()
        {
            if(_followCamera)
            {
            
            }
        }
        public class Factory: PlaceholderFactory<UiProtocol, UiController>
        {
        
        }    
    }
}