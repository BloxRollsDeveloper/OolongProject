using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCode : MonoBehaviour
{
    private InputManager2 _inputManager;
    public GameObject MenuUI;
    public GameObject RestartUI;
    public GameObject HeartUI;
    public bool skipUI = false;
    public GameObject Player;
    public bool gameOverInput;
    public bool MenuInput;
    public BossHead bossHead;
    public GameObject hardModeText;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hardModeText.active = false;
        _inputManager = GetComponent<InputManager2>();
        MenuInput = true;
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

    private void Update()
    {
        if (MenuInput && _inputManager.Jump) 
        {
            StartGame();
        }

        if (MenuInput && _inputManager.Attack)
        {
            bossHead.HardMode = !bossHead.HardMode;
            hardModeText.SetActive(bossHead.HardMode);
            print(bossHead.HardMode);
        }
    }

    public void StartGame()
    {
        skipUI = true;
        Time.timeScale = 1;
        MenuUI.gameObject.SetActive(false);
        HeartUI.SetActive(true);
        MenuInput = false;
        hardModeText.SetActive(false);
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
