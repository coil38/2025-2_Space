using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationUI : MonoBehaviour
{
    [SerializeField] Button _leftButton, _midleButton, _rightButton;
    [SerializeField] TextMeshProUGUI _leftButtonText, _midleButtonText, _rightButtonText;
    [SerializeField] TextMeshProUGUI _contentText;

    public void Show(ConfirmationUIStrategy strategy)
    {
        ResetUI();
        strategy.Execute();
        UIESCSystem.SetUIDepth(UIESCSystem.GetCurrentUIType() == UIType.PauseUI? UIType.PauseUI : UIType.StartSceneUI,
            () => gameObject.SetActive(false), gameObject);
    }

    private void ResetUI()
    {
        _leftButton.onClick.RemoveAllListeners();
        _midleButton.onClick.RemoveAllListeners();
        _rightButton.onClick.RemoveAllListeners();
        _rightButton.gameObject.SetActive(true);
    }
}
