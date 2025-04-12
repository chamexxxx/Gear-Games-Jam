using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class CursorController : MonoBehaviour
    {
        public static CursorController Instance  { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Фиксируем курсор в центре экрана
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Для временного отключения управления
        public void SetLookEnabled(bool enabled)
        {
            enabled = enabled;
            Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enabled;
        }
    }
}