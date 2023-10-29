using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Config/GameSettings", order = 0)]
    public class GameSettingsConfig : ScriptableObject
    {
        public bool IsAudioMute;
    }
}