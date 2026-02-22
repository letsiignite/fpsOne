using Game;
using mission;
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
    private HitPointsSystem hitPointsSystem;

    private string GAME_SCENE_NAME = "GameScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
