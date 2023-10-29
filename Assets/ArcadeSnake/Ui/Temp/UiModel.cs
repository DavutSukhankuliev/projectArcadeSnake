using System;
using UnityEngine;

namespace ArcadeSnake
{
    [Serializable]
    public class UiModel
    {
        [SerializeField] private int id;
        [SerializeField] private Vector3 position;
        [SerializeField] private Quaternion rotation;
        [SerializeField] private UiView viewPrefab;
        [SerializeField] private bool _followCamera;

        public int ID => id;
        public Vector3 Position => position;
        public Quaternion Rotation => rotation;
        public UiView ViewPrefab => viewPrefab;
        public bool FollowCamera => _followCamera;
    }
}