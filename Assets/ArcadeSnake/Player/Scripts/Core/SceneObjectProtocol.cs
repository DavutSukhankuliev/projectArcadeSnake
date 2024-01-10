using System;
using UnityEngine;

namespace ArcadeSnake
{
    [Serializable]
    public struct SceneObjectProtocol
    {
        public Vector3 Position;

        public SceneObjectProtocol(Vector3 position)
        {
            Position = position;
        }
    }
}
