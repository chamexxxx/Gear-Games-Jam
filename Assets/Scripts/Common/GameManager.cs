using System.Collections.Generic;
using System.Linq;
using Player;
using TransmigrationSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Common
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private List<PlayerTransmigrationData> _characters;
    
        [SerializeField]
        private PlayerInput _playerInput;
    
        [SerializeField]
        private UIController _uiController;
    
        [SerializeField]
        private int _currentIndex = 0;

        [SerializeField] private CinemachineCamera _cnCamera;
        
        public PlayerInput PlayerInput => _playerInput;
        //public int CurrentIndex => currentIndex;
    
        private void Awake()
        {
            _playerInput.OnSwitchPlayer += SwitchPlayer;
        }

        private void Start()
        {
            if (_characters.Count > 0)
            {
                _cnCamera.Target.TrackingTarget = _characters[0].PlayerController.gameObject.transform;

                _characters[0].InitialiseTransmigration( _characters[0].PlayerController.transform, _cnCamera);
                foreach (var player in _characters.Skip(1))
                {
                    player.InitialiseTransmigration(player.PlayerController.transform, _cnCamera);
                    PlayerOff(player);
                }
            }
        }
        
        public void HandleInteract()
        {
            if (_uiController.CurrentObject != null)
            {
                if (!_uiController.CurrentObject.Active && _characters[_currentIndex].IsInPlayer)
                {
                    _characters[_currentIndex].ActiveObject = _uiController.CurrentObject.gameObject;
                }
            }
            else
            {
                _characters[_currentIndex].ReturnToCharacter();
            }
        }
    
        private void SwitchPlayer()
        {
            if (_characters.Count < 2) return;
            PlayerOff(_characters[_currentIndex]);
            
            _currentIndex = (_currentIndex + 1) % _characters.Count;
            _cnCamera.Target.TrackingTarget = _characters[_currentIndex].CurrentTransform;
            
            PlayerOn(_characters[_currentIndex]);
            
        }
        
        private void PlayerOn(PlayerTransmigrationData player)
        {
            if (player.CurrentTransform == player.PlayerController.gameObject.transform)
            {
                player.PlayerController.enabled = true;
                player.PlayerController.CharacterController.enabled = true;
            }
            else
            {
                if (player.ActiveObject.TryGetComponent(out MovableObject movement))
                {
                    movement.enabled = true;
                }  
            }
        }

        private void PlayerOff(PlayerTransmigrationData player)
        {
            if (player.CurrentTransform == player.PlayerController.gameObject.transform)
            {
                player.PlayerController.enabled = false;
                player.PlayerController.CharacterController.enabled = false;
            }
            else
            {
                if (player.ActiveObject.TryGetComponent(out MovableObject movement))
                {
                    movement.enabled = false;
                }
            }
        }
    
        private void OnDestroy()
        {
            if (_playerInput != null)
                _playerInput.OnSwitchPlayer -= SwitchPlayer;
        }
    }
}
