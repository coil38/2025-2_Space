using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteFileConfirmationUI : ConfirmationUIStrategy
{
    [SerializeField] Button _leftButton, _midleButton, _rightButton;
    [SerializeField] TextMeshProUGUI _leftButtonText, _midleButtonText, _rightButtonText;
    [SerializeField] TextMeshProUGUI _contentText;
    [SerializeField] Sprite _exitLeftSprite, _exitMidleSprite, _exitRightSprite;
    [SerializeField] ConfirmationUI confirmationUI;

    [Header("설정UI")]
    [SerializeField] private StartGameUI startGameUI;

    public override void Execute()
    {
        _leftButtonText.text = "삭제";
        _midleButtonText.text = "삭제 후 시작";
        _rightButtonText.text = "취소";
        _contentText.text = "해당 저장파일을 삭제하시겠습니까? 이 작업은 되돌릴수 없습니다.";
        //각각의 버튼들 Sprite추가
        confirmationUI.gameObject.SetActive(true);

        _leftButton.onClick.AddListener(() =>
        {
            LogUtil.Log("파일 삭제 버튼 동작");
            if (startGameUI != null)
            {
                //해당 파일이 삭제되고 버튼 텍스트도 변경
                startGameUI.CheckAndProcessDeleteMode();
                startGameUI.DeleteFile();
            }
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
            UISoundManager.PlayeButtonClickSound();
        });

        _midleButton.onClick.AddListener(() =>
        {
            LogUtil.Log("파일 삭제후 시작 동작");
            if (startGameUI != null)
            {
                //해당 파일이 삭제되고 버튼 텍스트도 변경
                //바로 파일 버튼 클릭 판정
                startGameUI.CheckAndProcessDeleteMode();
                startGameUI.DeleteFile();
                startGameUI.PlayeNewGame();
            }
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
            UISoundManager.PlayeButtonClickSound();
        });

        _rightButton.onClick.AddListener(() =>
        {
            LogUtil.Log("파일 삭제 취소 동작");
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
            UISoundManager.PlayeButtonClickSound();
        });
    }
}
