using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    private PlayerInputActions inputActions;
    public event EventHandler OnJumpPerformed;
    public event EventHandler OnInteractPerformed;
    public event EventHandler<bool> OnRunToggled;
    public event EventHandler OnSkill1Performed;
    public event EventHandler OnSkill2Performed;

    private void Awake()
    {
        Time.timeScale = 1;

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Camera.Enable();
        
        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Interact.performed += Interact_performed;
        inputActions.Player.Run.performed += Run_performed;
        inputActions.Player.Run.canceled += Run_canceled;
        inputActions.Player.Skill1.performed += Skill1_performed;
        inputActions.Player.Skill2.performed += Skill2_performed;
    }

    private void OnDestroy()
    {
        inputActions.Player.Jump.performed -= Jump_performed;
        inputActions.Player.Interact.performed -= Interact_performed;
        inputActions.Player.Run.performed -= Run_performed;
        inputActions.Player.Run.canceled -= Run_canceled;
        inputActions.Player.Skill1.performed -= Skill1_performed;
        inputActions.Player.Skill2.performed -= Skill2_performed;

        inputActions.Dispose();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Run_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunToggled?.Invoke(this, true);
    }

    private void Run_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunToggled?.Invoke(this, false);
    }

    private void Skill1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSkill1Performed?.Invoke(this, EventArgs.Empty);
    }

    private void Skill2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSkill2Performed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();
        moveVector = moveVector.normalized;
        return moveVector;
    }

    public Vector2 GetCameraRotateNormalized()
    {
        Vector2 cameraVector = inputActions.Camera.Rotate.ReadValue<Vector2>();
        cameraVector = cameraVector.normalized;
        return cameraVector;
    }

    public Vector2 GetMouseScroll()
    {
        // TODO: Get scrollUp and scrollDown, ReadValue<float> may help;
        // Retuen a new Vector2 type
        throw new NotImplementedException();
    }

    public bool IsRunning()
    {
        return inputActions.Player.Run.IsPressed();
    }

}
