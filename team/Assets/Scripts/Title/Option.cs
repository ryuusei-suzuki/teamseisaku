using UnityEngine;

public class Option : MonoBehaviour
{
    public static Option Instance;

    private void Awake()
    {
        transform.SetParent(null);
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
