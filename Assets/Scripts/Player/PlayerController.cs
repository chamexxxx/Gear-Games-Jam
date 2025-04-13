using Common;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public CharacterController CharacterController;
        private Camera _playerCamera;

        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;
        public float gravity = -9.81f;
        public float targetHeight = 2.0f; // Желаемая высота над поверхностью
        public float heightCorrectionForce = 20.0f; // Сила коррекции высоты
        public float heightCorrectionDamping = 4f; // Демпфирование коррекции высоты

        [Header("Camera settings")] 
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerInput _playerInput;
        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;
        private Vector3 _verticalVelocity = Vector3.zero;
        private float _currentHeightAdjustment = 0f;

        private void Awake()
        {
            _playerCamera = FindAnyObjectByType<Camera>();
        }

        private void Start()
        {
            var gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager is null)
            {
                Debug.LogError("No SwitchManager registered");
            }
            else
            {
                _playerInput = gameManager.PlayerInput;
            }
        }

        private void Update()
        {
            // Обработка движения
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z)
                .normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z)
                .normalized;
            Vector3 movementDirection = cameraRightXZ * _playerInput.movementInput.x +
                                      cameraForwardXZ * _playerInput.movementInput.y;

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = new Vector3(CharacterController.velocity.x, 0f, CharacterController.velocity.z) + movementDelta;

            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

            // Проверка расстояния до земли
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                // Вычисляем разницу между текущей высотой и желаемой
                float heightDifference = targetHeight - hit.distance;
                
                // Вычисляем силу коррекции высоты (с демпфированием)
                float correctionForce = heightDifference * heightCorrectionForce - _verticalVelocity.y * heightCorrectionDamping;
                
                // Применяем силу коррекции
                _verticalVelocity.y += correctionForce * Time.deltaTime;
            }

            // Применение гравитации
            _verticalVelocity.y += gravity * Time.deltaTime;

            // Комбинированное движение
            CharacterController.Move((newVelocity + new Vector3(0, _verticalVelocity.y, 0)) * Time.deltaTime);
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
}