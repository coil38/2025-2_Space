using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectUI : MonoBehaviour
{
    // 버튼에서 호출할 함수
    public void LoadStage(string sceneName)
    {

        FadeManager.Instance.LoadScene(sceneName);
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}