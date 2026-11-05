using UnityEngine;

public class PersistentRoot : MonoBehaviour
{
    static PersistentRoot instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            // Deactivating first stops the managers inside this copy from running Awake
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
