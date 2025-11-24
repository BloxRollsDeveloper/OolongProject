using System;
using System.Collections;
using TMPro;
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
    
    [Header("tutorial stuff")]
    public GameObject tutorial;
    public TextMeshProUGUI tutorialSprite;
    public SpriteRenderer spacebarSprite;
    public SpriteRenderer rTriggerSprite;
    public float tutorialDuration;
    public AnimationCurve easeInCurve;
    public AnimationCurve easeOutCurve;
    public bool tutorialVisible;
    
    [Header("WEB BUILD CONTENT")]
    public bool webBuild;
    public GameObject quitButton;
    public GameObject QuitPrompt;

    IEnumerator fadeInTutorial()
    {
        tutorial.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < tutorialDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = easeInCurve.Evaluate(elapsedTime / tutorialDuration);
            tutorialSprite.color = new Color(
                tutorialSprite.color.r, 
                tutorialSprite.color.g, 
                tutorialSprite.color.b, 
                strength);
            spacebarSprite.color = tutorialSprite.color;
            rTriggerSprite.color = tutorialSprite.color;
            
            yield return null;
        }
    }

    IEnumerator fadeOutTutorial()
    {

        float elapsedTime = 0f;

        while (elapsedTime < tutorialDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = easeOutCurve.Evaluate(elapsedTime / tutorialDuration);
            tutorialSprite.color = new Color(
                tutorialSprite.color.r, 
                tutorialSprite.color.g, 
                tutorialSprite.color.b, 
                strength);
            spacebarSprite.color = tutorialSprite.color;
            rTriggerSprite.color = tutorialSprite.color;
            yield return null;
        }
        tutorial.SetActive(false);
    }
    
    public void ShowTutorial()
    {
        StartCoroutine(fadeInTutorial());
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        tutorial.SetActive(false);
        hardModeText.active = false;
        _inputManager = GetComponent<InputManager2>();
        MenuInput = true;
        print("awake");
        MenuUI.gameObject.SetActive(true);
        gameOverInput = false;
        if (webBuild) quitButton.SetActive(false);
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

        if (gameOverInput && _inputManager.Jump)
        {
            RestartGame();
        }

        if (tutorialVisible && bossHead.bossHealth < bossHead.bossHealthMax - 10)
        {
            tutorialVisible = false;
            StartCoroutine(fadeOutTutorial());
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
        if (bossHead.HardMode)
        {
            bossHead.bossHealthMax = 500;
            bossHead.bossHealth = 500;
        }
    }

    public void ExitGame()
    {
        if (webBuild) return;
        Application.Quit();
    }

    public void GameOver()
    {
        gameOverInput = true;
        tutorial.SetActive(false);
        Time.timeScale = 0.25f;
        RestartUI.gameObject.SetActive(true);
        if (webBuild) QuitPrompt.SetActive(false);
    }

    public void RestartGame()
    {
        RestartUI.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    
}
