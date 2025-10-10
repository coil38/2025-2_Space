using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;
    public bool isSceneLoading { get; private set; }

    private void Awake()
    {
        if(instance == null)
            instance = this;
         
        StartCoroutine(WaitUtilInitializeDon());   //게임을 키고 처음 한번만 실행
    }

    public void LoadScene(string sceneName)
    {
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

        SetSceneCondition();

        isSceneLoading = false;
    }

    private void SetSceneCondition()  //씬 타임스케일여부 관리
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "StartUIScene")
        {
            Time.timeScale = 0;
            LogUtil.Log("일시정지 처리");
        }
        else
        {
            Time.timeScale = 1;
            LogUtil.Log("일시정지 취소 처리");
        }
    }

    private IEnumerator WaitUtilInitializeDon()
    {
        yield return new WaitUntil(() => InitializationOrderer.instance != null);
        yield return new WaitUntil(() => InitializationOrderer.instance.isInitializeDon);
        LogUtil.Log("초기화 종료");
        SetSceneCondition();  //씬 타임스케일여부 설정
    }
}
