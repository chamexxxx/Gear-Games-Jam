﻿using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace TransmigrationSystem
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        [SerializeField] [CanBeNull] private MovableObject _physicsMovementController;
        
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

            if (_physicsMovementController != null)
            {
                _physicsMovementController.enabled = activate;
            }
        }
    }
}