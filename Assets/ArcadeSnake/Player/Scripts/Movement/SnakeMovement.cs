using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class SnakeMovement : ITickable
    {
        public event EventHandler OnDeathEvent;
        
        public bool GameIsStart;

        public EventHandler<int> OnUpdateScoreEvent
        {
            get;
            set;
        }

        private readonly Snake.Factory _snakeFactory;
        private readonly SnakeProtocolConfig _snakeProtocolConfig;
        private readonly SnakeConfig _snakeConfig;
        private readonly WallConfig _wallConfig;
        private readonly GlobalFoodSettings _globalFoodSettings;

        private readonly FoodController _foodController;
        private readonly IInstantiator _instantiator;
        private readonly GridConfig _gridConfig;
        private readonly WallController _wallController;
        private readonly InputState _inputState;
        private readonly AudioPlayerController _audioPlayerController;

        private SnakeMovePosition _previousSnakeMovePosition;
        private Snake _player;

        private List<SnakeMovePosition> _snakeMovePositionList;
        private List<SnakeBodyPart> _snakeBodyPartList;

        private Direction _cashedDirection;
        private Direction _moveDirection;

        private Vector3 _moveDirectionVector;
        private Vector3 _desirePosition;

        private float _snakeSpeed;
        private float _oldSpeed;
        private bool _boostingSpeed;
        private float _speedDelta;

        private float _timerOfboosting;
        
        private int _snakeBodySize;
        private int _partForSpeed;

        private float _wallRespawnTimeMax;
        private float _wallRespawnTime;
        
        private float _foodRespawnTimeMax;
        private float _foodRespawnTime;
        
        private bool _isMoving;


        public SnakeMovement(
            IInstantiator instantiator,
            
            Snake.Factory snakeFactory,
            SnakeProtocolConfig snakeProtocolConfig,
            SnakeConfig snakeConfig,
            
            GlobalFoodSettings globalFoodSettings,
            WallConfig wallConfig,

            FoodController foodController,
            WallController wallController,

            GridConfig gridConfig,
           
            InputState inputState,
            AudioPlayerController audioPlayerController
        )
        {
            _audioPlayerController = audioPlayerController;
            
            _snakeFactory = snakeFactory;
            _snakeProtocolConfig = snakeProtocolConfig;
            _snakeConfig = snakeConfig;
            
            _wallConfig = wallConfig;
            _globalFoodSettings = globalFoodSettings;

            _foodController = foodController;
            _instantiator = instantiator;
            _gridConfig = gridConfig;
            _wallController = wallController;
            _inputState = inputState;

            _snakeMovePositionList = new List<SnakeMovePosition>();
            _snakeBodyPartList = new List<SnakeBodyPart>();

            _moveDirection = _snakeConfig.Direction;
            _cashedDirection = _moveDirection;

            _snakeBodySize = 1;
            _partForSpeed = _snakeConfig.PartForUpSpeed;
            _speedDelta = _snakeConfig.SpeedDelta;
        }

        public void RetryMoving()
        {
            HandleMovement();
        }

        public void StartGame()
        {
            _wallRespawnTimeMax = _wallConfig.RespawnTime;
            _wallRespawnTime = 0;
            
            _foodRespawnTimeMax = _globalFoodSettings.RespawnTime;
            _foodRespawnTime = 0;
            
            OnUpdateScoreEvent?.Invoke(this, 0);

            _player = _snakeFactory.Create(_snakeProtocolConfig.Protocol);
            _player.StopHeadache();
            _player.transform.position = _snakeConfig.StartPosition;
            
            _snakeSpeed = _snakeConfig.Speed;
            
            _moveDirection = _snakeConfig.Direction;
            _cashedDirection = _moveDirection;

            _desirePosition = _player.transform.position;
            _player.EatingColliderNotify += TryEatingSmthColliderNotify;
            GameIsStart = true;
            HandleMovement();
        }

        public void EndGame()
        {
            _player.Dispose();
            
            RemovePartForRestart();
            
            _wallController.DestroyAllWalls();
            _foodController.DestroyAllFoods();
        }

        public void HandleMovement()
        {
            if (GameIsStart)
            {
                _isMoving = true;
            
                if (_snakeMovePositionList.Count > 0)
                {
                    _previousSnakeMovePosition = _snakeMovePositionList[0];
                }
            
                var snakeMovePosition = new SnakeMovePosition(_previousSnakeMovePosition, _desirePosition, _cashedDirection);

                _snakeMovePositionList.Insert(0, snakeMovePosition);
            
                switch (_cashedDirection)
                {
                    case Direction.Left:
                        _moveDirectionVector = Vector3.left;
                        break;
                    case Direction.Right:
                        _moveDirectionVector = Vector3.right;
                        break;
                    case Direction.Up:
                        _moveDirectionVector = Vector3.up;
                        break;
                    case Direction.Down:
                        _moveDirectionVector = Vector3.down;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
           
                _desirePosition += _moveDirectionVector;

                CheckBorder();

                _player.transform.DOMove(_desirePosition, _snakeSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _isMoving = false;
                    HandleMovement();
                });
            
                _player.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(_moveDirectionVector) - 90);
            
                UpdateSnakeBodyParts();

                if (_snakeMovePositionList.Count >= _snakeBodySize + 1)
                {
                    _snakeMovePositionList.RemoveAt(_snakeMovePositionList.Count - 1);
                }
                _isMoving = false;
            }
        }

        public void Tick()
        {
            HandleInput();
            
            if (_boostingSpeed)
            {
                AbilityBosstingTimer();
            }

            if (GameIsStart)
            {
                SpawnWall();
                SpawnFood();
            }
        }

        private void AbilityBosstingTimer()
        {
            _timerOfboosting -= Time.deltaTime;
            if (_timerOfboosting <= 0 )
            {
                _snakeSpeed = _oldSpeed;
                _boostingSpeed = false;
            }
        }

        private void HandleInput()
        {
            if ((_inputState.IsLeftPressed || OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) || Input.GetKeyDown(KeyCode.LeftArrow)) && _isMoving == false)
            {
                _isMoving = true;
                
                _cashedDirection = _cashedDirection switch {
                    Direction.Left => Direction.Down,
                    Direction.Up => Direction.Left,
                    Direction.Right => Direction.Up,
                    Direction.Down => Direction.Right,
                    _ => _cashedDirection
                };
                
                _inputState.IsLeftPressed = false;
            }

            if ((_inputState.IsRightPressed || OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) || Input.GetKeyDown(KeyCode.RightArrow)) && _isMoving == false)
            {
                _isMoving = true;
                
                _cashedDirection = _cashedDirection switch
                {
                    Direction.Left => Direction.Up,
                    Direction.Up => Direction.Right,
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    _ => _cashedDirection
                };
                
                _inputState.IsRightPressed = false;
            }
        }
       

        private void SpawnWall()
        {
            _wallRespawnTime -= Time.deltaTime;
            if (_wallRespawnTime <= 0)
            {
                _wallController.ClearWallList();
                _wallController.SpawnWall(GetFullSnakeGridPositionList(), _desirePosition);
                _wallRespawnTime = _wallRespawnTimeMax;
            }
        }

        private void SpawnFood()
        {
            _foodRespawnTime -= Time.deltaTime;
            if (_foodRespawnTime <= 0)
            {
                _foodController.ClearFoodList();
                _foodController.SpawnFood(GetFullSnakeGridPositionList(), _desirePosition);
                _foodRespawnTime = _foodRespawnTimeMax;
            }
        }
        
        private void CreateSnakeBodyPart()
        {
            _snakeBodyPartList.Add(new SnakeBodyPart(
                    _snakeMovePositionList[_snakeMovePositionList.Count - 1].GetGridPosition(), _instantiator, _snakeBodySize, _gridConfig));
            _snakeBodyPartList[_snakeBodyPartList.Count - 1]
                    .SetSnakeMovePosition(_snakeMovePositionList[_snakeMovePositionList.Count - 1], _snakeSpeed);
            _snakeBodySize++;
        }

        private void UpdateSnakeBodyParts()
        {
            for (int i = 0; i < _snakeBodyPartList.Count; i++)
            {
                _snakeBodyPartList[i].SetSnakeMovePosition(_snakeMovePositionList[i], _snakeSpeed);
            }
        }

        private float GetAngleFromVector(Vector3 dir)
        {
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0)
                n += 360;
            return n;
        }

        private void CheckBorder()
        {
            if (_desirePosition.x > _gridConfig.Width - 1)
            {
                _player.transform.DOKill();
                var pos = _player.transform.position;
                pos.x = 0;
                _player.transform.position = pos;
                
                _desirePosition.x = 1;
            }
            if (_desirePosition.y > _gridConfig.Height - 1)
            {
                _player.transform.DOKill();

                var pos = _player.transform.position;
                pos.y = 0;
                _player.transform.position = pos;
                
                _desirePosition.y = 1;
            }
            if (_desirePosition.x < 0)
            {
                _player.transform.DOKill();

                var pos = _player.transform.position;
                pos.x = _gridConfig.Width - 1;
                _player.transform.position = pos;

                _desirePosition.x = _gridConfig.Width - 2;
            }
            if (_desirePosition.y < 0)
            {
               _player.transform.DOKill();

                var pos = _player.transform.position;
                pos.y = _gridConfig.Height - 1;
                _player.transform.position = pos;
                
                _desirePosition.y = _gridConfig.Height - 2;
            }
        }

        private void TryEatingSmthColliderNotify(object sender, Collider2D collider)
        {
            TryEatingHimself(collider);

            TryEatingWall(collider);
            
            TryEatingFood(collider);
        }

        private void TryEatingHimself(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                _player.StartHeadache();
                _audioPlayerController.PlaySFX("Death");
                _player.transform.DOKill();
                OnDeathEvent?.Invoke(this, EventArgs.Empty);
                GameIsStart = false;
            }
        }
        
        private void TryEatingWall(Collider2D collider)
        {
            if (collider.CompareTag("Wall"))
            {
                _player.StartHeadache();
                _audioPlayerController.PlaySFX("Death");
                _player.transform.DOKill();
                OnDeathEvent?.Invoke(this, EventArgs.Empty);
                GameIsStart = false;
            }
        }

        private void TryEatingFood(Collider2D collider)
        {
            if (collider.CompareTag("Food"))
            {
                _foodController.ClearFoodList();
               
                var food = collider.gameObject.GetComponent<FoodView>();

                OnUpdateScoreEvent?.Invoke(this, food.PointForEating);
                
                BoostSpeedEveryEatingFood();

                ChangeCountBodyParts(food);
                
                if (food.BoostSpeed > 0)
                {
                    _oldSpeed = _snakeSpeed;
                    _snakeSpeed -= food.BoostSpeed;
                    _timerOfboosting = food.TimeBoostingSpeed;
                    _boostingSpeed = true;
                    _audioPlayerController.PlaySFX("SpeedUp");
                }
                else
                {
                    _audioPlayerController.PlaySFX("EatSound");
                }
            }
        }

        private void BoostSpeedEveryEatingFood() 
        {
            if (_partForSpeed <= 0) 
            {
                _snakeSpeed -= _speedDelta;
                _partForSpeed = _snakeConfig.PartForUpSpeed;
            }
            else 
            {
                _partForSpeed--;
            }
        }

        private void ChangeCountBodyParts(FoodView view)
        {
            if (view.WillAddParts < 0)
            {
                if (_snakeBodySize > view.WillAddParts*(-1))
                {
                    for (int i = 0; i > view.WillAddParts; i--)
                    {
                        RemoveParts();
                    }
                }
                else
                {
                    for (int i = 0; i < _snakeBodySize-1; i++)
                    {
                        RemoveParts();
                    }
                }

                view.Dispose();
                _foodController.SpawnFood(GetFullSnakeGridPositionList(),_desirePosition);
                _foodRespawnTime = _foodRespawnTimeMax;
            }
            else
            {
                view.Dispose();
                _foodController.SpawnFood(GetFullSnakeGridPositionList(),_desirePosition);
                _foodRespawnTime = _foodRespawnTimeMax;
                CreateSnakeBodyPart();
            }
        }
        

        private void RemoveParts() 
        {
            _audioPlayerController.PlaySFX("DeletePartOfBody");
            var temp = _snakeBodyPartList[_snakeBodyPartList.Count - 1];
            _snakeBodyPartList.RemoveAt(_snakeBodyPartList.Count - 1);
            _snakeMovePositionList.RemoveAt(_snakeMovePositionList.Count - 1);
            temp.Death();
            _snakeBodySize--;
        }

        private void RemovePartForRestart()
        {
            while (_snakeBodySize!=1)
            {
                var temp = _snakeBodyPartList[_snakeBodyPartList.Count - 1];
                _snakeBodyPartList.RemoveAt(_snakeBodyPartList.Count - 1);
                _snakeMovePositionList.RemoveAt(_snakeMovePositionList.Count - 1);
                temp.Death();
                _snakeBodySize--;
            }
        }

        private List<Vector3> GetFullSnakeGridPositionList()
        {
            var gridPositionList = new List<Vector3>() {_desirePosition};
            foreach (SnakeMovePosition snakeMovePosition in _snakeMovePositionList)
            {
                gridPositionList.Add(snakeMovePosition.GetGridPosition());
            }
            return gridPositionList;
        }
    }
}