using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Image healthFill;              // 빨간 체력 바
    public Image[] phaseIndicators;       // 오른쪽 아래 원 3개
    public Boss boss;                     // 보스 스크립트 참조

    private float maxHp;

    void Start()
    {
        if (boss != null)
            maxHp = boss.hp;
        UpdatePhaseIndicators();
        UpdateHealthBar();
    }

    void Update()
    {
        if (boss == null) return;

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthFill.fillAmount = Mathf.Clamp01(boss.hp / maxHp);
    }

    public void UpdatePhaseIndicators()
    {
        if (boss == null || phaseIndicators.Length == 0) return;

        for (int i = 0; i < phaseIndicators.Length; i++)
        {
            // 현재 페이즈보다 낮으면 빈 원, 높으면 채워진 원
            phaseIndicators[i].enabled = (i >= boss.currentPhase);
        }
    }
}
