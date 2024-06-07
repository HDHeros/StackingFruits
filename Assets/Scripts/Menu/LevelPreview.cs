using System;
using DG.Tweening;
using Gameplay.GameCore;
using Gameplay.LevelsLogic;
using GameStructConfigs;
using HDH.UnityExt.Extensions;
using Infrastructure.SoundsLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Menu
{
    public class LevelPreview : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<LevelPreview> OnClick;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private MeshRenderer _modelRenderer;
        [SerializeField] private Transform _model;
        [SerializeField] private float _onMouseHoverOffset;
        public LevelData LevelData => _levelConfig.GetLevelData();
        public string Id => _levelConfig.Id;
        private LevelConfig _levelConfig;
        private bool _isSelectionEnabled;
        private Tween _materialColorTween;
        private SoundsService _sounds;

        public void Initialize(LevelsService.LevelModel levelModel, Bounds bounds, SoundsService sounds)
        {
            _sounds = sounds;
            _levelConfig = levelModel.Config;
            transform.localPosition = bounds.center + (Random.insideUnitCircle * 0.05f).To3();
            transform.localScale *= FitInto(bounds) * Random.Range(0.9f, 1f);
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            SetLevelProgress(levelModel.Progress, false);
            _collider.enabled = false;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            _sounds.RaiseEvent(EventId.FruitClicked);
            OnClick?.Invoke(this);
        }

        public void EnableSelection()
        {
            _isSelectionEnabled = true;
            _collider.enabled = true;
        }

        public void DisableSelection()
        {
            _isSelectionEnabled = false; 
            _collider.enabled = false;
            // SetSelected(false);
            ResetOnHoverAnimation();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelectionEnabled == false) return;
            _sounds.RaiseEvent(EventId.FruitSelected);
            _model.DOKill();
            _model.DOLocalMove(_model.InverseTransformDirection(Vector3.back) * _onMouseHoverOffset, 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelectionEnabled == false) return;
            _sounds.RaiseEvent(EventId.FruitUnselected);
            _model.DOKill();
            _model.DOLocalMove(Vector3.zero * _onMouseHoverOffset, 0.2f);
        }

        public void SetSelected(bool isSelected)
        {
            _modelRenderer.material.SetFloat("_OutlineWidth", isSelected ? 10f : 3f);
            _modelRenderer.material.SetColor("_OutlineColor", isSelected ? Color.white : Color.black);
        }
        
        private void ResetOnHoverAnimation()
        {
            _model.DOKill();
            _model.localPosition = Vector3.zero;
        }
    }
}