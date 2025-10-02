using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCode : MonoBehaviour
{
    public GameObject MenuUI;
    public GameObject RestartUI;
    public GameObject HeartUI;
    public bool skipUI = false;
    public GameObject Player;
    public bool gameOverInput;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        print("awake");
        MenuUI.gameObject.SetActive(true);
        gameOverInput = false;
        if (!skipUI)
        {
            HeartUI.SetActive(false);
            Time.timeScale = 0;
        }

        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        print("start");
        if (skipUI)
        {
            Time.timeScale = 1;
            MenuUI.gameObject.SetActive(false);
            HeartUI.SetActive(true);
            Player.transform.position = new Vector3(0, -4, 0);
        }
    }

    public void StartGame()
    {
        skipUI = true;
        Time.timeScale = 1;
        MenuUI.gameObject.SetActive(false);
        HeartUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        Time.timeScale = 0.25f;
        RestartUI.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        RestartUI.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    
}
