using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetConfirmationUI : ConfirmationUIStrategy
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
        _leftButtonText.text = "초기화";
        _midleButtonText.text = "취소";
        _contentText.text = "모든 설정을 기본값으로 되돌리시겠습니까? 이 작업은 되돌릴 수 없습니다.";
        //각각의 버튼들 Sprite추가
        _rightButton.gameObject.SetActive(false);
        confirmationUI.gameObject.SetActive(true);

        _leftButton.onClick.AddListener(() =>
        {
            LogUtil.Log("초기화 버튼 동작");
            if (settingUIManager != null)
            {
                settingUIManager.isChanged.Clear();  //저장 체크용 변경 사항 초기화
                settingUIManager.ResetSetting();   //설정 초기화
            }
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
            UISoundManager.PlayeButtonClickSound();
        });

        _midleButton.onClick.AddListener(() =>
        {
            LogUtil.Log("취소 버튼 동작");
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
            UISoundManager.PlayeButtonClickSound();
        });
    }
}
