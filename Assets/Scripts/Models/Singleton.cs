using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : Singleton where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object objLock = new object();

    [SerializeField]
    private bool _persistent = true;

    public static T instance
    {
        get
        {
            if (isQuitting)
            {
                Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                return null;
            }

            lock (objLock)
            {
                if (_instance != null)
                    return _instance;

                var instances = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
                if (instances.Length > 0)
                {
                    if (instances.Length > 1)
                    {
                        Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] More than one trailInstance found. Destroying extras.");
                        for (int i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                    }
                    return _instance = instances[0];
                }

                // Allow null if we are not in the Game scene
                if (SceneManager.GetActiveScene().name != "Game")
                {
                    return null; // Do not create a new trailInstance
                }

                Debug.Log($"[{nameof(Singleton)}<{typeof(T)}>] Creating new trailInstance in Game scene.");
                return _instance = new GameObject($"({nameof(Singleton)}){typeof(T)}")
                            .AddComponent<T>();
            }
        }
    }


    private void Awake()
    {
        if (_persistent)
            DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake() { }
}

public abstract class Singleton : MonoBehaviour
{
    public static bool isQuitting { get; private set; }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}
