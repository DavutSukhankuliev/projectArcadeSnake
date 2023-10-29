using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArcadeSnake
{
    public class WallController
    {
        private readonly GridConfig _gridConfig;
     
        private readonly Wall.Factory _wallFactory;
        private readonly WallProtocolConfig _wallProtocolConfig;
      
        private Vector3 _wallGridPosition;

        private List<int> _randomList;

        private List<Wall> _walls;

        public WallController(
              GridConfig gridConfig,
              Wall.Factory wallFactory,
              WallProtocolConfig wallProtocolConfig
            )
        {
            _gridConfig = gridConfig;
          
            _wallFactory = wallFactory;
            _wallProtocolConfig = wallProtocolConfig;
            
            _randomList = new List<int>();

            for (int i = 1; i <= 4; i++)
            {
                var a = (int) Math.Pow(2, i);
                switch (a) {
                    case 2:
                    {
                        for (var j = 12; j > a * 3; j--)
                        {
                            _randomList.Add(j);
                        }
                        break;
                    }
                    case 4: {
                        for (var j = 6; j > 0; j--)
                        {
                            _randomList.Add(j);
                        }
                        for (var j = 6; j > 0; j--)
                        {
                            _randomList.Add(j);
                        }
                        break;
                    }
                    case 8:
                    {
                        for (int j = 0; j < 48; j++)
                        {
                            _randomList.Add(0);
                        }
                        break;
                    }
                }
            }
            _walls = new List<Wall>();
        }

        #region SpawnWall

        public void SpawnWall(List<Vector3> posToSpawn, Vector3 snakePosition)
        {
            var verticalOrHorizontalWall = Random.Range(0,47);

            if (verticalOrHorizontalWall == 0 || verticalOrHorizontalWall < 30)
            {
                for (int i = (int)snakePosition.x - 2; i <= snakePosition.x + 2; i++)
                {
                    for (int j = (int)snakePosition.y - 2; j <= snakePosition.y + 2; j++)
                    {
                        posToSpawn.Add(new Vector3(i, j));
                    }

                }
            }
            else if (verticalOrHorizontalWall >= 30 && verticalOrHorizontalWall <42)
            {
                for (int i = (int)snakePosition.x - 3; i <= snakePosition.x + 3; i++)
                {
                    for (int j = (int)snakePosition.y - 3; j <= snakePosition.y + 3; j++)
                    {
                        posToSpawn.Add(new Vector3(i, j));
                    }

                }
            }
            else if (verticalOrHorizontalWall >= 42 && verticalOrHorizontalWall < 48)
            {
                for (int i = (int)snakePosition.x - 3; i <= snakePosition.x + 3; i++)
                {
                    for (int j = (int)snakePosition.y - 3; j <= snakePosition.y + 3; j++)
                    {
                        posToSpawn.Add(new Vector3(i, j));
                    }
                }
            }
            
            if (verticalOrHorizontalWall >=0 && verticalOrHorizontalWall<30)
            {
                do {
                    SpawnCoordinate(0,0);
                } while (posToSpawn.IndexOf(_wallGridPosition) != -1);
            }
            
            else if (verticalOrHorizontalWall>=30 && verticalOrHorizontalWall<42)
            {
                do {
                    SpawnCoordinate(1,1);
                } while (posToSpawn.IndexOf(_wallGridPosition) != -1);
            }
            else if (verticalOrHorizontalWall>=42 && verticalOrHorizontalWall<48)
            {
                do {
                    SpawnCoordinate(2,2);
                } while (posToSpawn.IndexOf(_wallGridPosition) != -1);
            }
            
            if (verticalOrHorizontalWall >= 0 || verticalOrHorizontalWall < 30)
            {
                SpawnWall_1Elements();
            } 
            if (verticalOrHorizontalWall == 30 || verticalOrHorizontalWall == 36)
            {
                SpawnWallVertical_3Elements();
            }
            if (verticalOrHorizontalWall == 31 || verticalOrHorizontalWall == 37)
            {
                SpawnWallHorizontal_3Elements();
            }
            if (verticalOrHorizontalWall == 32 || verticalOrHorizontalWall == 38)
            {
                SpawnType_1_Wall_3Elements();
            }
            if (verticalOrHorizontalWall == 33 || verticalOrHorizontalWall == 39)
            {
                SpawnType_2_Wall_3Elements();
            }
            if (verticalOrHorizontalWall == 34|| verticalOrHorizontalWall == 40)
            {
                SpawnType_3_Wall_3Elements();
            }
            if (verticalOrHorizontalWall == 35 || verticalOrHorizontalWall == 41)
            {
                SpawnType_4_Wall_3Elements();
            } 
            if ( verticalOrHorizontalWall == 42 )
            {
                SpawnWallVertical_5Elements();
            }
            if (verticalOrHorizontalWall == 43)
            {
                SpawnWallHorizontal_5Elements();
            }
            if (verticalOrHorizontalWall==43)
            {
                SpawnType_4_Wall_5Elements();
            }
            if (verticalOrHorizontalWall==44)
            {
                SpawnType_1_Wall_5Elements();
            } 
            if (verticalOrHorizontalWall==45)
            {
                SpawnType_2_Wall_5Elements();
            } 
            if (verticalOrHorizontalWall==46)
            {
                SpawnType_3_Wall_5Elements();
            }
        }
        private void SpawnWall_1Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
        } 
        private void SpawnWallVertical_3Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(0, +1));
            SpawnWallExemplar(new Vector3(0, -1));
        } 
        private void SpawnWallVertical_5Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(0, +1));
            SpawnWallExemplar(new Vector3(0, -1));
            SpawnWallExemplar(new Vector3(0, +2));
            SpawnWallExemplar(new Vector3(0, -2));
        }
        private void SpawnWallHorizontal_3Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(+1, 0));
            SpawnWallExemplar(new Vector3(-1, 0));
        } 
        private void SpawnWallHorizontal_5Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(+1, 0));
            SpawnWallExemplar(new Vector3(-1, 0));
            SpawnWallExemplar(new Vector3(+2, 0));
            SpawnWallExemplar(new Vector3(-2, 0));
        }
        private void SpawnType_1_Wall_3Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(+1, 0));
            SpawnWallExemplar(new Vector3(0, +1));
        }
        private void SpawnType_1_Wall_5Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(+1, 0));
            SpawnWallExemplar(new Vector3(0, +1));
            SpawnWallExemplar(new Vector3(+2, 0));
            SpawnWallExemplar(new Vector3(0, +2));
        }
        private void SpawnType_2_Wall_3Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(-1, 0));
            SpawnWallExemplar(new Vector3(0, +1));
        }
        private void SpawnType_2_Wall_5Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(-1, 0));
            SpawnWallExemplar(new Vector3(0, +1)); 
            SpawnWallExemplar(new Vector3(-2, 0));
            SpawnWallExemplar(new Vector3(0, +2));
        }
        private void SpawnType_3_Wall_3Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(-1, 0));
            SpawnWallExemplar(new Vector3(0, -1));
        } 
        private void SpawnType_3_Wall_5Elements()
        {
            SpawnWallExemplar( new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(-1, 0));
            SpawnWallExemplar(new Vector3(0, -1));
            SpawnWallExemplar(new Vector3(-2, 0));
            SpawnWallExemplar(new Vector3(0, -2));
        }
        private void SpawnType_4_Wall_3Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(0, 1));
            SpawnWallExemplar(new Vector3(-1, 0));
        }
        private void SpawnType_4_Wall_5Elements()
        {
            SpawnWallExemplar(new Vector3(0, 0));
            SpawnWallExemplar(new Vector3(0, 1));
            SpawnWallExemplar(new Vector3(-1, 0));
            SpawnWallExemplar(new Vector3(0, 2));
            SpawnWallExemplar(new Vector3(-2, 0));
        }

        #endregion

        private void SpawnWallExemplar(Vector3 vector)
        {
            var wall = _wallFactory.Create(_wallProtocolConfig.Protocol);
            wall.transform.position = _wallGridPosition + vector;
            _walls.Add(wall);
        }

        public void DestroyAllWalls()
        {
            if (_walls.Count>0)
            {
                foreach (var i in _walls)
                {
                    i.Dispose();
                }
            
                _walls.Clear();
            }
        }
        
        public void ClearWallList()
        {
            _walls.Clear();
        }

        private void SpawnCoordinate(int deltaX, int deltaY)
        {
            _wallGridPosition.x = Random.Range(deltaX, _gridConfig.Width - deltaX);
            _wallGridPosition.y = Random.Range(deltaY, _gridConfig.Height - deltaY);
        }

        public List<Wall> GetWallList()
        {
            return _walls;
        }
    }
}