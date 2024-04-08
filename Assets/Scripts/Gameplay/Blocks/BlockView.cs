using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HDH.GoPool;
using HDH.GoPool.Components;
using HDH.UnityExt.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Gameplay.Blocks
{
    public class BlockView : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEquatable<BlockView>
    {
        [SerializeField] private BlockType _type;
        [SerializeField] private PooledParticle[] _onStackParticles;
        [SerializeField] private ParticleSystem.MinMaxGradient _stackParticleColor;
        [SerializeField] private Color _decalColor;
        public Transform Transform => _transform;
        public BlockSlot Slot { get; private set; }
        public BlockType Type => _type;
        public Color DecalColor => _decalColor;

        private BlockReplacer _replacer;
        private Transform _transform;


        public void Setup(Vector2Int position, BlockReplacer replacer)
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(position.x, position.y);
            _replacer = replacer;
        }

        public UniTask MoveTo(Vector2Int to, bool animated)
        {
            Vector3 position = new Vector3(to.x, to.y);
            if (animated == false)
            {
                transform.position = position;
                return UniTask.CompletedTask;
            }
            
            return AnimateDrop(position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_replacer.TryBeginReplacement(eventData, this) == false) return;
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

        public UniTask AnimateStacking()
        {
            _transform.DOKill();
            Tween tween = _transform.DOScale(0, 0.5f).SetEase(Ease.InBack);
            return UniTask.WaitWhile(() => tween.IsActive() && tween.IsPlaying());
        }

        public UniTask Drop()
        {
            return AnimateDrop(_transform.position.AddY(-20));
        }

        public void ResetBlock()
        {
            _transform.DOKill();
            _transform.localScale = Vector3.one;
            Slot = null;
        }

        private UniTask AnimateDrop(Vector3 position)
        {
            Tween tween = transform.DOMove(position, 20f).SetSpeedBased(true).SetEase(Ease.InSine);
            return UniTask.WaitWhile(() => tween.IsActive() && tween.IsPlaying());
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void OnDisable()
        {
            ResetBlock();
            _transform.localScale = Vector3.one;
            _transform.rotation = Quaternion.identity;
        }

        public UniTask MoveToStackingAnimationPos(Vector3 position)
        {
            _transform.DOKill();
            Sequence sequence = DOTween.Sequence()
                .Append(_transform.DOMove(position, 0.25f).SetEase(Ease.InSine))
                .Join(_transform.DORotate(Vector3.zero.WithY(Random.Range(90, 180)), 0.25f));
            
            return UniTask.WaitWhile(() => sequence.IsActive() && sequence.IsPlaying());
        }

        public async UniTask PlayOnStackAnimation(IGoPool pool)
        {
            _transform.DOKill();
            Sequence sequence = DOTween.Sequence()
                .Append(_transform.DOScale(Vector3.one * 0.8f, 0.5f).SetEase(Ease.Linear))
                .Join(_transform.DOShakePosition(0.5f, strength: 0.05f, vibrato: 15, fadeOut: false))
                .Join(_transform.DOShakeRotation(0.5f, strength: 15f, vibrato: 15, fadeOut: false));
            await UniTask.WaitWhile(() => sequence.IsActive() && sequence.IsPlaying());
            SpawnStackingParticle(pool);
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

        public bool Equals(BlockView other)
        {
            return other != null && other.Type == _type;
        }
    }
}