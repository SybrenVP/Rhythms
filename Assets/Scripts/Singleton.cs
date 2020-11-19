using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool IsApplicationClosing = false;

    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null && !IsApplicationClosing)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }

            return _instance;
        }
    }

    private void Awake()
    {
        IsApplicationClosing = false;
    }

    public virtual void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this.GetComponent<T>();
            DontDestroyOnLoad(_instance);
        }
    }

    private void OnApplicationQuit()
    {
        IsApplicationClosing = true;
    }

    private void OnDestroy()
    {
        IsApplicationClosing = true;
    }
}