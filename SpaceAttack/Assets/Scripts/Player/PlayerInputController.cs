using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputController : MonoBehaviour
{
    private PlayerInput playerInput;

    private static InputAction verticalAction;
    private static InputAction horizontalAction;
    private float smoothTime = 0.5f;
    private float h_timer;
    private float v_timer;

    //외부에서 접근가능한 필드
    public static float verticalValue;
    public static float horizontalValue;
    public static InputAction attackAction;
    public static InputAction skill1Action;
    public static InputAction skill2Action;
    public static InputAction skill3Action;
    public static InputAction interactionAction;
    public static InputAction dashAction;
    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        verticalAction = playerInput.actions["Vertical"];
        horizontalAction = playerInput.actions["Horizontal"];
        attackAction = playerInput.actions["Attack"];
        skill1Action = playerInput.actions["Skill1"];
        skill2Action = playerInput.actions["Skill2"];
        skill3Action = playerInput.actions["Skill3"];
        interactionAction = playerInput.actions["Interaction"];
        dashAction = playerInput.actions["Dash"];

        EnableAction();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        DisableAction();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnableAction();
        playerInput.DeactivateInput();
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap("GamePlay");
    }

    public static void EnableAction()
    {
        if(verticalAction != null) verticalAction.Enable();
        if (horizontalAction != null) horizontalAction.Enable();
        if (attackAction != null) attackAction.Enable();
        if (skill1Action != null) skill1Action.Enable();
        if (skill2Action != null) skill2Action.Enable();
        if (skill3Action != null) skill3Action.Enable();
        if (interactionAction != null) interactionAction.Enable();
        if (dashAction != null) dashAction.Enable();
    }

    public static void DisableAction()
    {
        if (verticalAction != null) verticalAction.Disable();
        if (horizontalAction != null) horizontalAction.Disable();
        if (attackAction != null) attackAction.Disable();
        if (skill1Action != null) skill1Action.Disable();
        if (skill2Action != null) skill2Action.Disable();
        if (skill3Action != null) skill3Action.Disable();
        if (interactionAction != null) interactionAction.Disable();
        if (dashAction != null) dashAction.Disable();
    }

    void Update()
    {
        float temp1 = verticalAction.ReadValue<float>();
        float temp2 = horizontalAction.ReadValue<float>();
        
        if (temp1 == 0)   //상,하방향값이 0일 경우, 
        {
            verticalValue = 0f;
            v_timer = 0f;  //타이머 초기화
        }
        else
        {
            v_timer += Time.deltaTime / smoothTime;
            float value = (1f - v_timer) * 0 + v_timer * temp1;  //보간
            verticalValue = Mathf.Abs(value) > 1? value / Mathf.Abs(value): value;
        }
        
        
        if (temp2 == 0)  //좌,우방향값이 0일 경우
        {
            horizontalValue = 0f;
            h_timer = 0f;
        }
        else
        {
            h_timer += Time.deltaTime / smoothTime;
            float value = (1f - h_timer) * 0 + h_timer * temp2; //보간
            horizontalValue = Mathf.Abs(value) > 1? value / Mathf.Abs(value) : value;
        }
    }
}
