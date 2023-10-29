using UnityEngine;
using System;
using Newtonsoft.Json;

namespace ArcadeSnake
{
    [Serializable]
    public class FoodModel
    {
        [SerializeField] private int _id;
        [SerializeField] private Sprite _sprite;
        
        [SerializeField] private int _pointForEating;
        
        [SerializeField] private int _willAddParts;
        
        [SerializeField] private float _boostSpeed;
        [SerializeField] private float _timeBoostingSpeed;
        
        [SerializeField] private float _timeAddAlpha;
        [SerializeField] private float _timeOfLive;
        [SerializeField] private float _timeRemoveAlpha;
        [SerializeField] private float _deltaAddAlpha;
        [SerializeField] private float _deltaRemoveAlpha;
        [SerializeField] private float _respawnTime;
        
        public int ID => _id;
        public Sprite Sprite => _sprite;
        public int PointForEating => _pointForEating;
        public int WillAddParts => _willAddParts;
        public float BoostSpeed => _boostSpeed;
        public float TimeBoostingSpeed => _timeBoostingSpeed;
        public float TimeAddAlpha => _timeAddAlpha;
        public float TimeOfLive => _timeOfLive;
        
        public float TimeRemoveAlpha => _timeRemoveAlpha;

        public float DeltaAddAlpha => _deltaAddAlpha;
        public float DeltaRemoveAlpha => _deltaRemoveAlpha;

        public float RespawnTime => _respawnTime;
        
        
    }
}