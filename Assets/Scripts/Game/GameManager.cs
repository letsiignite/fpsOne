using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public enum GameState { MissionIntro, Running, Pause, PlayerKilled }
public enum DropGunType { Assault, Sniper };

public class GameManager : MonoBehaviour
{
    private GameState CurrentGameState;
    [SerializeField]
    public GameObject player;
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private GameObject AssaultGun;
    [SerializeField]
    private GameObject SniperGun;
    [SerializeField]
    private GameObject OnKillCrosshair;
    private List<Action> onPlayerDeathCallbacks = new List<Action>();
    private List<Action> onGameRunningCallbacks = new List<Action>();

    private PlayerController playerController;

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
    private void OnDisable()
    {
        
    }

    public void ShowOnKillCrosshair()
    { 
        OnKillCrosshair.SetActive(true);
    }

    public void ApplySniperHitToPlayer()
    {
        if (CurrentGameState != GameState.Running)
            return;

        if (player != null)
        {
            Debug.Log(" ++ ApplySniperHitToPlayer ++");
            playerController = (playerController == null) ? player.GetComponent<PlayerController>(): playerController;
            playerController.ApplySniperHitEffect();
        }
    }


    public void AddOnPlayerDeathCallbacks(Action callback)
    {
        if(!onPlayerDeathCallbacks.Contains(callback))
        onPlayerDeathCallbacks.Add(callback);
    }

    public void AddOnGameRunningCallbacks(Action callback)
    {
        if(!onGameRunningCallbacks.Contains(callback))
        onGameRunningCallbacks.Add(callback);
    }

    public void SetGameState(GameState gameState)
    {
        CurrentGameState = gameState;

        switch (gameState)
        {
            case GameState.PlayerKilled:
                foreach (Action callback in onPlayerDeathCallbacks)
                { 
                    callback.Invoke();
                }
                break;

            case GameState.Running:
                foreach (Action callback in onGameRunningCallbacks)
                {
                    callback.Invoke();
                }
                break;

        } 
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
            case DropGunType.Sniper:
                Debug.Log("Providing Sniper gun");
                return Instantiate(SniperGun);

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
