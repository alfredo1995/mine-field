using UnityEngine;

namespace Pattern
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object _lock = new();
        private static T _instance;

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    Initialize();
                    return _instance;
                }
            }
        }

        private void Awake()
        {
            if (_instance != null)
                DontDestroyOnLoad(this);
        }

#if !UNITY_WEBGL
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Initialize()
        {
            if (_instance == null || _instance.Equals(null))
            {
                _instance = FindObjectOfType<T>();

                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                                   " - there should never be more than 1 singleton!" +
                                   " Reopening the scene might fix it.");
                }

                if (_instance == null)
                {
                    var singletonObject = new GameObject(typeof(T) + "(Singleton)")
                    {
                        hideFlags = HideFlags.NotEditable
                    };

                    _instance = singletonObject.AddComponent<T>();

                    DontDestroyOnLoad(singletonObject);
                }
            }
        }
    }

    public abstract class SingletonTemporary<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = FindObjectOfType<T>();
        }

        protected void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
}