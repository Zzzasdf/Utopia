using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour
    where T: MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }
    public bool Survival() => _instance != null;
}
