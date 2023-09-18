using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;
    private enum State{
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }
    private State state;
    private float countToStartTimer = 3f;    
    private float gamePlayingTimer;    
    private float gamePlayingTimerMax = 180f;
    private bool isGamePaused;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start()
    {
        InputManager.Instance.OnPauseAction += InputManagerOnPauseAction;
        InputManager.Instance.OnInteractionAction += InputManagerOnInteractionAction;
    }

    private void InputManagerOnInteractionAction(object sender, EventArgs e)
    {
        if (state == State.WaitingToStart)
        {
            state = State.CountDownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void InputManagerOnPauseAction(object sender, EventArgs e)
    {
        PauseGame();
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:

                break;
            case State.CountDownToStart:
                countToStartTimer -= Time.deltaTime;
                if (countToStartTimer < 0)
                {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
               gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
        // Debug.Log(state);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountDownStart()
    {
        return state == State.CountDownToStart;  
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public float GetCountDownStartTimer()
    {
        return countToStartTimer;
    }

    public float GetGamePlayingNormalize()
    {
        return 1 - gamePlayingTimer / gamePlayingTimerMax;
    }

    public void PauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;    //Time.timeScale会影响fixUpdate函数的执行，同时还会影响时间的流逝速度
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
}
