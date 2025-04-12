using UnityEngine;

namespace SpellSystem
{
    public class FreeCameraMouseLook : MonoBehaviour
    {
        [Header("Настройки вращения")]
        [SerializeField] private float _mouseSensitivity = 100f;
        [Tooltip("Чувствительность мыши")]
        
        [Header("Ограничения угла")]
        [SerializeField] private float _minVerticalAngle = -90f;
        [Tooltip("Минимальный угол наклона вниз")]
        [SerializeField] private float _maxVerticalAngle = 90f;
        [Tooltip("Максимальный угол наклона вверх")]
        
        private float _xRotation = 0f;
        private float _yRotation = 0f;

        void Start()
        {
            // Инициализируем текущие углы поворота
            Vector3 currentRotation = transform.localEulerAngles;
            _xRotation = currentRotation.x;
            _yRotation = currentRotation.y;
        }

        void Update()
        {
            // Получаем ввод мыши
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

            // Изменяем углы поворота
            _yRotation += mouseX;  // Горизонтальное вращение
            _xRotation -= mouseY;  // Вертикальное вращение
            
            // Ограничиваем вертикальный угол
            _xRotation = Mathf.Clamp(_xRotation, _minVerticalAngle, _maxVerticalAngle);
            
            // Применяем вращение
            transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }
    }
}