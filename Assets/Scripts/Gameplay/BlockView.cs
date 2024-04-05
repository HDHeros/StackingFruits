using HDH.UnityExt.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public class BlockView : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private MeshRenderer _renderer;
        public Transform Transform => _transform;
        public BlockSlot Slot { get; private set; }

        private BlockReplacer _replacer;
        private Transform _transform;

        public void Setup(int block, Vector2Int position, BlockReplacer replacer)
        {
            Color color = Color.white;
            switch (block)
            {
                case 1: color = Color.green; break;
                case 2: color = Color.yellow; break;
                case 3: color = Color.red; break;
            }

            transform.position = new Vector3(position.x, position.y);
            _renderer.material.color = color;
            _replacer = replacer;
        }

        public void MoveTo(Vector2Int to, bool animated)
        {
            Vector3 position = new Vector3(to.x, to.y);
            // if (animated == false)
            {
                transform.position = position;
                return;
            }
            
            // Tween tween = transform.DOMove(position, 0.2f);
            // return UniTask.WaitWhile(() => tween.IsActive() && tween.IsPlaying());
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

        private void Awake()
        {
            _transform = transform;
        }
    }
}