using System;
using System.Threading;
using Player;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;

namespace TransmigrationSystem
{
    [Serializable]
    public class PlayerTransmigrationData
    {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private Ease _moveEase = Ease.InOutQuad;

        [SerializeField] private GameObject _activeObject;
        [SerializeField] private bool _isInPlayer;
        
        [SerializeField] private Transform _currentTransform;
        private CinemachineCamera _cnCamera;
        
        private CancellationTokenSource _cts;
        private Tween _currentTween;

        public PlayerController PlayerController => _playerController;
        public Transform CurrentTransform => _currentTransform;
        
        public GameObject ActiveObject
        {
            get => _activeObject;
            set
            {
                if (value == _activeObject || value == null) return;
                
                CancelCurrentOperation();
                _activeObject = value;
                _activeObject.GetComponent<InteractiveObject>().Activate(true);
                _ = HandleTransmigrationAsync();
            }
        }

        public void InitialiseTransmigration(Transform initialTransform, CinemachineCamera cnCamera)
        {
            _currentTransform = initialTransform;
            _cnCamera = cnCamera;
        }

        private void CancelCurrentOperation()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            _currentTween?.Kill();
            _currentTween = null;
        }

        private async UniTaskVoid HandleTransmigrationAsync()
        {
            try
            {
                if (_activeObject && _playerController)
                {
                    
                    if (_activeObject.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.isKinematic = false;
                    }

                    // Перемещаем игрока к объекту
                    _currentTween = _playerController.transform.DOMove(
                        _activeObject.transform.position, 
                        _moveDuration
                    )
                    .SetEase(_moveEase)
                    .OnKill(() =>
                    {
                        _currentTween = null;
                        _playerController.gameObject.SetActive(false);
                    })
                    .OnComplete(() =>
                    {
                        _playerController.gameObject.SetActive(false);
                        _currentTransform = _activeObject.transform;
                        _cnCamera.Target.TrackingTarget = _currentTransform;
                    });
                    
                    await _currentTween
                        .AsyncWaitForCompletion()
                        .AsUniTask()
                        .AttachExternalCancellation(_cts.Token);
                    
                }

            }
            catch (OperationCanceledException)
            {
                // Отмена ожидается
            }
            catch (Exception e)
            {
                Debug.LogError($"Transmigration error: {e}");
            }
        }

        public async UniTaskVoid ReturnToCharacterAsync()
        {
            try
            {
                CancelCurrentOperation();
                
                if (!_activeObject || !_playerController) return;

                if (_activeObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                }

                // Мгновенное перемещение игрока к объекту
                _playerController.transform.position = _activeObject.transform.position;
                _playerController.gameObject.SetActive(true);
                
                // Анимация прыжка
                _currentTween = _playerController.transform
                    .DOJump(_activeObject.transform.position, 0.01f, 1, 1f);
                
                await _currentTween
                    .AsyncWaitForCompletion()
                    .AsUniTask()
                    .AttachExternalCancellation(_cts.Token);
                
                _currentTransform = _playerController.gameObject.transform;
                _cnCamera.Target.TrackingTarget = _currentTransform;
                _activeObject.GetComponent<InteractiveObject>().Activate(false);
                _activeObject = null;
            }
            catch (OperationCanceledException)
            {
                // Отмена - нормально
            }
        }

        public void ReturnToCharacter() => ReturnToCharacterAsync().Forget();

        private void OnDestroy()
        {
            CancelCurrentOperation();
            _cts?.Dispose();
        }
    }
}