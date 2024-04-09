using System;
using Gameplay.Blocks;
using Gameplay.GameCore;
using GameStructConfigs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class LevelPreview : MonoBehaviour, IPointerClickHandler
    {
        public event Action<LevelPreview> OnClick;
        [SerializeField] private BoxCollider _collider;
        private LevelConfig _levelConfig;
        public LevelData<BlockView> LevelData => _levelConfig.GetLevelData();

        public void Initialize(LevelConfig levelConfig, Bounds bounds)
        {
            _levelConfig = levelConfig;
            transform.localPosition = bounds.center;
            transform.localScale *= FitInto(bounds);
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