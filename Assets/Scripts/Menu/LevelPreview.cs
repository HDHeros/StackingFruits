using System;
using DG.Tweening;
using Gameplay.Blocks;
using Gameplay.GameCore;
using Gameplay.LevelsLogic;
using GameStructConfigs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class LevelPreview : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<LevelPreview> OnClick;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private MeshRenderer _modelRenderer;
        [SerializeField] private Transform _model;
        [SerializeField] private float _onMouseHoverOffset;
        public LevelData<BlockView> LevelData => _levelConfig.GetLevelData();
        public string Id => _levelConfig.Id;
        private LevelConfig _levelConfig;
        private bool _isSelectionEnabled;
        private Tween _materialColorTween;

        public void Initialize(LevelsService.LevelModel levelModel, Bounds bounds)
        {
            _levelConfig = levelModel.Config;
            transform.localPosition = bounds.center;
            transform.localScale *= FitInto(bounds);
            SetLevelProgress(levelModel.Progress, false);
        }

        public void SetLevelProgress(float progress, bool animated = true)
        {
            Color targetColor = progress < 1 ? Color.black : Color.white;
            if (animated)
            {
                _materialColorTween?.Kill();
                _materialColorTween = _modelRenderer.material.DOColor(targetColor, 0.5f).SetEase(Ease.OutQuart);
            }
            else
            {
                _modelRenderer.material.color = targetColor;
            }
        }
        
        private float FitInto(Bounds bounds)
        {
            Vector3 cellSize = bounds.size;

            var selfBounds = _collider.bounds;
            float xFactor = cellSize.x / selfBounds.size.x;
            float yFactor = cellSize.y / selfBounds.size.y;
            float zFactor = cellSize.z / selfBounds.size.z;

            float minFactor = Mathf.Min(xFactor, Mathf.Min(yFactor, zFactor));
            return minFactor;
        }

        public void OnPointerClick(PointerEventData eventData) => 
            OnClick?.Invoke(this);

        public void EnableSelection()
        {
            _isSelectionEnabled = true;
        }

        public void DisableSelection()
        {
            _isSelectionEnabled = false; 
            ResetOnHoverAnimation();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelectionEnabled == false) return;
            _model.DOKill();
            _model.DOLocalMove(_model.InverseTransformDirection(Vector3.back) * _onMouseHoverOffset, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelectionEnabled == false) return;
            _model.DOKill();
            _model.DOLocalMove(Vector3.zero * _onMouseHoverOffset, 0.2f);
        }

        private void ResetOnHoverAnimation()
        {
            _model.DOKill();
            _model.localPosition = Vector3.zero;
        }
    }
}