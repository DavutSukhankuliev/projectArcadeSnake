using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "MiographSettings", menuName = "Miograph/Settings")]
    public class MiographSettings : ScriptableObject
    {
        public string IP;
        public int port;
    }
}