using UnityEngine;
using UnityEngine.Rendering;
public enum GameState { MissionIntro, Running, Pause, PlayerKilled }
public enum DropGunType { Assault };

public class GameManager : MonoBehaviour
{
    private GameState CurrentGameState;
    [SerializeField]
    public GameObject player;
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private GameObject AssaultGun;

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

    public GameObject GetGunPrefabToDrop(DropGunType gun)
    {
        switch (gun) 
        { 
            case DropGunType.Assault:
                Debug.Log("Providing gun");
                return Instantiate(AssaultGun);
                break;
            
            default:return null;
                break;
        }
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
