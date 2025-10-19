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
    public void ResetHpUI()
    {
        playerHPUI.GenerateHPSlot();
    }

    public void ReducePlayerUI(int hp, int shild_hp, int damage)
    {
        playerHPUI.ReduceHPUI(hp, shild_hp, damage);
    }

    public void UpdatePlayerEXP(ExpInfo expInfo)
    {
        playerCoreUI.UpdateEXP(expInfo);
    }

    public void SetChipsetInfo(Sprite chipsetIcon, Sprite skill1Icon, Sprite skill2Icon, Sprite skill3Icon)
    {
        if (chipsetImage == null || skill1Image == null || skill2Image == null || skill3Image == null) return;

        chipsetImage.sprite = chipsetIcon;
        skill1Image.sprite = skill1Icon;
        skill2Image.sprite = skill2Icon;
        skill3Image.sprite = skill3Icon;
    }

    public void ChangeDarkMaterialUI(bool isAdd, float value)
    {
        playerDarkMaterialUI.ChangeDarkMaterialUI(isAdd, value);
    }

    public void ResetDarkMaterialUI()
    {
        playerDarkMaterialUI.ResetDarkMaterialUI();
    }

    public void SetRelicFloatingUI(bool onFloatingText, BaseRelic relic = null)
    {
        playerRelicFloatingUI.SetFloatingUI(onFloatingText, relic);
    }

    public void SetRelicPopUpUI(bool onPopUpUI, BaseRelic relic = null)
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

    public void OnPlayerInventoryInfo(RelicSO relicSO)
    {
        playerInventoryInfoUI.SetRelicInfoUI(relicSO);
    }
}
