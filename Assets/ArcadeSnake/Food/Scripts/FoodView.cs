using System;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class FoodView : MonoBehaviour, IDisposable
    {
        [HideInInspector] public int PointForEating;
        [HideInInspector] public int WillAddParts;
        [HideInInspector] public float BoostSpeed;
        [HideInInspector] public float TimeBoostingSpeed;
        
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _collider2D;
        [SerializeField] private FoodConfig _foodConfig;
        [SerializeField] private GlobalFoodSettings _globalFoodSettings;
        
        private SnakeMovement _snakeMovement;
        private FoodController _foodController;

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
            FoodController foodController)
        {
            _snakeMovement = snakeMovement;
            _foodController = foodController;
            
            _snakeMovement.OnDeathEvent += PlayerDeath;

            _playerIsDeath = false;
        }

        private void PlayerDeath(object sender, EventArgs e)
        {
            _playerIsDeath = true;
        }

        private void Start()
        {
            PointForEating = _foodConfig.PointForEating;
            WillAddParts = _foodConfig.WillAddParts;
            BoostSpeed = _foodConfig.BoostSpeed;
            TimeBoostingSpeed = _foodConfig.TimeBoostingSpeed;
            
            _spriteRenderer.sprite = _foodConfig.Sprite;
            
            _timerAddAlpha = _globalFoodSettings.TimeAddAlpha;
              
            _timerOfLive = _globalFoodSettings.TimeOfLive;
           
            _timerRemoveAlpha = _globalFoodSettings.TimeRemoveAlpha;
            
            _deltaAddAlpha = _globalFoodSettings.DeltaAddAlpha;
            _deltaRemoveAlpha = _globalFoodSettings.DeltaRemoveAlpha;

            _timer = _timerAddAlpha;
        }


        public void RemoveAlpha(float delta)
        {
            _alpha = _spriteRenderer.color.a;
            _alpha -= delta;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _alpha);
            _timer = _timerRemoveAlpha;
            if (_alpha <= 0)
            {
                //_foodController.DestroyAllFoods();
                Dispose();
            }
        }
        public void AddAlpha(float delta)
        {
            _alpha = _spriteRenderer.color.a;
            _alpha += delta;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _alpha);
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
                if (_timer<=0 && _isAlpha )
                {
                    RemoveAlpha(_deltaRemoveAlpha);
                }
            }
        }
    }
}