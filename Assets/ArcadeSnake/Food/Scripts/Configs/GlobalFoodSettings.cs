using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "GlobalFoodSettings", menuName = "Config/GlobalFoodSettings", order = 0)]
    public class GlobalFoodSettings: ScriptableObject
    {
        public float TimeAddAlpha;
        public float TimeOfLive;
        public float TimeRemoveAlpha;
        public float DeltaAddAlpha;
        public float DeltaRemoveAlpha;
        public float RespawnTime;
    }
}