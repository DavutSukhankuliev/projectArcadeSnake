using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class Back : MonoBehaviour, IDisposable
    {
        private IMemoryPool _pool;

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            _pool = null;
        }


        public void ReInit(SceneObjectProtocol protocol)
        {
            transform.position = protocol.Position;
        }

        public class Pool : MonoMemoryPool<SceneObjectProtocol, Back>
        {
            protected override void Reinitialize(SceneObjectProtocol protocol, Back item)
            {
                item.ReInit(protocol);
            }

            protected override void OnSpawned(Back item)
            {
                base.OnSpawned(item);
                item.OnSpawned(this);
            }
        }
        
    }
}
