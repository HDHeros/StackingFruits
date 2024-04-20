using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.GameCore;
using HDH.GoPool;
using HDH.GoPool.Components;
using HDH.UnityExt.Extensions;
using Infrastructure.SoundsLogic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;

namespace Gameplay.Blocks
{
    public class BlockView : 
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler, 
        IBeginDragHandler, 
        IDragHandler, 
        IEquatable<BlockView>
    {
        [SerializeField] private BlockType _type;
        [SerializeField] private PooledParticle[] _onStackParticles;
        [SerializeField] private ParticleSystem.MinMaxGradient _stackParticleColor;
        [SerializeField] private Color _decalColor;
        [SerializeField] private Transform _model;
        [SerializeField] private bool _isStackable;
        [SerializeField] private bool _isMovable;
        public Transform Transform => _transform;
        public BlockSlot Slot { get; private set; }
        public BlockType Type => _type;
        public Color DecalColor => _decalColor;

        private BlockReplacer _replacer;
        private Transform _transform;
        private Vector3 _initialScale;
        private SoundsService _sounds;

        [Inject]
        private void Inject(SoundsService sounds) => 
            _sounds = sounds;
        
        public void Setup(Vector2Int position, BlockReplacer replacer)
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(position.x, position.y);
            _replacer = replacer;
        }

        public UniTask MoveTo(Vector3 to, bool animated)
        {
            if (animated == false)
            {
                transform.position = to;
                return UniTask.CompletedTask;
            }
            
            return AnimateDrop(to);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isStackable == false) return;
            _replacer.BeginReplacement(eventData, this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void SetSlot(BlockSlot slot)
        {
            if (Slot.IsNotNull())
                Slot.RemoveCurrentBlock();
            Slot = slot;
        }

        public UniTask Drop()
        {
            return _isMovable == false ? UniTask.CompletedTask : AnimateDrop(_transform.position.AddY(-20));
        }

        public void ResetBlock()
        {
            _transform.DOKill();
            _transform.localScale = _initialScale;
            _transform.rotation = Quaternion.identity;
            _model.localScale = Vector3.one;
            _model.DOKill();
            
            Slot = null;
        }

        public async UniTask MoveToStackingAnimationPos(Vector3 position, TimeSpan delay)
        {
            await UniTask.Delay(delay);
            _transform.DOKill();
            Sequence sequence = DOTween.Sequence()
                .Append(_transform.DOMove(position, 0.25f).SetEase(Ease.InSine))
                .Join(_transform.DORotate(Vector3.zero.WithY(Random.Range(90, 180)), 0.25f));
            _sounds.RaiseEvent(EventId.StackFruitFlight);
            await UniTask.WaitWhile(() => sequence.IsActive() && sequence.IsPlaying());
        }

        public async UniTask PlayOnStackAnimation(IGoPool pool)
        {
            _transform.DOKill();
            Sequence sequence = DOTween.Sequence()
                .Append(_transform.DOScale(_initialScale * 0.8f, 0.5f).SetEase(Ease.Linear))
                .Join(_transform.DOShakePosition(0.5f, strength: 0.05f, vibrato: 15, fadeOut: false))
                .Join(_transform.DOShakeRotation(0.5f, strength: 15f, vibrato: 15, fadeOut: false));
            await UniTask.WaitWhile(() => sequence.IsActive() && sequence.IsPlaying());
            SpawnStackingParticle(pool);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_replacer.IsCurrentlyReplace || _isStackable == false) return;
            _model.DOKill();
            _model.DOScale(Vector3.one * 1.1f, 0.25f).SetEase(Ease.OutBack);
                _sounds.RaiseEvent(EventId.FruitSelected);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isStackable == false) return;
            _model.DOKill();
            _model.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuint);
        }

        public bool Equals(BlockView other)
        {
            return other != null && other.Type == _type;
        }
        
        private UniTask AnimateDrop(Vector3 position)
        {
            Tween tween = transform.DOMove(position, 20f).SetSpeedBased(true).SetEase(Ease.InSine);
            return UniTask.WaitWhile(() => tween.IsActive() && tween.IsPlaying());
        }

        private void Awake()
        {
            _transform = transform;
            _initialScale = _transform.localScale;
        }

        private void OnDisable()
        {
            ResetBlock();
        }

        private void SpawnStackingParticle(IGoPool pool)
        {
            PooledParticle particlePrefab = _onStackParticles.GetRandom();
            PooledParticle pooledParticle = pool.Get(particlePrefab);
            var particleSystemMain = pooledParticle.ParticleSystem.main;
            particleSystemMain.startColor = _stackParticleColor;
            pooledParticle
                .PlayAndReturn(_transform.position, particlePrefab, pool, CancellationToken.None, Random.rotation);
        }

        public BlockData GetBlockData()
        {
            return new BlockData(_type, this, _isStackable, _isMovable);
        }
    }
}