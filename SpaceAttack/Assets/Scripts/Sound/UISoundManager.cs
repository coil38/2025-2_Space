using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISoundManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TEst());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TEst()
    {
        yield return new WaitForSeconds(1);    //임시
        Initialized(); 
        Debug.Log($"{gameObject.name}에서 작동한다");
    }

    private void Initialized()
    {
        SoundManager.instance.RegisterGameObjectBySoundType(gameObject, SoundType.UI);
    }

    public static void PlayeButtonClick()  //버튼 클릭 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4003, SoundType.UI);
    }
    public static void PlayeOnAndOffPanel()  //버튼 클릭 사운드 재생
    {
        SoundManager.instance.PlayBGMOrUISound(4004, SoundType.UI);
    }
}
