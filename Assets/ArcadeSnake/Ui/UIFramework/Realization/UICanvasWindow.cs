using System;
using UnityEngine;

namespace ArcadeSnake
{
    public abstract class UICanvasWindow : MonoBehaviour, IUICanvasWindow
    {
        [SerializeField] private RectTransform rectTransform;

        public EventHandler ShowEvent { get; set; }
        public EventHandler HideEvent { get; set; }
        public abstract void Show();
        public abstract void Hide();

        public void SetNewParent(RectTransform parent)
        {
            rectTransform.parent = parent;
            rectTransform.position = parent.position;
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);
            rectTransform.rotation = new Quaternion(0, 0, 0, 0);
        }

        protected virtual void OnShowEnd() { }
        protected virtual void OnHideEnd() { }

    }
}
