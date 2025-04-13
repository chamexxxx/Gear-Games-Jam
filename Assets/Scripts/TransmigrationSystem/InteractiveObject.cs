using UnityEngine;

namespace TransmigrationSystem
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private float _maxStudyDistance = 5f;
    
        [SerializeField] private Rigidbody _rigidbody;
        
        public bool Active = false;


        public void Activate(bool activate)
        {
            Active = activate;
            _rigidbody.isKinematic = !activate;
        }
    }
}
