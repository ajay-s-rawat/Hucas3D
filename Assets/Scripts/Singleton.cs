using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            // Lock for thread safety
            lock (lockObject)
            {
                if (instance == null)
                {
                    // Try to find an existing instance in the scene
                    instance = FindObjectOfType<T>();

                    // If no instance found, create a new GameObject
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject); // Persist across scenes
                    }
                }
                return instance;
            }
        }
    }

    // Optionally, This method can be used to method to reset the instance, useful for testing.
    public static void ResetInstance()
    {
        lock (lockObject)
        {
            instance = null;
        }
    }

    protected virtual void Awake()
    {
        // Ensure that the singleton instance is assigned and prevent duplicates
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }
}
