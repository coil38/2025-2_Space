using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExitConfirmationUI : ConfirmationUIStrategy
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
        _leftButtonText.text = "저장 후 나가기";
        _midleButtonText.text = "그냥 나가기";
        _rightButtonText.text = "취소";
        _contentText.text = "변경 사항이 저장되지 않았습니다. 나가시겠습니까?";
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
            UISoundManager.PlayeButtonClickSound();
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
            UISoundManager.PlayeButtonClickSound();
        });

        _rightButton.onClick.AddListener(() =>
        {
            LogUtil.Log("취소 동작");
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
            UISoundManager.PlayeButtonClickSound();
        });
    }

}
