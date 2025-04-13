using System.Collections.Generic;
using System.Linq;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Common
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private List<PlayerController> _characters;
    
        [SerializeField]
        private PlayerInput _playerInput;
    
        [SerializeField]
        private UIController _uiController;
    
        [SerializeField]
        private int _currentIndex = 0;

        [SerializeField] private CinemachineCamera _cnCamera;
        //public int CurrentIndex => currentIndex;
    
        private void Awake()
        {
            _playerInput.OnSwitchPlayer += SwitchPlayer;
        }

        private void Start()
        {
            if (_characters.Count > 0)
            {
                _cnCamera.Target.TrackingTarget = _characters[0].gameObject.transform;

                foreach (var player in _characters.Skip(1))
                {
                    PlayerOff(player);
                }
            }
        }

        public PlayerInput GetPlayerInput()
        {
            return _playerInput;
        }
    
        private void SwitchPlayer()
        {
            if (_characters.Count < 2) return;
            PlayerOff(_characters[_currentIndex]);
            
            _currentIndex = (_currentIndex + 1) % _characters.Count;
            _cnCamera.Target.TrackingTarget = _characters[_currentIndex].gameObject.transform;
            
            PlayerOn(_characters[_currentIndex]);
            //_uiController.Camera = 
        }
        
        private void PlayerOn(PlayerController playerController)
        {
            playerController.enabled = true;
            playerController.CharacterController.enabled = true;
        }

        private void PlayerOff(PlayerController playerController)
        {
            playerController.enabled = false;
            playerController.CharacterController.enabled = false;
        }
    
        private void OnDestroy()
        {
            if (_playerInput != null)
                _playerInput.OnSwitchPlayer -= SwitchPlayer;
        }
    }
}
