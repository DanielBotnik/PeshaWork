using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private Text currentScore;
    private Text bestScore;
    private Text timerText;
    private Text pauseTimerText;
    private bool prevTimerEnabled;
    private GameObject pausePanel;
    private GameObject winPanel;
    private GameObject pauseButton;
    void Start()
    {
        InitFields();
        RegisterToEvents();
    }

    private void InitFields()
    {
        currentScore = GameObject.Find("CurrentScoreText").GetComponent<Text>();
        bestScore = GameObject.Find("BestScoreText").GetComponent<Text>();
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        pausePanel = GameObject.Find("PausePanel");
        pauseButton = GameObject.Find("PauseButton");
        winPanel = GameObject.Find("WinPanel");
        pauseTimerText = GameObject.Find("PauseTimerText").GetComponent<Text>();
        bestScore.text = $"Best Score: {PlayerPrefs.GetInt("bestScore")}";
        timerText.enabled = false;
        prevTimerEnabled = false;
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void RegisterToEvents()
    {
        GameHandler gameHandler = GameHandler.GetInstance();
        gameHandler.UpdateCurrentScoreEvent += UpdateCurrentScore;
        gameHandler.UpdateBestScoreEvent += UpdateBestScore;
        gameHandler.UpdateTimerEvent += UpdateTimer;
        gameHandler.TimerOnEvent += SetTimerOn;
        gameHandler.TimerOffEvent += SetTimerOff;
        gameHandler.SetPauseEvent += Pause;
        gameHandler.SetUnPauseEvent += UnPause;
        gameHandler.UpdatePauseTimerEvent += UpdatePauseTimer;
        gameHandler.OnWinEvent += OnWin;
        gameHandler.OnWinClickEvent += OnWinClick;
    }

    private void UpdateCurrentScore(int score)
    {
        currentScore.text = $"Score: {score}";
    }

    private void UpdateBestScore(int score)
    {
        bestScore.text = $"Best Score: {score}";
    }

    private void UpdateTimer(string timer)
    {
        timerText.text = timer;
    }

    private void SetTimerOn()
    {
        timerText.enabled = true;
    }

    private void SetTimerOff()
    {
        timerText.enabled = false;
    }

    private void UpdatePauseTimer(string timer)
    {
        pauseTimerText.text = timer;
    }

    private void Pause()
    {
        prevTimerEnabled = timerText.enabled;
        currentScore.enabled = false;
        bestScore.enabled = false;
        timerText.enabled = false;
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void UnPause()
    {
        currentScore.enabled = true;
        bestScore.enabled = true;
        timerText.enabled = prevTimerEnabled;
        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
    }

    private void OnWin()
    { 
        timerText.enabled = false;
        pauseButton.SetActive(false);
        winPanel.SetActive(true);
    }

    private void OnWinClick()
    {
        pauseButton.SetActive(true);
        winPanel.SetActive(false);
    }

}
