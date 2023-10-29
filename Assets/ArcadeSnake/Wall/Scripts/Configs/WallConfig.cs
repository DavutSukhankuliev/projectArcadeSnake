using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "WallConfig", menuName = "Config/WallConfig", order = 0)]
    public class WallConfig : ScriptableObject
    {
        public Sprite Sprite;
        public float TimeAddAlpha;
        public float TimeOfLive;
        public float TimeRemoveAlpha;
        public float DeltaAddAlpha;
        public float DeltaRemoveAlpha;
        public float RespawnTime;
    }
}