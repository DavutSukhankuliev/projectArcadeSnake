using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class BodyScript : MonoBehaviour, IDisposable, IPoolable<SceneObjectProtocol, IMemoryPool>
    {
        private IMemoryPool _pool;

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
           _pool = null;
        }

        public void OnSpawned(SceneObjectProtocol p1, IMemoryPool p2)
        {
            _pool = p2;
            transform.position = p1.Position;
        }
        public class Factory : PlaceholderFactory<SceneObjectProtocol, BodyScript>
        {
        
        }
    }
}
