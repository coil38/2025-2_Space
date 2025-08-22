using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SettingUIManager : MonoBehaviour
{
    [Header("InputSystem용 변수")]
    [SerializeField] private InputActionReference horizontalMoveRefer;
    [SerializeField] private InputActionReference verticalMoveRefer;
    [SerializeField] private InputActionReference skill1Refer;
    [SerializeField] private InputActionReference skill2Refer;
    [SerializeField] private InputActionReference skill3Refer;
    [SerializeField] private InputActionReference interactionRefer;
    [SerializeField] private InputActionReference dashRefer;

    [Header("TMP용 변수")]
    [SerializeField] private TMP_InputField upMoveCommand;
    [SerializeField] private TMP_InputField downMoveCommand;
    [SerializeField] private TMP_InputField leftMoveCommand;
    [SerializeField] private TMP_InputField rightMoveCommand;
    [SerializeField] private TMP_InputField skill1Command;
    [SerializeField] private TMP_InputField skill2Command;
    [SerializeField] private TMP_InputField skill3Command;
    [SerializeField] private TMP_InputField interactionCommand;
    [SerializeField] private TMP_InputField dashCommand;

    private InputActionReference currentRefer;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Debug.Log(command.text);
        }
    }

    private void RebindKey()
    {
        var action = currentRefer.action;
        int targetIndex = 0;
        action.PerformInteractiveRebinding(targetIndex)
              .WithControlsExcluding("Mouse")
              .OnComplete(op =>
              {
                  op.Dispose();
                  Debug.Log("Rebind완료: " + action.bindings[targetIndex].effectivePath);
                  SaveRebinds(action.actionMap.asset); //저장
              })
              .Start();

    }

    private void SaveRebinds(InputActionAsset asset)
    {
        var json = asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", json);
        PlayerPrefs.Save();
    }

    private void LoadRebinds(InputActionAsset asset)
    {
        var json = PlayerPrefs.GetString("rebinds", string.Empty);
        if(!string.IsNullOrEmpty(json))
            asset.LoadBindingOverridesFromJson(json);
    }
}
