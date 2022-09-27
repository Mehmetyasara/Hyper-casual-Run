using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    public static LevelController Current;
    public bool gameActive = false;

    public GameObject startMenu, gameUIMenu, gameOverMenu, finishMenu;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;

    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;

    int currentLevel;
    int score;
    void Awake()
    {
        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
    }
    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)
        {
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            PlayerController.Current = GameObject.FindObjectOfType<PlayerController>();
            GameObject.FindObjectOfType<MarketController>().InitializeMarketController();
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
            UpdateMoneyTexts();
        }
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (gameActive)
        {
            PlayerController player = PlayerController.Current;
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }
    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameUIMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running", true);
        gameActive = true;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1));
    }
    public void GameOver()
    {
        UpdateMoneyTexts();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameUIMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()
    {
        GiveMoneyToPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip, 0.5f);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        finishScoreText.text = score.ToString();
        gameUIMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }

    public void ChangeScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }
    public void UpdateMoneyTexts()
    {
        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    }

    public void GiveMoneyToPlayer(int increment)
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyTexts();
    }
}

