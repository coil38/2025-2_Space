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

    public GameObject tutorialMonsterPrefab;
    public Transform monsterSpawnPoint;

    bool monsterSpawned = false;

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
                    StartCoroutine(FadeOutAndNextStep(StartStep6)); 
                break;

            case 6:
                chipsetObject.SetActive(true);

                if (chipsetSelectUI != null && chipsetSelectUI.isEquiping)
                {
                    chipsetObject.SetActive(false);
                    StartCoroutine(FadeOutAndNextStep(StartStep7));
                }
                break;

            case 7: 
                if (Input.GetMouseButtonDown(0)) StartCoroutine(FadeOutAndNextStep(StartStep8)); break;

            case 8:
                if (Input.GetKeyDown(KeyCode.Q)) qPressed = true;
                if (Input.GetKeyDown(KeyCode.E)) ePressed = true;
                if (Input.GetKeyDown(KeyCode.R)) rPressed = true;

                if (qPressed && ePressed && rPressed)
                    StartCoroutine(FadeOutAndNextStep(StartStep8_5));
                break;


            case 9:
                chipsetObject.SetActive(true);

                if (chipsetSelectUI != null && chipsetSelectUI.isEquiping)
                {
                    chipsetObject.SetActive(false);
                    StartCoroutine(FadeOutAndNextStep(StartStep10));
                }
                break;

            case 10:
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
        SetPanel("F키를 눌러 칼 무기를 선택하세요");

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
        ResetSkillKeyFlags();
        SetPanel("Q,E,R을 눌러 스킬을 사용해보세요");


    }

    void StartStep6()
    {
        step = 6;
        chipsetSelectUI.ResetEquipState();   
        SetPanel("F키를 눌러 활 무기를 선택하세요");

        chipsetObject.SetActive(true);
    }

    void StartStep7()
    {
        step = 7;
        ResetSkillKeyFlags();
        SetPanel("마우스 왼쪽 클릭으로 공격하세요");
    }

    void StartStep8_5()
    {
        step = 750; 
        SetPanel("모든 스킬은 레벨업으로 잠금 해제 해야 합니다!");

        StartCoroutine(PauseAndNext(2.5f, StartStep9)); 
    }

    void StartStep8()
    {
        step = 8;
        SetPanel("Q,E,R을 눌러 스킬을 사용해보세요");
    }

    void StartStep9()
    {
        step = 9;
        chipsetSelectUI.ResetEquipState();   
        SetPanel("원하는 무기를 고르세요");

        chipsetObject.SetActive(true);
    }

    void StartStep10()
    {
        if (monsterSpawned) return; 
        monsterSpawned = true;

        step = 10;
        SetPanel("앞에 적을 공격 해보세요!");

        GameObject mob = Instantiate(tutorialMonsterPrefab, monsterSpawnPoint.position, tutorialMonsterPrefab.transform.rotation);
        TutorialMonster tm = mob.GetComponent<TutorialMonster>();

        tm.onDead = () =>
        {
            TutorialComplete();
        };
    }

    void TutorialComplete()
    {
        step = 999; 
        SetPanel("튜토리얼 클리어! 10초 후 자동으로 이동합니다.");

        StartCoroutine(GoNextSceneDelay());
    }

    IEnumerator GoNextSceneDelay()
    {
        yield return new WaitForSeconds(10f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("ChipsetSelectScene");
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

        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            yield return null;
        }
    }

    void ResetSkillKeyFlags()
    {
        qPressed = false;
        ePressed = false;
        rPressed = false;
    }

    IEnumerator PauseAndNext(float delay, System.Action next)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutAndNextStep(next));
    }
}
