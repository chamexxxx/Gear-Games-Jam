using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerInput : MonoBehaviour, PlayerControls.IPlayerLocomotionActions
{
    public PlayerControls playerControls;
    public Vector2 movementInput;
    public Vector2 lookInput;

    private void OnEnable()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        
        playerControls.PlayerLocomotion.Enable();
        playerControls.PlayerLocomotion.SetCallbacks(this);
    }

    private void OnDisable()
    {
        playerControls.PlayerLocomotion.Disable();
        playerControls.PlayerLocomotion.RemoveCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
