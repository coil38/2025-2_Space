using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageManager : MonoBehaviour
{
    private bool playerDeathHandled = false;

    private void Update()
    {
        if (!playerDeathHandled && PlayerStatus.Instance != null && PlayerStatus.Instance.isDead)
        {
            playerDeathHandled = true;
            StartCoroutine(ReturnToChipsetScene());
        }
    }

    private IEnumerator ReturnToChipsetScene()
    {
        yield return new WaitForSeconds(5f); // 연출용 대기 시간 (3초)
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("ChipsetSelectScene");

        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 0f);

        SaveManager.instance.PlayerReset();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }
}
