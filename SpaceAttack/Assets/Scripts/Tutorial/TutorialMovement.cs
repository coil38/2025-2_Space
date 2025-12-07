using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMovement : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Text tutorialText;
    public float fadeDuration = 1f;
    private ChipsetSelectUI chipsetSelectUI;


    public GameObject chipsetObject;   

    bool wPressed, aPressed, sPressed, dPressed, qPressed, ePressed, rPressed;
    bool dashPressed;
    

    int step = 0; 


    private void Awake()
    {
        chipsetSelectUI = FindObjectOfType<ChipsetSelectUI>(true);

    }
    void Start()
    {
        StartStep1();
    }

    void Update()
    {
        switch (step)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W)) wPressed = true;
                if (Input.GetKeyDown(KeyCode.A)) aPressed = true;
                if (Input.GetKeyDown(KeyCode.S)) sPressed = true;
                if (Input.GetKeyDown(KeyCode.D)) dPressed = true;

                if (wPressed && aPressed && sPressed && dPressed)
                    StartCoroutine(FadeOutAndNextStep(StartStep2));
                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    dashPressed = true;
                    StartCoroutine(FadeOutAndNextStep(StartStep3));
                }
                break;

            case 3:
                chipsetObject.SetActive(true);  

                if (chipsetSelectUI != null && chipsetSelectUI.isEquiping)
                {
                    chipsetObject.SetActive(false);
                    StartCoroutine(FadeOutAndNextStep(StartStep4));
                }
                break;

            case 4:
                if (Input.GetMouseButtonDown(0))
                    StartCoroutine(FadeOutAndNextStep(StartStep5));
                break;

            case 5:
                if (Input.GetKeyDown(KeyCode.Q)) qPressed = true;
                if (Input.GetKeyDown(KeyCode.E)) ePressed = true;
                if (Input.GetKeyDown(KeyCode.R)) rPressed = true;

                if (qPressed && ePressed && rPressed)
                    StartCoroutine(FadeOutAndNextStep(TutorialComplete)); 
                break;
        }
    }

    void StartStep1()
    {
        step = 1;
        SetPanel("WASD를 이용하여 움직이시오");
    }

    void StartStep2()
    {
        step = 2;
        SetPanel("스페이스바를 눌러 대쉬하시오");
    }

    void StartStep3()
    {
        step = 3;
        SetPanel("F키를 눌러 무기를 선택하세요");

        chipsetObject.SetActive(true); 
    }

    void StartStep4()
    {
        step = 4;
        SetPanel("마우스 왼쪽 클릭으로 공격하세요");
    }

    void StartStep5()
    {
        step = 5;
        SetPanel("Q,E,R을 눌러 스킬을 사용해보세요");
    }
    void TutorialComplete()
    {
        Debug.Log("튜토리얼 전체 종료!");
    }

    void SetPanel(string text)
    {
        tutorialPanel.SetActive(true);
        tutorialPanel.GetComponent<CanvasGroup>().alpha = 1;
        tutorialText.text = text;
    }

    IEnumerator FadeOutAndNextStep(System.Action nextStepFunc)
    {
        CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }

        nextStepFunc?.Invoke();

        cg.alpha = 0;
        tutorialPanel.SetActive(true);
        yield return null;

        // Fade in
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            yield return null;
        }
    }
}
