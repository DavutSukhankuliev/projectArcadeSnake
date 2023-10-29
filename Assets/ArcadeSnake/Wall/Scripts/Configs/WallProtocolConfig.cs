using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "WallProtocolConfig", menuName = "Config/WallProtocolConfig", order = 0) ]
    public class WallProtocolConfig : ScriptableObject
    {
        public SceneObjectProtocol Protocol;
    }
}