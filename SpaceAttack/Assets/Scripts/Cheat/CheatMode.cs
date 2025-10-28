using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMode : MonoBehaviour
{
    [Header("치트 설정")]
    public float attackIncreaseAmount = 10f;  // C키로 증가시킬 공격력 수치
    public int healAmount = 5;                // V키로 회복할 체력 수치
    public KeyCode attackCheatKey = KeyCode.C;
    public KeyCode healKey = KeyCode.V;

    public static CheatMode Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 공격력 증가
        if (Input.GetKeyDown(attackCheatKey))
        {
            IncreasePlayerAttack();
        }

        // 체력 회복
        if (Input.GetKeyDown(healKey))
        {
            HealPlayer();
        }
    }

    private void IncreasePlayerAttack()
    {
        PlayerStatus.normalDamage += attackIncreaseAmount;
        Debug.Log($"[CheatMode] 공격력 +{attackIncreaseAmount}! 현재 공격력: {PlayerStatus.normalDamage}");
    }

    private void HealPlayer()
    {
        if (PlayerStatus.Instance == null)
        {
            return;
        }

        PlayerStatus.AddHp(healAmount);
    }
}
