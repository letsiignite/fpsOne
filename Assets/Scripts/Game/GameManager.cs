using UnityEngine;
using UnityEngine.Rendering;
public enum GameState { Running, Pause, PlayerKilled }

public class GameManager : MonoBehaviour
{
    

    private GameState CurrentGameState;
    public GameObject player;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if the singleton needs to persist across scenes
        }
    }

    public void SetGameState(GameState gameState)
    {
        CurrentGameState = gameState;
    }

    public GameState GetGameState() 
    {
        return CurrentGameState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
