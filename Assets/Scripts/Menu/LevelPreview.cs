using System;
using Gameplay.Blocks;
using Gameplay.GameCore;
using Gameplay.LevelsLogic;
using GameStructConfigs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class LevelPreview : MonoBehaviour, IPointerClickHandler
    {
        public event Action<LevelPreview> OnClick;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private MeshRenderer _modelRenderer;
        private LevelConfig _levelConfig;
        public LevelData<BlockView> LevelData => _levelConfig.GetLevelData();

        public void Initialize(LevelsService.LevelModel levelModel, Bounds bounds)
        {
            _levelConfig = levelModel.Config;
            transform.localPosition = bounds.center;
            transform.localScale *= FitInto(bounds);
            SetCompletedView(levelModel.IsCompleted);
        }

        public void SetCompletedView(bool isCompleted)
        {
            _modelRenderer.material.color = isCompleted ? Color.white : Color.black;
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
    }
}