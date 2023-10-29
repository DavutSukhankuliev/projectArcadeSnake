using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ArcadeSnake 
{
    public class UIService : IUIService
    {
        private readonly IUIRoot _uIRoot;
        private readonly IInstantiator _instantiator;
        private Transform _deactivatedContainer;
        private readonly Dictionary<Type,UICanvasWindow> _viewStorage = new Dictionary<Type,UICanvasWindow>();
        private readonly Dictionary<Type, GameObject> _initWindows= new Dictionary<Type, GameObject>();

        public UIService(
            IInstantiator instantiator,
            IUIRoot uIRoot)
        {

            _instantiator = instantiator;
            _uIRoot = uIRoot;
        }

        public T Show<T>() where T : UICanvasWindow
        {
            var window = Get<T>();
            if(window!=null)
            {
                window.transform.SetParent(_uIRoot.Container);
                window.Show();
                return window;
            }
            return null;
        }

        public T Get<T>() where T : UICanvasWindow
        {
            var type = typeof(T);
            if (_initWindows.ContainsKey(type))
            {
                var view = _initWindows[type];            
                return view.GetComponent<T>();
            }
            return null;
        }

        public void Hide<T>() where T : UICanvasWindow
        {
            var window = Get<T>();
            if(window!=null)
            {
                window.Hide();
            }
        }

        public void InitWindows(Transform poolDeactiveContiner = null)
        {
            _deactivatedContainer = poolDeactiveContiner == null ? _uIRoot.PoolContainer : poolDeactiveContiner;
            foreach (var windowKVP in _viewStorage)
            {
                Init(windowKVP.Key, _deactivatedContainer);
            }
        }

        public void LoadWindows()
        {
            var windows = Resources.LoadAll("", typeof(UICanvasWindow));

            foreach (var window in windows)
            {
                var windowType = window.GetType();
                _viewStorage.Add(windowType, (UICanvasWindow) window);
            }

            SetCameraForCanvas(GameObject.Find("CenterEyeAnchor").GetComponent<Camera>());
            SetCanvasPos(GameObject.Find("Back(Clone)").GetComponent<Transform>());
        }    
    
        private void Init(Type t, Transform parent = null)
        {
            if(_viewStorage.ContainsKey(t))
            {
                GameObject view = null;
                if(parent!=null)
                {
                    view = _instantiator.InstantiatePrefab(_viewStorage[t], parent);
                }
                else
                {
                    view = _instantiator.InstantiatePrefab(_viewStorage[t]);
                }
                _initWindows.Add(t, view);
            }
        }

        private void SetCameraForCanvas(Camera camera)
        {
            _uIRoot.Camera = camera;
            _uIRoot.Canvas.worldCamera = _uIRoot.Camera;
        }

        private void SetCanvasPos(Transform transform)
        {
            var rectTransform = _uIRoot.Canvas.GetComponent<RectTransform>();
            var width = transform.localScale.x*53.56f;
            var height = transform.localScale.y*54.2f;
            rectTransform.position = transform.position;
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = new Vector3(1/(width/transform.localScale.x), 1/(height/transform.localScale.y));
        }
    }
}
