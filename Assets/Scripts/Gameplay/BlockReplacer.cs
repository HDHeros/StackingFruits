using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Blocks;
using Gameplay.GameCore;
using HDH.UnityExt.Extensions;
using Infrastructure.SimpleInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public class BlockReplacer
    {
        public bool IsReplacementLocked { get; set; }
        public bool IsCurrentlyReplace { get; private set; }
        
        private readonly Camera _camera;
        private readonly InputService _inputService;
        private readonly float _replacementDepth;
        private readonly Action<Vector2Int, Vector2Int> _performMovement;
        private readonly SlotsHighlighter _slotsHighlighter;
        private readonly StackingGame<BlockView> _game;
        private BlockView _currentBlock;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _currentPosVelocity;
        private Quaternion _currentRotationVelocity;
        private Vector3 _currentScaleVelocity;
        private CancellationTokenSource _syncCtSource;
        private bool _isCurrentBlockInSlot;
        private BlockSlot _currentSlot;


        public BlockReplacer(Camera camera, InputService inputService, float replacementDepth,
            Action<Vector2Int, Vector2Int> performMovement, SlotsHighlighter slotsHighlighter)
        {
            _camera = camera;
            _inputService = inputService;
            _replacementDepth = replacementDepth;
            _performMovement = performMovement;
            _slotsHighlighter = slotsHighlighter;
        }

        public void ForceFinishReplacement()
        {
            if (_currentBlock.IsNotNull())
                FinishReplacement(false);
        }

        public void BeginReplacement(PointerEventData eventData, BlockView block)
        {
            if (_currentBlock.IsNotNull() || IsReplacementLocked) return;

            _slotsHighlighter.OnReplacementBegin(block);
            IsCurrentlyReplace = true;
            _currentBlock = block;
            _currentBlock.gameObject.SetActive(true);
            
            CalculateTargetTransform(eventData.position);
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
            
            _currentSlot = slot;
            _isCurrentBlockInSlot = true;
        }

        private void FinishReplacement(Vector2 screenPosition) => 
            FinishReplacement(true);

        private void FinishReplacement(bool replaceToNewPosIfExist)
        {
            _syncCtSource?.Cancel();
            if (_isCurrentBlockInSlot && _currentSlot != _currentBlock.Slot && replaceToNewPosIfExist)
            {
                Vector2Int oldBlockPos = _currentBlock.Slot.Position;
                _performMovement.Invoke(oldBlockPos, _currentSlot.Position);   
            }
            else
                _currentBlock.Slot.FitCurrentBlockInside(true);

            _slotsHighlighter.OnReplacementFinished();
            IsCurrentlyReplace = false;
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
        }

        private Vector3 GetWorldPos(Vector2 screenPosition) =>
            _camera.ScreenToWorldPoint(screenPosition.To3(missingValue: _replacementDepth));

        private async UniTaskVoid SyncPosWithTargetPos()
        {
            while (_syncCtSource.IsCancellationRequested == false)
            {
                _currentBlock.Transform.position = Vector3.SmoothDamp(_currentBlock.Transform.position, 
                    _targetPosition, ref _currentPosVelocity, 0.1f);
                
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}