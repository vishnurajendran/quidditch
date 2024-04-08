using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Teams;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private GameObject mainMenuObj;
    private void Start()
    {
        settingsManager.OnSettingsClosed += () =>
        {
            mainMenuObj.SetActive(true);
        };
        
        TeamManager.ResetManager();
        AudioManager.Instance.SetupMenuAudio();
        playButton?.onClick.AddListener(OnClickPlay);
        settingsButton?.onClick.AddListener(OnClickSettings);
        quitButton?.onClick.AddListener(OnClickQuit);
    }

    private void OnClickPlay()
    {
       SceneLoader.Instance.LoadGameScene();
    }

    private void OnClickSettings()
    {
        mainMenuObj.SetActive(false);
        settingsManager.ShowSettings();
    }

    private void OnClickQuit()
    {
        Application.Quit();
    }
}
