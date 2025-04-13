using TransmigrationSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Common
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private float _maxInteractDistance = 5f;
        [SerializeField] private LayerMask _interactiveLayer;

        [SerializeField] private InteractiveObject _currentObject;

        [SerializeField] private Image _centerDot;
        [SerializeField] private Color _highlightColor = Color.green;
        
        private Color _originalDotColor;

        public InteractiveObject CurrentObject => _currentObject;
        
        
        private void Start()
        {
            _originalDotColor = _centerDot.color;
            
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main;
            }
        }

        private void Update()
        {
            CheckForInteractiveObjects();
        }

        private void CheckForInteractiveObjects()
        {
            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Центр экрана
            RaycastHit hit;
    
            if (Physics.Raycast(ray, out hit, _maxInteractDistance, _interactiveLayer))
            {
                InteractiveObject interactive = hit.collider.GetComponent<InteractiveObject>();
                if (interactive != null)
                {
                    _centerDot.color = _highlightColor;
                    _currentObject = interactive;
                }
                else
                {
                    ResetDotColor();
                }
            }
            else
            {
                ResetDotColor();
            }
        }

        private void ResetDotColor()
        {
            _centerDot.color = _originalDotColor;
            _currentObject = null;
        }
    }
}