using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageClearUI : MonoBehaviour
{
    public GameObject clearMessageObj; // Text 오브젝트
    public float delayToReturn = 3f;   // 몇 초 후에 돌아갈지

    public void ShowClearMessage()
    {
        if (clearMessageObj != null)
        {
            clearMessageObj.SetActive(true);
            clearMessageObj.GetComponent<Text>().text = "칩셋 선택씬으로 돌아갑니다...";
        }

        StartCoroutine(ReturnToStageSelect());
    }

    private IEnumerator ReturnToStageSelect()
    {
        yield return new WaitForSeconds(delayToReturn);
        if(SaveManager.instance != null)
            SaveManager.instance.PlayerReset();
    }
}
