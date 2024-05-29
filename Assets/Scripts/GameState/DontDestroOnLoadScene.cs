using UnityEngine;

public class DontDestroOnLoadScene : MonoBehaviour
{
    public GameObject[] objects;
    void Awake()
    {
        foreach (var element in objects)
        {
            DontDestroyOnLoad(element);
        }
    }
}