using System;
using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;

    public float runAcceleration = 0.25f;
    public float runSpeed = 4f;
    public float drag = 0.1f;

    [Header("Camera settings")] 
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;

    private PlayerInput _playerInput;
    private Vector2 _cameraRotation = Vector2.zero;
    private Vector2 _playerTargetRotation = Vector2.zero;

    private void Start()
    {
        var playerSwitch = FindAnyObjectByType<SwitchManager>();
        if (playerSwitch is null)
        {
            Debug.LogError("No SwitchManager registered");
        }
        else
        {
            _playerInput = playerSwitch.GetPlayerInput();
        }
    }

    private void Update()
    {
        Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z)
            .normalized;
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z)
            .normalized;
        Vector3 movementDirection = cameraRightXZ * _playerInput.movementInput.x +
                                    cameraForwardXZ * _playerInput.movementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        _characterController.Move(newVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        _cameraRotation.x += lookSenseH * _playerInput.lookInput.x;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerInput.lookInput.y, -lookLimitV,
            lookLimitV);

        _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerInput.lookInput.x;
        transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
        
        _playerCamera.transform.rotation = quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
    }
    
}
