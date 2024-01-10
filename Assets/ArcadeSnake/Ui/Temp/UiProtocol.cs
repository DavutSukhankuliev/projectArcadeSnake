using UnityEngine;

namespace ArcadeSnake
{
    public class UiProtocol
    {
        public int ID;
        public Vector3 Position;
        public Quaternion Rotation;
        public UiView ViewPrefab;
        public bool FollowCamera;

        public UiProtocol(int id, Vector3 position, Quaternion rotation, UiView viewPrefab, bool folowCamera)
        {
            ID = id;
            Position = position;
            Rotation = rotation;
            ViewPrefab = viewPrefab;
            FollowCamera = folowCamera;
        }
    }
}