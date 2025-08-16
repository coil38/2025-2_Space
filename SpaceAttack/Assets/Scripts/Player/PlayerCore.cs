using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    public static int Level {  get; private set; }
    public static int DarkMaterialCount;
    private int[] DarkMaterialRequired = new int[3];

    private static int maxLevel = 100;
    void Start()
    {
        //데이터에서 필요한 암흑물질의 레벨을 받아서 DarkMatterRequired에 받음.
    }

    void Update() 
    {
        //테스용
        if (Input.GetKeyDown(KeyCode.V))
        {
            GetDarkMatter(15);
        }
    }

    public static void GetDarkMatter(int exp)     //암흑물질(경험치) 획득
    {
        if (PlayerUIManager.instance == null)     //예외처리
        {
            Debug.LogError("플레이어 메인 UI를 찾을 수 없습니다.");
            return;
        }

        ExpInfo expInfo = new ExpInfo(DarkMaterialCount, DarkMaterialCount + exp, maxLevel, Level, 110);  //경험치업에 필요한 정보생성
        PlayerUIManager.instance.UpdatePlayerEXP(expInfo);     //UI 갱신 및 연출 재생

        DarkMaterialCount += exp;

        if (DarkMaterialCount >= maxLevel)                      //(임시) 현재 레벨이 최대 레벨을 넘었을 경우
        {
            int remainExpCount = DarkMaterialCount - maxLevel;
            maxLevel = 110;                                     //임시 (새로운 최대경험치량 업데이트)
            DarkMaterialCount = remainExpCount;

            LevelUp();  //레벨업
            
            if(remainExpCount > 0) GetDarkMatter(0);  //남은 경험치 재획득처리
        }

    }

    private static void LevelUp()
    {
        Level++;
        //EventManager.f_CorrectionValueEvent.FindCorectionValue(Level);  //이벤트 실행
    }

    public void SetCorrectionValue(Object obj, FindCorectionValueEvent e)
    {
        Debug.Log($"{e.test}를 플레이어 보정치에 주입");
    }
}
