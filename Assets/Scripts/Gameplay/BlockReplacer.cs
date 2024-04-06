using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Blocks;
using HDH.UnityExt.Extensions;
using Infrastructure.SimpleInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public class BlockReplacer
    {
        private readonly Camera _camera;
        private readonly InputService _inputService;
        private readonly float _replacementDepth;
        private readonly Action<Vector2Int, Vector2Int> _performMovement;
        private BlockView _currentBlock;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _currentPosVelocity;
        private Quaternion _currentRotationVelocity;
        private Vector3 _currentScaleVelocity;
        private CancellationTokenSource _syncCtSource;
        private bool _isCurrentBlockInSlot;
        private BlockSlot _currentSlot;

        public BlockReplacer(Camera camera, InputService inputService, float replacementDepth, Action<Vector2Int, Vector2Int> performMovement)
        {
            _camera = camera;
            _inputService = inputService;
            _replacementDepth = replacementDepth;
            _performMovement = performMovement;
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
            
            CalculateTargetTransform(eventData.position);
            _currentBlock.Transform.position = _targetPosition;
            // _currentBlock.Transform.rotation = _targetRotation;
            _syncCtSource = new CancellationTokenSource();
            _inputService.SimpleDrag += OnReplace;
            _inputService.SimpleDragFinished += FinishReplacement;
            SyncPosWithTargetPos().Forget();
        }

        private void OnReplace(Vector2 delta, Vector2 screenPosition)
        {
            _isCurrentBlockInSlot = false;
            CalculateTargetTransform(screenPosition);
            
            if (RayCastSlot(out BlockSlot slot, out _) == false)
                return;

            _targetPosition = slot.WorldPosition;

            // _targetRotation = slot.Rotation;
            // _targetScale = slot.Transform.localScale;
            _currentSlot = slot;
            _isCurrentBlockInSlot = true;
        }

        private void FinishReplacement(Vector2 screenPosition)
        {
            _syncCtSource?.Cancel();
            if (_isCurrentBlockInSlot && _currentSlot != _currentBlock.Slot)
            {
                Vector2Int oldBlockPos = _currentBlock.Slot.Position;
                // _currentSlot.SetBlock(_currentBlock, false);
                _performMovement.Invoke(oldBlockPos, _currentSlot.Position);   
            }
            else
                _currentBlock.Slot.FitCurrentBlockInside(true);

            _currentBlock = null;
            _inputService.SimpleDrag -= OnReplace;
            _inputService.SimpleDragFinished -= FinishReplacement;
        }

        private bool RayCastSlot(out BlockSlot slot, out RaycastHit hit)
        {
            slot = null;
            var camPosition = _camera.transform.position;
            return Physics.Raycast(new Ray(camPosition, _targetPosition - camPosition),
                out hit, 100, LayerMask.GetMask("SlotsLayer")) && hit.transform.TryGetComponent(out slot) && slot.IsAvailableForReceive;
        }

        private void CalculateTargetTransform(Vector2 screenPosition)
        {
            _targetPosition = GetWorldPos(screenPosition);
            // _targetRotation = Quaternion.LookRotation(-_camera.transform.forward, Vector3.up);
        }

        private Vector3 GetWorldPos(Vector2 screenPosition) =>
            _camera.ScreenToWorldPoint(screenPosition.To3(missingValue: _replacementDepth));

        private async UniTaskVoid SyncPosWithTargetPos()
        {
            while (_syncCtSource.IsCancellationRequested == false)
            {
                _currentBlock.Transform.position = Vector3.SmoothDamp(_currentBlock.Transform.position, 
                    _targetPosition, ref _currentPosVelocity, 0.1f);
                // _currentBlock.Transform.rotation = QuaternionExtensions.SmoothDamp(_currentBlock.Transform.rotation,
                //     _targetRotation, ref _currentRotationVelocity, 0.1f);
                
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}