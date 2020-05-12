using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    int score = 0;
    bool gameOver = true;
    
    public int Score { get {return score; } }

    enum PageState 
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    public bool GameOver { get {return gameOver; } }

    void Awake() 
    {
        Instance = this;
    }

    void OnEnable() 
    {
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable() 
    {
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

     void OnCountdownFinished() 
    {
        // No pages necessary until player dies again
        SetPageState(PageState.None);
        OnGameStarted(); // Event sent to tapController
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored() 
    {
        score++;
        scoreText.text = score.ToString();
    }

    void OnPlayerDied() 
    {
        gameOver = true;
        // Save the latest highscore
        int savedScore = PlayerPrefs.GetInt("HighScore");
        // If the current score is higher, it is the new highscore
        if (score > savedScore) 
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    // Controls current page state of game UI
    void SetPageState(PageState state) 
    {
        switch(state) 
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            
        }
    }

    // Activates when replay button is clicked
    public void ConfirmGameOver()
    {
        SetPageState(PageState.Start);
         scoreText.text = "0";
        // Event: game objects reset whenever game over occurs
        OnGameOverConfirmed(); // event sent to TapController
    }

    // Activates when play button is hit
    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }
}