using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "SnakePrtocolConfig", menuName = "Config/SnakePrtocolConfig", order = 0) ]
    public class SnakeProtocolConfig : ScriptableObject
    {
        public SceneObjectProtocol Protocol;
    }
}
