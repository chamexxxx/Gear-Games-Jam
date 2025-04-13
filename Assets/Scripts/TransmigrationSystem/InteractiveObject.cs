using System;
using UnityEngine;

namespace TransmigrationSystem
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private float _maxStudyDistance = 5f;
    
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private InteractiveObjectMovement _physicsMovementController;
        
        private bool _active = false;
        public bool Active => _active;

        private void Start()
        {
            Activate(false);
        }

        public void Activate(bool activate)
        {
            _active = activate;
            _rigidbody.isKinematic = !activate;
            _physicsMovementController.enabled = activate;
        }
        
    }
}
