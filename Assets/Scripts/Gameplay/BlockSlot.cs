using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Blocks;
using Infrastructure.SoundsLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class BlockSlot : MonoBehaviour
    {
        private static readonly int Arc2 = Shader.PropertyToID("_Arc2");
        [SerializeField] private Bounds _bounds;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public bool IsAvailableForReceive => _isContainBlock == false;
        public Bounds Bounds => _bounds;
        public Vector2Int Position => _position;

        private SoundsService _sounds;
        private BlockView _currentBlock;
        private Transform _transform;
        private Tween _highlightingTween;
        private Vector2Int _position;
        private bool _isContainBlock;
        private Vector3 _initialSpriteRendererScale;

        [Inject]
        private void Inject(SoundsService sounds) =>
            _sounds = sounds;
        
        public void Setup(Vector2Int position, Vector3 worldPosition)
        {
            gameObject.SetActive(true);
            _position = position;
            transform.position = worldPosition;
        }

        public UniTask SetBlock(BlockView block, bool animated)
        {
            if (_isContainBlock)
                throw new Exception();
            _isContainBlock = true;
            _currentBlock = block;
            block.SetSlot(this);
            return FitCurrentBlockInside(animated);
        }

        public BlockView RemoveCurrentBlock()
        {
            BlockView block = _currentBlock;
            _currentBlock = null;
            _isContainBlock = false;
            return block;
        }

        public UniTask FitCurrentBlockInside(bool animated)
        {
            return _currentBlock.MoveTo(_transform.position, animated);
        }

        public UniTask DropContent()
        {
            return _currentBlock.Drop();
        }
        
        public BlockView ResetSlot()
        {
            BlockView block = _currentBlock;
            _currentBlock = null;
            _isContainBlock = false;
            SetHighlighting(false, false);
            return block;
        }

        public void SetHighlighting(bool isVisible, bool animated = true, float delay = 0f)
        {
            float targetValue = isVisible ? 0 : 360;
            _highlightingTween?.Kill();
            if (_spriteRenderer.material.GetFloat(Arc2) >= 359)
                RandomizeSpriteRendererTransform();
            
            if (animated)
            {
                _highlightingTween = DOTween.To(
                    () => _spriteRenderer.material.GetFloat(Arc2),
                    value => _spriteRenderer.material.SetFloat(Arc2, value),
                    targetValue,
                    Random.Range(0.15f, 0.25f))
                    .SetEase(Ease.InSine)
                    .SetDelay(delay)
                    .OnStart(isVisible ? () => _sounds.RaiseEvent(EventId.SlotHighlightingStarted) : null);
            }
            else
            {
                _spriteRenderer.material.SetFloat(Arc2, targetValue);
            }
        }

        private void Awake()
        {
            _transform = transform;
            _initialSpriteRendererScale = _spriteRenderer.transform.localScale;
            SetHighlighting(false, false);
        }

        private void RandomizeSpriteRendererTransform()
        {
            Transform spriteRendererTransform = _spriteRenderer.transform;
            spriteRendererTransform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
            spriteRendererTransform.localPosition = Random.insideUnitCircle * 0.1f;
            spriteRendererTransform.localScale = _initialSpriteRendererScale * Random.Range(0.8f, 1.1f);
        }

        private void OnDisable()
        {
            ResetSlot();
        }

        [Button]
        private void GetBoundsFromCollider()
        {
            _bounds = GetComponent<BoxCollider>().bounds;
        }
    }
}