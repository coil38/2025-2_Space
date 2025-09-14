using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExitConfirmationUI : SettingUIStrategy
{
    [SerializeField] Button _leftButton, _midleButton, _rightButton;
    [SerializeField] TextMeshProUGUI _leftButtonText, _midleButtonText, _rightButtonText;
    [SerializeField] TextMeshProUGUI _contentText;
    [SerializeField] Sprite _exitLeftSprite, _exitMidleSprite, _exitRightSprite;
    [SerializeField] ConfirmationUI confirmationUI;

    [Header("설정UI")]
    [SerializeField] private SettingUIManager settingUIManager;

    public override void Execute()
    {
        _leftButtonText.text = "Save & Exit";
        _midleButtonText.text = "Exit";
        _rightButtonText.text = "Cancel";
        _contentText.text = "Your changes have not been saved. Would you like to exit?";
        //각각의 버튼들 Sprite추가
        confirmationUI.gameObject.SetActive(true);

        _leftButton.onClick.AddListener(() =>
        {
            LogUtil.Log("저장 버튼 동작");
            if (settingUIManager != null)
            {
                settingUIManager.SaveSetting();  //저장하고 나가기
                settingUIManager.gameObject.SetActive(false);   //나가기 기능
            }
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);

        });

        _midleButton.onClick.AddListener(() =>
        {
            LogUtil.Log("저장하지 않고 나가기 동작");
            if (settingUIManager != null)
            {
                settingUIManager.CancelSave();    //저장하지 않고 나가기
                settingUIManager.gameObject.SetActive(false);   //나가기 기능
            }
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);

        });

        _rightButton.onClick.AddListener(() =>
        {
            LogUtil.Log("취소 동작");
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
        });
    }

}
