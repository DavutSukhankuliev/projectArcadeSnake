using System.Collections.Generic;

namespace ArcadeSnake
{
    public class ArcadeMachineSystem
    {
        private UiConfig _config;
        private Dictionary<int, UiController> _dict = new Dictionary<int, UiController>();
        private UiController.Factory _controllerFactory;
    
        public ArcadeMachineSystem(
            UiConfig config,
            UiController.Factory controllerFactory
        )
        {        
            _config = config;
            _controllerFactory = controllerFactory;
        
            foreach (var id in _config.ActiveMachines)
            {
                var b = _config.Get(id);            
                var protocol = new UiProtocol(b.ID, b.Position, b.Rotation, b.ViewPrefab, b.FollowCamera);            
                _dict.Add(id, _controllerFactory.Create(protocol));
            }        
        }
   
        public UiController Get(int id)
        {        
            return _dict[id];
        }
    }
}