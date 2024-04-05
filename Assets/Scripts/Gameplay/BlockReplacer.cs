using System.Threading;
using Cysharp.Threading.Tasks;
using HDH.UnityExt.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public class BlockReplacer
    {
        private readonly Camera _camera;
        private readonly float _replacementDepth;
        private BlockView _currentBlock;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _targetScale;
        private Vector3 _currentPosVelocity;
        private Quaternion _currentRotationVelocity;
        private Vector3 _currentScaleVelocity;
        private CancellationTokenSource _syncCtSource;
        private bool _isCurrentBlockInSlot;
        private BlockSlot _currentSlot;

        public BlockReplacer(Camera camera, float replacementDepth)
        {
            _camera = camera;
            _replacementDepth = replacementDepth;
        }
        
        public bool TryBeginReplacement(PointerEventData eventData, BlockView block)
        {
            if (_currentBlock.IsNotNull()) return false;
            BeginReplacement(eventData, block);
            return true;
        }

        private void BeginReplacement(PointerEventData eventData, BlockView block)
        {
            _currentBlock = block;
            _currentBlock.gameObject.SetActive(true);
            
            CalculateTargetTransform(eventData);
            _currentBlock.Transform.position = _targetPosition;
            _currentBlock.Transform.rotation = _targetRotation;
            _currentBlock.Transform.localScale = _targetScale;
            _syncCtSource = new CancellationTokenSource();
            SyncPosWithTargetPos().Forget();
        }

        private void OnReplace(PointerEventData eventData)
        {
            _isCurrentBlockInSlot = false;
            CalculateTargetTransform(eventData);
            
            if (RayCastSlot(out BlockSlot slot) == false)
                return;

            _targetPosition = slot.Transform.position;
            _targetRotation = slot.Transform.rotation;
            _targetScale = slot.Transform.localScale;
            _currentSlot = slot;
            _isCurrentBlockInSlot = true;
        }

        private void FinishReplacement(PointerEventData eventData)
        {
            _syncCtSource?.Cancel();
            if (_isCurrentBlockInSlot)
            {
                _currentSlot.SetBlock(_currentBlock);
            }
            else
                _currentBlock.Slot.FitCurrentBlockInside();

            _currentBlock = null;
        }

        private bool RayCastSlot(out BlockSlot slot)
        {
            slot = null;
            var camPosition = _camera.transform.position;
            return Physics.Raycast(new Ray(camPosition, _targetPosition - camPosition),
                out var hit, 100) && hit.transform.TryGetComponent(out slot) && slot.IsAvailableForReceive;
        }

        private void CalculateTargetTransform(PointerEventData eventData)
        {
            _targetPosition = GetWorldPos(eventData);
            _targetRotation = Quaternion.LookRotation(-_camera.transform.forward, Vector3.up);
            _targetScale = Vector3.one;
        }

        private Vector3 GetWorldPos(PointerEventData eventData) =>
            _camera.ScreenToWorldPoint(eventData.position.To3(missingValue: _replacementDepth));

        private async UniTaskVoid SyncPosWithTargetPos()
        {
            while (_syncCtSource.IsCancellationRequested == false)
            {
                _currentBlock.Transform.position = Vector3.SmoothDamp(_currentBlock.Transform.position, 
                    _targetPosition, ref _currentPosVelocity, 0.1f);
                _currentBlock.Transform.rotation = QuaternionExtensions.SmoothDamp(_currentBlock.Transform.rotation,
                    _targetRotation, ref _currentRotationVelocity, 0.1f);
                _currentBlock.Transform.localScale = Vector3.SmoothDamp(_currentBlock.Transform.localScale, _targetScale,
                    ref _currentScaleVelocity, 0.1f);
                
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}