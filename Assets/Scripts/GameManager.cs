using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,Playing,Pause,VideoPlayer,
    }
    public static GameManager Instance;
    public GameState currentGameState = GameState.Playing;
    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetState(GameState state)
    {
        currentGameState = state;
    }
}
