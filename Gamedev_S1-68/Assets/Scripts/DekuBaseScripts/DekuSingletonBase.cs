using UnityEngine;

/// <summary>
/// Clase base para crear Singletons de MonoBehaviour.
/// Heredá de esta clase con la clase que querés que sea singleton.
/// Ej: public class GameManager : DekuSingletonBase<GameManager> { }
/// </summary>
public class DekuSingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] La instancia '{typeof(T)}' ya fue destruida al cerrar la aplicación.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debug.LogError($"[Singleton] ¡Hay múltiples instancias de '{typeof(T)}' en la escena!");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject($"(singleton) {typeof(T)}");
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
