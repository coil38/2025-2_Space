using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCoreUI : MonoBehaviour
{
    [SerializeField] private Text maxExpText;
    [SerializeField] private Text expText;
    [SerializeField] private Text levelText;
    [SerializeField] private Slider expSlider;

    private Coroutine currentCor;
    private Queue<ExpInfo> expInfos = new Queue<ExpInfo>();

    public void UpdateEXP(ExpInfo expInfo)
    {
        if (currentCor != null)
        {
            expInfos.Enqueue(expInfo);   //현재 레벨업 중일 경우, 대기
            return;
        }

        if (expInfo.targetExp >= expInfo.maxExp)  //레벨업 조건 충족 되었다면
        {
            currentCor = StartCoroutine(LevelUp(expInfo));  //nextMaxExp는 리스트에서 받아오기
        }
        else
        {
            //변수 할당
            Variables.Object(this.gameObject).Set("currentExp", expInfo.currentExp);
            Variables.Object(this.gameObject).Set("targetExp", expInfo.targetExp);
            Variables.Object(this.gameObject).Set("maxExp", expInfo.maxExp);

            if (expInfo.isInitial)  //초기화 설정
            {
                //UI텍스트 갱신
                levelText.text = expInfo.currentLevel.ToString(); //현재 레벨 갱신
                expText.text = expInfo.currentExp.ToString(); //현재 경험치량
                maxExpText.text = "/" + expInfo.maxExp;  //최대 경험치량

                expSlider.maxValue = expInfo.maxExp;  //최대 경험치량 (슬라이더용)
                expSlider.minValue = 0f;
                expSlider.value = expInfo.currentExp; //현재 경험치량 (슬라이더용)
            }
            else
            {
                //함수 실행
                CustomEvent.Trigger(this.gameObject, "UpdateExp");
            }
        }
    }

    private void Update()
    {
        //Debug.Log($"현재 대기자수: {expInfos.Count}");

        if (expInfos.Count > 0 && currentCor == null)  //대기중인 정보들 업데이트
        {
            if (expInfos.TryDequeue(out ExpInfo info))
            {
                UpdateEXP(info);
            }
        }
    }
    private IEnumerator LevelUp(ExpInfo expInfo)
    {
        Variables.Object(this.gameObject).Set("currentExp", expInfo.currentExp);    //변수 할당
        Variables.Object(this.gameObject).Set("targetExp", expInfo.targetExp);
        Variables.Object(this.gameObject).Set("maxExp", expInfo.maxExp);
        CustomEvent.Trigger(this.gameObject, "UpdateExp");  //경험치 상승 연출 시작

        bool isLevelUping = Variables.Object(this.gameObject).Get<bool>("isLevelUping");

        Debug.Log("대기중..");
        yield return new WaitUntil(() => Variables.Object(this.gameObject).Get<bool>("isLevelUping"));      //레벨업 연출 끝날 때까지 대기
        Variables.Object(this.gameObject).Set("isLevelUping", false);
        Debug.Log("대기 종료");

        Variables.Object(this.gameObject).Set("currentLevel", expInfo.currentLevel);   //변수 할당
        Variables.Object(this.gameObject).Set("targetLevel", expInfo.currentLevel + 1);
        CustomEvent.Trigger(this.gameObject, "UpdateLevel");  //레벨업 연출 시작

        expText.text = "0";                          //현재 경험치량
        maxExpText.text = "/" + expInfo.nextMaxExp;  //최대 경험치량
        expSlider.value = 0;                          //현재 경험치량 (슬라이더용)
        expSlider.maxValue = expInfo.nextMaxExp;      //최대 경험치량 (슬라이더용)
        currentCor = null;

        yield return null;
    }
}
