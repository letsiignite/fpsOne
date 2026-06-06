using Game;
using Mission;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickAudio;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private MissionManager missionManager;
    [SerializeField]
    private DeathScreen deathScreen;
    [SerializeField]
    private GameObject onDeathMenu;
    [SerializeField]
    private GameObject resumeButton;
    [SerializeField]
    private GameObject quitButton;
    [SerializeField]
    private GameObject missionCompleteScreen;
    [SerializeField]
    private HitPointsSystem hitPointsSystem;
    [SerializeField]
    private TMP_Text fpsText;
    [SerializeField]
    private GameObject keyMapping;
    [SerializeField]
    private GameObject pauseMenu;

    private string GAME_SCENE_NAME = "GameScene";
    private float deltaTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = 1.0f / deltaTime;

        if(fpsText != null)
        fpsText.text = Mathf.Ceil(fps).ToString();
    }

    public void StartGame()
    {
        PlayClickSound();
        Invoke("LoadGameScene", 0.3f);
    }
    private void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }

    public void ExitGame()
    {
        PlayClickSound();
        Time.timeScale = 1;
        Invoke("CloseGame", 0.3f);
    }

    private void CloseGame()
    { 
        Application.Quit();
    }

    private void PlayClickSound()
    {
        audioSource.Stop();
        audioSource.clip = buttonClickAudio;
        audioSource.Play();
    }

    public void DisplayPauseMenu()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            AudioListener.pause = true;
            GameManager.Instance.SetGameState(GameState.Pause);
            Time.timeScale = 0f;
            missionManager.DisablePlayerMovement();
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 01f;
            AudioListener.pause = false;
            GameManager.Instance.SetGameState(GameState.Running);
            missionManager.EnablePlayerMovements();
        }
    }

    public void DisplayMissionComplitionScreen()
    {
        missionCompleteScreen.SetActive(true);
        onDeathMenu.SetActive(true);
        GameManager.Instance.SetGameState(GameState.PlayerKilled);
        missionManager.DisablePlayerMovement();
    }
    

    public void DisplayDeathScreen()
    {
        onDeathMenu.SetActive(true);
        deathScreen.DisplayDeathScreen();
    }

    public void LoadLastCheckpoint()
    {
        PlayClickSound();
        Invoke("LoadCheckpoint", 0.3f);
    }

    private void LoadCheckpoint()
    {
        
        hitPointsSystem.HandleLoadLastCheckpoint();
        
        missionManager.Respawn();
    }

    public void HideMissionMenus()
    {
        onDeathMenu.SetActive(false);
        deathScreen.HideDeathScreen();
    }

    public void ShowKeyMapping()
    {
        PlayClickSound();
        keyMapping.SetActive(!keyMapping.activeInHierarchy);
    }
}
