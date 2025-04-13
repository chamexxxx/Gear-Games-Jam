using System;
using Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(GameManager))]
    public class PlayerInput : MonoBehaviour, PlayerControls.IPlayerLocomotionActions
    {
        public PlayerControls playerControls;
        public Vector2 movementInput;
        public Vector2 lookInput;
        
        private GameManager _gameManager;
        
        public event Action OnSwitchPlayer;

        private void Start()
        {
            _gameManager = gameObject.GetComponent<GameManager>();
        }

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

        public void OnExtra(InputAction.CallbackContext context)
        {
            OnSwitchPlayer?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            _gameManager.HandleInteract();
        }
    }
}
