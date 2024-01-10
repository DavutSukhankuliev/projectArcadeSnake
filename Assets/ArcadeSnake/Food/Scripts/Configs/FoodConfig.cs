using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "FoodConfig", menuName = "Config/FoodConfig", order = 0)]
    public class FoodConfig : ScriptableObject
    {
        public Sprite Sprite;
        
        public int PointForEating;
        
        public int WillAddParts;
        
        public float BoostSpeed;
        public float TimeBoostingSpeed;

    }
}