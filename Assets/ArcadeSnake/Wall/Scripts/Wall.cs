using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class Wall : MonoBehaviour, IDisposable, IPoolable<SceneObjectProtocol, IMemoryPool>
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

        public void Dispose()
        {
            Destroy(gameObject);
        }

        public class Factory : PlaceholderFactory<SceneObjectProtocol, Wall>
        {
        
        }
    }
}