using UnityEngine;

namespace ArcadeSnake
{
    [CreateAssetMenu(fileName = "FoodProtocolConfig", menuName = "Config/FoodProtocolConfig", order = 0) ]
   
    public class FoodProtocolConfig : ScriptableObject
    {
        public SceneObjectProtocol Protocol;
    }
}