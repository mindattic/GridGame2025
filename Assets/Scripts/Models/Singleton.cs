using UnityEngine;

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
                return null; //Do not return an instance when quitting
            }

            lock (objLock)
            {
                if (_instance != null)
                    return _instance;

                //Use FindObjectsByType instead of FindObjectsOfType
                var instances = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
                var count = instances.Length;

                if (count > 0)
                {
                    if (count == 1)
                        return _instance = instances[0];

                    Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                    for (var i = 1; i < instances.Length; i++)
                        Destroy(instances[i]);
                    return _instance = instances[0];
                }

                Debug.Log($"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
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
