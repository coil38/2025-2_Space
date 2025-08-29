using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float lifeTime = 1.5f;     //무조건!! < 반짝이 간격 * 횟수 < lifeTime / 2 >

    [Header("텍스트 오브젝트")]
    [SerializeField] public TextMeshProUGUI textMesh;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Color originalColor;
    private Vector3 moveDirection;
    private float timer = 0f;

    private bool isCritical = false;
    private Vector3 originalScale;
    private bool isFlashing = false;
    private float currentFlashCount;
    private Timer flashTimer;

    void Update()
    {
        FlashEffect();   //반짝임 실행함수
        Move();
    }

    public void Initialized(bool critical)
    {
        isCritical = critical;
        
        //초기화
        if(textMesh != null)
            rectTransform = textMesh.gameObject.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
        if (textMesh != null)
        {
            originalColor = textMesh.color;
            moveDirection = transform.up;  //위로 이동

            //if (rectTransform != null)
            //{
            //    rectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f)); //기울기 랜덤 설정
            //}

            PunchScale(1.1f);
            if (isCritical) StartFlashEffect();   //크리티컬일 경우, 반짝임 효과 재생
        }
    }

    private void Move()
    {
        if (rectTransform == null) return;

        rectTransform.position += moveDirection * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime * 0.5f)
        {
            if (textMesh != null)
            {
                float alpha = Mathf.Lerp(originalColor.a, 0f, (timer - lifeTime * 0.5f / lifeTime * 0.5f));
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            moveSpeed = Mathf.Lerp(moveSpeed, 0.2f, Time.deltaTime * 2f);

            if (textMesh != null && textMesh.color.a <= 0.05f)  //거의 투명해지면 파괴
            {
                Destroy(gameObject);
            }
        }
    }

    private void PunchScale(float intencity)
    {
        rectTransform.DOPunchScale(Vector2.one * intencity, 0.2f);
    }

    private void StartFlashEffect()
    {
        if (textMesh == null) return;
        textMesh.color = Color.white;     //색갈 설정

        //반짝임 시작 실행부
        isFlashing = true;
        currentFlashCount = 10f;
        flashTimer = new Timer(0.05f);
        flashTimer.Start();  //반짝임 타이머 재생 시작
    }

    private void FlashEffect()
    {
        if (textMesh == null || !isFlashing) return;

        if (flashTimer.IsRunning())
        {
            textMesh.alpha = 0.5f;
        }
        else
        {
            textMesh.alpha = 1f;
            currentFlashCount--;
            if (currentFlashCount > 0) flashTimer.Start();
            else isFlashing = false;
        }
    }
}
