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

    [Header("칩셋 UI 이미지")]
    [SerializeField] Image chipsetImage;
    [SerializeField] Image skill1Image;
    [SerializeField] Image skill2Image;
    [SerializeField] Image skill3Image;
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

    public void ReducePlayerUI(int hp, int damage)
    {
        playerHPUI.ReduceHPUI(hp, damage);
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
}
