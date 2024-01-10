using UnityEngine;

namespace ArcadeSnake
{
    public class UiView : MonoBehaviour
    {
    
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Transform screenTransform;
        [SerializeField] private RectTransform canvasRectTransform;

        public RectTransform CanvasRectTransform => canvasRectTransform;
        public Vector3 Position
        {
            get => bodyTransform.position;
            set => bodyTransform.position = value;
        }    
        public Quaternion Rotation
        {
            get => bodyTransform.rotation;
            set => bodyTransform.rotation = value;
        }
        public Transform Screen => screenTransform;
    }
}