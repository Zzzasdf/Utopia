using UnityEngine;

public class ThreadSafeMonoSingleton<T> : MonoBehaviour
    where T : ThreadSafeMonoSingleton<T>
{
    private static T _instance;
    private static readonly object _lock = new object();
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }
    }
    public bool Survival() => _instance != null;
}