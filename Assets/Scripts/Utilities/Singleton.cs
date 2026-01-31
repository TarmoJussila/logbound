using UnityEngine;

namespace Logbound.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new();
        
        private const bool DestroyOnLoad = true;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = (T)FindObjectOfType(typeof(T));
                            if (_instance == null)
                            {
                                var singletonObj = new GameObject(typeof(T).Name);
                                _instance = singletonObj.AddComponent<T>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (!DestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}