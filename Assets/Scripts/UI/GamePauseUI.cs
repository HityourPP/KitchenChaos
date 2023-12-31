using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;   
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener((() =>
        {
            GameManager.Instance.PauseGame();
        }));
        mainMenuButton.onClick.AddListener((() =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        }));
        optionsButton.onClick.AddListener((() =>
        {
            OptionsUI.Instance.Show();
        }));
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManagerOnGamePaused;
        GameManager.Instance.OnGameUnPaused += GameManagerOnGameUnPaused;
        Hide();
    }

    private void GameManagerOnGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManagerOnGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {   
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
