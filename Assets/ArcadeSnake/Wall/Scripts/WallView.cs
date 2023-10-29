using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class WallView : MonoBehaviour, IDisposable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _collider2D;
        [SerializeField] private WallConfig wallConfig;
        
        private SnakeMovement _snakeMovement;
        private WallController _wallController;

        private float _alpha;
        private float _timerAddAlpha;
       
        private float _timerOfLive;
        
        private float _timerRemoveAlpha;
       
        private float _deltaAddAlpha;
        private float _deltaRemoveAlpha;

        private float _timer;
        private bool _isAlpha;

        private bool _playerIsDeath;

        [Inject]
        private void Inject (SnakeMovement snakeMovement,
            WallController wallController)
        {
            _snakeMovement = snakeMovement;
            _wallController = wallController;
            
            _snakeMovement.OnDeathEvent += PlayerDeath;

            _playerIsDeath = false;
        }

        private void PlayerDeath(object sender, EventArgs e)
        {
           _playerIsDeath = true;
        }


        private void Start()
        {
            _spriteRenderer.sprite = wallConfig.Sprite;
            
            _timerAddAlpha = wallConfig.TimeAddAlpha;
              
            _timerOfLive = wallConfig.TimeOfLive;
           
            _timerRemoveAlpha = wallConfig.TimeRemoveAlpha;
            
            _deltaAddAlpha = wallConfig.DeltaAddAlpha;
            _deltaRemoveAlpha = wallConfig.DeltaRemoveAlpha;
            
            _timer = _timerAddAlpha;
        }


        public void RemoveAlpha(float delta)
        {
            var color = _spriteRenderer.color;
            _alpha = color.a;
            _alpha -= delta;
            color = new Color(color.r, color.g, color.b, _alpha);
            _spriteRenderer.color = color;
            _timer = _timerRemoveAlpha;
            
            if (_alpha <= 0)
            {
                //_wallController.DestroyAllWalls();
                Dispose();
            }
        }
        public void AddAlpha(float delta)
        {
            var color = _spriteRenderer.color;
            _alpha = color.a;
            _alpha += delta;
            color = new Color(color.r, color.g, color.b, _alpha);
            _spriteRenderer.color = color;
            _timer = _timerAddAlpha;
            if (_alpha>=1)
            {
               _collider2D.enabled = true;
                _timer = _timerOfLive;
                _isAlpha = true;
                
            }
        }
        public void Dispose()
        { 
            Destroy(gameObject);
        }

       
        private void Update()
        {
            if (!_playerIsDeath)
            {
                _timer -= Time.deltaTime;
                if (_timer<=0 && _isAlpha == false)
                {
                    AddAlpha(_deltaAddAlpha);
                }
                if (_timer<=0 && _isAlpha) 
                {
                    RemoveAlpha(_deltaRemoveAlpha);
                }
            }
        }
    }
}