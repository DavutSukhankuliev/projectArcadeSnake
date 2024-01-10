using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ArcadeSnake
{
    public class FoodController
    {
        private readonly GridConfig _gridConfig;
        private readonly IInstantiator _instantiator;

        private readonly Food.Factory _foodFactory;
        private readonly FoodProtocolConfig _foodProtocolConfig;

        private Vector3 _foodGridPosition;
        
        private List<int> _controlRandom ;
        private List<int> _randomList;
        
        private List<Food> _foods;

        private Food[] _foodPrefabs;

        public FoodController(
            GridConfig gridConfig,
            IInstantiator instantiator
        )
        {
            _gridConfig = gridConfig;
            _instantiator = instantiator;

            _randomList= new List<int>(); 
            _controlRandom= new List<int>(); 
            
            _foodPrefabs =  Resources.LoadAll<Food>("FoodPrefabs");
            
            for (int j = _foodPrefabs.Length-1; j >=0; j--)
            {
                _randomList.Add((int)Math.Pow(_foodPrefabs.Length,j));
            }
           
            for (int i = 0; i < _foodPrefabs.Length - 1; i++)
            {
                for (int j = 0; j < _randomList[i]; j++)
                {
                    _controlRandom.Add(i);
                } 
            }

            _foods = new List<Food>();
        }

        public void SpawnFood(List<Vector3> posToSpawn, Vector3 snakePosition)
        {
            Food randPrefab = new Food();
            
            for (int i = (int)snakePosition.x - 2; i <= snakePosition.x + 2; i++)
            {
                for (int j = (int)snakePosition.y - 2; j <= snakePosition.y + 2; j++)
                {
                    posToSpawn.Add(new Vector3(i, j));
                }
            }   
            
            do {
                SpawnCoordinate();
            } while (posToSpawn.IndexOf(_foodGridPosition) != -1);

            var temp = Random.Range(0, _controlRandom.Count);
            
            if (temp >= 0 && temp < 64)
            {
                randPrefab = _foodPrefabs[0];
            } 
            if (temp >= 64 && temp < 80)
            {
                randPrefab = _foodPrefabs[1];
            }
            if (temp >= 80 && temp < 84)
            {
                randPrefab = _foodPrefabs[2];
            }
            if (temp == 84)
            {
                randPrefab = _foodPrefabs[3];
            }

            var foodPrefab = _instantiator.InstantiatePrefab(randPrefab);
            
            foodPrefab.transform.position = _foodGridPosition;
            
            _foods.Add(foodPrefab.GetComponent<Food>());
        }
        
        public void DestroyAllFoods()
        {
            if (_foods.Count>0)
            {
                foreach (var i in _foods)
                {   
                    i.Delete();
                }
                _foods.Clear();
            }
        }

        public void ClearFoodList()
        {
            _foods.Clear();
        }

        private void SpawnCoordinate()
        {
            _foodGridPosition.x = Random.Range(0, _gridConfig.Width);
            _foodGridPosition.y = Random.Range(0, _gridConfig.Height);
        }
        public List<Food> GetFoodList()
        {
            return _foods;
        }
    }
}
