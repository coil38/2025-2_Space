using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;
    public bool isSceneLoading { get; private set; }
    private event Action endLoadEvent;      //씬 로드하기 전에 한번 실행되고 초기화되는 이벤트

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void LoadScene(string sceneName)
    {
        if (isSceneLoading) return;

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isSceneLoading = true;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
        {
            yield return null;
        }

        endLoadEvent?.Invoke();   //씬 로드 종료시, 실행
        endLoadEvent = null;
        isSceneLoading = false;
    }

    public void AddEvent(Action action)
    {
        endLoadEvent += action;
    }
}
