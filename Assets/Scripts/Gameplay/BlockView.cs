using DG.Tweening;
using HDH.UnityExt.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay
{
    public class BlockView : MonoBehaviour, IPointerDownHandler, IBeginDragHandler
    {
        [SerializeField] private MeshRenderer _renderer;
        public Transform Transform => _transform;
        public BlockSlot Slot { get; private set; }

        private Vector2Int _position;
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

            _position = position;
            transform.position = new Vector3(_position.x, _position.y);
            _renderer.material.color = color;
            _replacer = replacer;
        }

        public void MoveTo(Vector2Int to)
        {
            transform.DOMove(new Vector3(to.x, to.y), 0.2f);
            _position = to;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_replacer.TryBeginReplacement(eventData, this) == false) return;
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