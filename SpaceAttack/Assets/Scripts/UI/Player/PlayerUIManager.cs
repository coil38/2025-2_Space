using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [SerializeField] PlayerHPUI playerHPUI;
    [SerializeField] PlayerCoreUI playerCoreUI;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetHpUI()
    {
        playerHPUI.GenerateHPSlot();
    }

    public void ReducePlayerUI(int hp, int damage)
    {
        playerHPUI.ReduceHPUI(hp, damage);
    }

    public void UpdatePlayerEXP(ExpInfo expInfo)
    {
        playerCoreUI.UpdateEXP(expInfo);
    }
}
