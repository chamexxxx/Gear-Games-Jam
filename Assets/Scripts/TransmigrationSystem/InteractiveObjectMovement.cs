using UnityEngine;
using Player;

namespace TransmigrationSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractiveObjectMovement : MonoBehaviour
    {
        [Header("Movement Settings")] 
        [SerializeField] private float _moveForce = 50f;
        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _maxSpeed = 5f;

        [Header("References")] 
        [SerializeField] private Transform _groundCheckPoint;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private PlayerInput _playerInput;

        private Rigidbody _rb;
        private bool _isGrounded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
            if (_groundCheckPoint == null)
                _groundCheckPoint = transform;
            
            if (_playerCamera == null)
                _playerCamera = Camera.main;
            
            if (_playerInput == null)
                _playerInput = FindObjectOfType<PlayerInput>();
        }

        private void Update()
        {
            CheckGrounded();
            HandleJump();
        }

        private void CheckGrounded()
        {
            _isGrounded = Physics.Raycast(
                _groundCheckPoint.position,
                Vector3.down,
                _groundCheckDistance,
                _groundLayer
            );
        }

        private void HandleJump()
        {
            if (_isGrounded && _playerInput != null && _playerInput.playerControls.PlayerLocomotion.Extra.triggered)
            {
                _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }

        private void FixedUpdate()
        {
            if (_playerInput == null || _playerCamera == null) return;

            Vector3 movementDirection = CalculateMovementDirection();
            ApplyMovement(movementDirection);
            LimitVelocity();
        }

        private Vector3 CalculateMovementDirection()
        {
            Vector3 cameraForward = _playerCamera.transform.forward;
            Vector3 cameraRight = _playerCamera.transform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            return (cameraForward * _playerInput.movementInput.y + cameraRight * _playerInput.movementInput.x).normalized;
        }

        private void ApplyMovement(Vector3 direction)
        {
            if (direction.magnitude > 0.1f)
            {
                Vector3 force = direction * _moveForce * (_isGrounded ? 1f : 0.5f);
                _rb.AddForce(force);
            }
            else if (_isGrounded)
            {
                ApplyBrakingForce();
            }
        }

        private void ApplyBrakingForce()
        {
            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > 0.1f)
            {
                Vector3 brakingForce = -horizontalVelocity.normalized * _moveForce * 0.5f;
                _rb.AddForce(brakingForce);
            }
        }

        private void LimitVelocity()
        {
            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > _maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * _maxSpeed;
                _rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        private void OnDrawGizmos()
        {
            if (_groundCheckPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    _groundCheckPoint.position,
                    _groundCheckPoint.position + Vector3.down * _groundCheckDistance
                );
            }
        }
    }
}