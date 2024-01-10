using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class Food : MonoBehaviour, IPoolable<SceneObjectProtocol, IMemoryPool>
    {
        private IMemoryPool _pool;

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(SceneObjectProtocol p1, IMemoryPool p2)
        {
            _pool = p2;
            transform.position = p1.Position;
        }
    
        public void Delete()
        {
            Destroy(gameObject);
        }

        public class Factory : PlaceholderFactory<SceneObjectProtocol, Food>
        {
        
        }
    }
}