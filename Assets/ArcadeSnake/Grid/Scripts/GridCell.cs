using System;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class GridCell : MonoBehaviour, IDisposable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private IMemoryPool _pool;
        
        private int colorPosition;

        public void SetColor()
        { 
            colorPosition = (int)transform.position.x + (int)transform.position.y;
            if (colorPosition % 2 == 1)
            {
                var color = _spriteRenderer.color;
                color = new Color(color.r - 0.07f, color.g - 0.07f,
                    color.b - 0.07f, 1f);
                _spriteRenderer.color = color;
            }
        }

        public void OnSpawned( IMemoryPool pool)
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

        public class Pool : MonoMemoryPool<SceneObjectProtocol,GridCell>
        {
            protected override void Reinitialize(SceneObjectProtocol protocol, GridCell item)
            {
                item.ReInit(protocol);
                item.SetColor();
            }

            protected override void OnSpawned(GridCell item)
            {
                base.OnSpawned(item);
                item.OnSpawned(this);
            }
        }
    }
}
