﻿using UnityEngine;

namespace EuNet.Unity
{
    /// <summary>
    /// 모든 씬에 전역적으로 사용될 싱글톤. 씬이 바뀌어도 제거되지 않음. 없다면 자동생성함.
    /// </summary>
    /// <typeparam name="T">싱글턴이 될 컴포넌트</typeparam>
    public class SingletonAutoCreate<T> : MonoBehaviour where T : Component
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_inited == false)
                {
                    s_inited = true;
                    s_instance = FindObjectOfType<T>();

                    if (s_instance == null &&
                        s_quitted == false)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        s_instance = obj.AddComponent<T>();
                    }
                }

                return s_instance;
            }
        }

        private static bool s_inited = false;
        private static bool s_quitted = false;

        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
                DontDestroyOnLoad(this.gameObject);
                s_inited = true;
            }
            else
            {
                Debug.LogErrorFormat(gameObject, "Already created singleton object {0}", typeof(T));
            }
        }

        protected virtual void OnApplicationQuit()
        {
            s_quitted = true;
        }
    }
}