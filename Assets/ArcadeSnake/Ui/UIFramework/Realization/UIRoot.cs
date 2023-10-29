using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class UIRoot : MonoBehaviour, IUIRoot
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform container;
        [SerializeField] private Transform poolContainer;

        public Canvas Canvas
        {
            get => canvas;
            set => canvas = value;
        }
        
        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        public Transform Container => container;

        public Transform PoolContainer => poolContainer;
    }
}
