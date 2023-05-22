using UnityEngine;

public class DontDestroyOnLoadScene : MonoBehaviour
{
   
    private static DontDestroyOnLoadScene instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
