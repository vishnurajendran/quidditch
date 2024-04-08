using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{

    [SerializeField] private CanvasGroup cg;
    [SerializeField] private Transform loadingScreen;
    [SerializeField] private int gameSceneIndx;
    [SerializeField] private Button continueButton;
    [SerializeField] private CanvasGroup fadoutCG;
    
    [RuntimeInitializeOnLoadMethod]
    private static void LoadAudioManager()
    {
        Instantiate(Resources.Load("SceneLoader"));
    }
    
    private void Start()
    {
        DontDestroyOnLoad(this.GameObject());
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timeStep = 0;
        yield return new WaitForEndOfFrame();
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 1f;
            fadoutCG.alpha = Mathf.Lerp(1, 0, timeStep);
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void LoadGameScene()
    {
        Debug.Log("Loading game scene");
        StartCoroutine(WaitWhileLoading(gameSceneIndx, false));
    }
    
    public void LoadMainMenu()
    {
        Debug.Log("Loading main scene");
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        StartCoroutine(WaitWhileLoading(0, true));
    }

    IEnumerator WaitWhileLoading(int sceneIndx, bool allowSceneActivation)
    {
        float timeStep = 0;
        loadingScreen.gameObject.SetActive(true);
        continueButton.interactable = false;
        var text = continueButton.GetComponentInChildren<TMP_Text>();
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime/0.25f;
            cg.alpha = Mathf.Lerp(0,1,timeStep);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        var asyncOp = SceneManager.LoadSceneAsync(sceneIndx, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = allowSceneActivation;
        var targetProg = !allowSceneActivation ? 0.89f : 1;
        while (asyncOp.progress < targetProg)
        {
            text.text = $"{((int)(asyncOp.progress * 100))}";
            yield return new WaitForEndOfFrame();
        }

        if (allowSceneActivation)
        {
            StartCoroutine(ClearLoadingScreen());
        }
        
        text.text = "Continue";
        continueButton.interactable = true;
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            asyncOp.allowSceneActivation = true;
            StartCoroutine(ClearLoadingScreen());
        });
    }

    IEnumerator ClearLoadingScreen()
    {
        float timeStep = 0;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime/0.25f;
            cg.alpha = Mathf.Lerp(1,0,timeStep);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        loadingScreen.gameObject.SetActive(false);
    }
}
