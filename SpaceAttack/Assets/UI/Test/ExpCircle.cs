using UnityEngine;
using UnityEngine.UI;

public class ExpCircle : MonoBehaviour
{
    public Image expFill; // 이미지 연결할 변수
    public float targetFill = 0f; // 목표 경험치 비율 (0~1)
    public float fillSpeed = 0.5f; // 채워지는 속도

    void Update()
    {
        // 현재 FillAmount를 목표로 부드럽게 보간
        expFill.fillAmount = Mathf.Lerp(expFill.fillAmount, targetFill, Time.deltaTime * fillSpeed);
    }

    // 외부에서 경험치 추가할 때 호출
    public void AddExp(float amount)
    {
        targetFill = Mathf.Clamp01(targetFill + amount);
    }
}
