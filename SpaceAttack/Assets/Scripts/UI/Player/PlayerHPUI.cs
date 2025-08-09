using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private GameObject HPSlot;   //체력 슬롯 프리팹

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateHPSlot()
    {
        int hp = PlayerStatus.m_hp;
    }
}
