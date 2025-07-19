using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public static int Level {  get; private set; }
    private int[] DarkMatterRequired = new int[3];
    void Start()
    {
        //데이터에서 필요한 암흑물질의 레벨을 받아서 DarkMatterRequired에 받음.
    }

    void Update()
    {
        
    }

    private void DetectDarkMatter()
    {
        //암흑물질(경험치) 감지
    }

    private void GetDarkMatter()
    {
        //암흑물질(경험치) 획득
    }

    private void LevelUp()
    {
        //현재 레벨이 필요레벨을 초과했을 때, 실행

        EventManager.f_CorrectionValueEvent.FindCorectionValue(Level);  //이벤트 실행
    }

    public void SetCorrectionValue(Object obj, FindCorectionValueEvent e)
    {
        Debug.Log($"{e.test}를 플레이어 보정치에 주입");
    }
}
