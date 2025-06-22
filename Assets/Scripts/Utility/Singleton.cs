using UnityEngine;

namespace Prism.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new();
        private static bool _isShuttingDown;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown) return null;

                lock (_lock)
                {
                    if (_instance != null) return _instance;
                    
                    _instance = FindFirstObjectByType<T>();

                    if (_instance != null) return _instance;
                    
                    var singletonObject = new GameObject(typeof(T).Name);

                    _instance = singletonObject.AddComponent<T>();
                            
                    DontDestroyOnLoad(singletonObject);

                    return _instance;
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isShuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            _isShuttingDown = true;
        }
    }
}