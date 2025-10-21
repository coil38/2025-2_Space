using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [SerializeField] PlayerHPUI playerHPUI;
    [SerializeField] PlayerCoreUI playerCoreUI;
    [SerializeField] DarkMaterialUI playerDarkMaterialUI;
    [SerializeField] RelicFloatingUI playerRelicFloatingUI;
    [SerializeField] RelicPopUpUI playerRelicPopUpUI;
    [SerializeField] PlayerItemUI playerItemUI;
    [SerializeField] PlayerInventoryInfoUI playerInventoryInfoUI;

    [Header("칩셋 UI 이미지")]
    [SerializeField] Image chipsetImage;
    [SerializeField] Image skill1Image;
    [SerializeField] Image skill2Image;
    [SerializeField] Image skill3Image;

    private InventoryManager inventoryManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ResetHpUI()                                           //체력UI 초기화 함수
    {
        playerHPUI.GenerateHPSlot();
    }

    public void ReducePlayerUI(int hp, int shild_hp, int damage)      //체력UI 감소 함수
    {
        playerHPUI.ReduceHPUI(hp, shild_hp, damage);
    }

    public void UpdatePlayerEXP(ExpInfo expInfo)                      //경험치UI 증가 함수
    {
        playerCoreUI.UpdateEXP(expInfo);
    }

    public void SetChipsetInfo(Sprite chipsetIcon, Sprite skill1Icon, Sprite skill2Icon, Sprite skill3Icon)       //스킬, 무기 이미지 설정 함수
    {
        if (chipsetImage == null || skill1Image == null || skill2Image == null || skill3Image == null) return;

        chipsetImage.sprite = chipsetIcon;
        skill1Image.sprite = skill1Icon;
        skill2Image.sprite = skill2Icon;
        skill3Image.sprite = skill3Icon;
    }

    public void ChangeDarkMaterialUI(bool isAdd, float value)          //현재 암흑물질수치 변경 함수
    {
        playerDarkMaterialUI.ChangeDarkMaterialUI(isAdd, value);
    }

    public void ResetDarkMaterialUI()                                  //암흑물질수치 게이지 초기화 함수
    {
        playerDarkMaterialUI.ResetDarkMaterialUI();
    }
    public void ChangeMaxDarkMaterial(int value)                       //최대 암흑물질수치 변경 함수
    {
        playerDarkMaterialUI.ChangeMaxDarkMaterial(value);
    }

    public void SetRelicFloatingUI(bool onFloatingText, BaseRelic relic = null)    //유물 가이드 플로팅UI 활성화 함수
    {
        playerRelicFloatingUI.SetFloatingUI(onFloatingText, relic);
    }

    public void SetRelicPopUpUI(bool onPopUpUI, BaseRelic relic = null)            //유물팝업UI 활성화 함수
    {
        playerRelicPopUpUI.SetRelicPopUpUI(onPopUpUI, relic);
    }

    public void SetPlayerItem(RelicSO relicSO)      //플레이어 유물 이미지 추가
    {
        playerItemUI.AddItem(relicSO);
    }
    public void RemovePlayerItem(RelicSO relicSO)   //플레이어 유물 이미지 삭제
    {
        if(inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.DropRelic(relicSO);
        inventoryManager.RemoveRelic(relicSO);
        playerItemUI.RemoveItem(relicSO);
    }
    public void ClearPlayerItem()                   //모든 유물 이미지 삭제
    {
        playerItemUI.RemoveAllItems();
    }

    public void OnPlayerInventoryInfo(RelicSO relicSO)          //플레이어 인벤토리정보UI 활성화
    {
        playerInventoryInfoUI.SetRelicInfoUI(relicSO);
    }
}
