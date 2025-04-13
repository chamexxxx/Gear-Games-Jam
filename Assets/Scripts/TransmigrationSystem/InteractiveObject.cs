using System.Collections.Generic;
using UnityEngine;

namespace TransmigrationSystem
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private float _maxStudyDistance = 5f;
    
        [SerializeField] private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        [SerializeField] private InteractiveObjectMovement _physicsMovementController;
        
        private bool _active = false;
        public bool Active => _active;

        private void Start()
        {
            if (_rigidbodies == null || _rigidbodies.Count == 0)
            {
                _rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>(true));
            }
            
            Activate(false);
        }

        public void Activate(bool activate)
        {
            _active = activate;
            
            foreach (var rb in _rigidbodies)
            {
                if (rb != null)
                {
                    rb.isKinematic = !activate;
                }
            }
            
            _physicsMovementController.enabled = activate;
        }
    }
}