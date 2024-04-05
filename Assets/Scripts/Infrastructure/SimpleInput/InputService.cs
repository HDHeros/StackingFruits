using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lean.Touch;
using UnityEngine;

namespace Infrastructure.SimpleInput
{
    public class InputService : IDisposable
    {
        private readonly CancellationTokenSource _ctSource;
        private LeanFingerFilter _filter;
        public event Action<float> Pinch;
        /// <summary>
        /// Delta, position
        /// </summary>
        public event Action<Vector2, Vector2> SimpleDrag;
        public event Action<Vector2> SimpleDragFinished;
        public bool IsTouchDetected { get; private set; }
        private Vector2 _lastDragDelta;
        private float _lastPinchRatio;

        public InputService()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
            _ctSource = new CancellationTokenSource();
            ObserveEvents(_ctSource.Token).Forget();
        }

        public void Dispose() => 
            _ctSource?.Dispose();

        private async UniTaskVoid ObserveEvents(CancellationToken token)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);
            _filter = new LeanFingerFilter(true);

            while (token.IsCancellationRequested == false)
            {
                List<LeanFinger> fingers = _filter.UpdateAndGetFingers();
                IsTouchDetected = fingers.Count > 0;
                HandlePinch(fingers);
                HandleDrag(fingers);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        private void HandleDrag(List<LeanFinger> fingers)
        {
            Vector2 lastScreenPoint = LeanGesture.GetLastScreenCenter(fingers);
            Vector2 screenPoint     = LeanGesture.GetScreenCenter(fingers);
            Vector2 delta = screenPoint - lastScreenPoint;
            if (fingers.Count > 1) 
                delta = Vector2.zero;
            
            if (delta != Vector2.zero)
            {
                SimpleDrag?.Invoke(delta, screenPoint);
                _lastDragDelta = delta;
                return;
            }

            if (_lastDragDelta != Vector2.zero && delta == Vector2.zero && fingers.Count == 0)
            {
                SimpleDragFinished?.Invoke(lastScreenPoint);
                _lastDragDelta = Vector2.zero;
            }
                
        }

        private void HandlePinch(List<LeanFinger> fingers)
        {
            var pinchRatio = LeanGesture.GetPinchRatio(fingers, -0.2f) - 1;
            if (pinchRatio != 0)
            {
                Pinch?.Invoke(pinchRatio);
                _lastPinchRatio = pinchRatio;
                return;
            }

            if (_lastPinchRatio != 0 && pinchRatio == 0 && fingers.Count == 0)
            {
                OnPinchFinished();
                _lastPinchRatio = 0;
            }
        }

        private void OnPinchFinished()
        {
        }

        private void OnFingerDown(LeanFinger obj)
        {
        }
    }
}