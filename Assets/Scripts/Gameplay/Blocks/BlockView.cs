using Cysharp.Threading.Tasks;
using DG.Tweening;
using HDH.UnityExt.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Blocks
{
    public class BlockView : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private BlockType _type;
        public Transform Transform => _transform;
        public BlockSlot Slot { get; private set; }
        public BlockType Type => _type;

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

        private UniTask AnimateDrop(Vector3 position)
        {
            Tween tween = transform.DOMove(position, 20f).SetSpeedBased(true).SetEase(Ease.InSine);
            return UniTask.WaitWhile(() => tween.IsActive() && tween.IsPlaying());
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

        private void Awake()
        {
            _transform = transform;
        }

        private void OnDisable()
        {
            ResetBlock();
        }
    }
}