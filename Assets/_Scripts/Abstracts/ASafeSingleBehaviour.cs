using UnityEngine;

public abstract class ASafeSingleBehaviour<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T GetInstance
    {
        get
        {
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = gameObject.GetComponent<T>();

            CheckAndDestroyOtherInstances();
        }
    }

    protected static void CheckAndDestroyOtherInstances()
    {
        T[] otherInstances = FindObjectsOfType<T>();

        foreach (var OtherInstance in otherInstances)
        {
            if (OtherInstance != _instance)
            {
                Destroy(OtherInstance);
            }
        }
    }
}
