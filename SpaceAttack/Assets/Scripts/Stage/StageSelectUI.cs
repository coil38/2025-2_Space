using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectUI : MonoBehaviour
{
    // 버튼에서 호출할 함수
    public void LoadStage(string sceneName)
    {
        StartCoroutine(C_LoadScene(sceneName));
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator C_LoadScene(string sceneName)
    {
        PlayerStatus.Instance.isRooted = false;  //플레이어 속박처리 취소
        yield return new WaitUntil(() => PlayerStatus.Instance.isRooted == false);  //플레이어 속박해제후, 씬로드
        FadeManager.Instance.LoadScene(sceneName);
    }
}