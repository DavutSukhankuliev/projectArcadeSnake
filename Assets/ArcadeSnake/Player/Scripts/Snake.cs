using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class Snake : MonoBehaviour, IDisposable, IPoolable<SceneObjectProtocol, IMemoryPool>
    {
        [SerializeField] private GameObject _starsAnimation;
        public event EventHandler<Collider2D> EatingColliderNotify;
    
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
        private void ReInit(SceneObjectProtocol protocol)
        {
            transform.position = protocol.Position;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            EatingColliderNotify?.Invoke(this, other);
        }
        public void StartHeadache()
        {
            _starsAnimation.SetActive(true);
        } 
        public void StopHeadache()
        {
            _starsAnimation.SetActive(false);
        }
        
    
        public class Pool : MonoMemoryPool<SceneObjectProtocol,Snake>
        {
            protected override void Reinitialize(SceneObjectProtocol protocol, Snake item)
            {
                item.ReInit(protocol);
            }
        }

        public class Factory : PlaceholderFactory<SceneObjectProtocol, Snake>
        {
        
        }
    }
}
