using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "SnakeConfig", menuName = "Config/SnakeConfig", order = 0)]
    public class SnakeConfig : ScriptableObject
    {
        public Vector3 StartPosition;
        public float Speed;
        //сколько нужно сьесть частей чтобы увеличить скорость змейки
        public int PartForUpSpeed;
        public float SpeedDelta;
        public int SnakeBodySize;
        public Direction Direction;
    }
}

    
