using UnityEngine;

public class CurrentSceneManager : MonoBehaviour
{
    public bool isPlayerPresentByDefault = false;

    public static CurrentSceneManager instance;

    private void Awake()
    {
        instance = this;
    }
}
