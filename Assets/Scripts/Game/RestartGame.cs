using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Game;


public class RestartGame : MonoBehaviour
{
    public GameObject Death;
    public GameObject DeathScreen;
    public bool isDead = false;
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(1);
    }
    void Update()
    {

        if (isDead)
        {
            StartCoroutine("DeathScreenLoader");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isDead = false;
        }
    }
    public IEnumerator DeathScreenLoader()
    {
        yield return new WaitForSeconds(2);
        DeathScreen.SetActive(true);
    }
    void Start()
    {
        DeathScreen.SetActive(false);
    }


}
