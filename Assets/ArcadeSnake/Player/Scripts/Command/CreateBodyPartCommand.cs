using UnityEngine;

namespace ArcadeSnake
{
    public class CreateBodyPartCommand :Command
    {
        private readonly Snake.Pool _pool;
        private readonly SceneObjectProtocol _protocol;

        private Snake _bodyScript;
        private SpriteRenderer _sprite;
     
        public CreateBodyPartCommand(Snake.Pool pool,
            SceneObjectProtocol protocol)
        {
            _pool = pool;
            _protocol = protocol;
            _bodyScript = SpawnPart();
        }
    
        public Snake SpawnPart()
        {
            var body =  _pool.Spawn(_protocol);
            Execute();
            return body;
        }
 
        public override void Execute()
        {
            OnDone();
        }

        public Snake GetBody()
        {
            return _bodyScript;
        }
    }
}
