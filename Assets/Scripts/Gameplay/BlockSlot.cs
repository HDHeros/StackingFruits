using System;
using Cysharp.Threading.Tasks;
using Gameplay.Blocks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class BlockSlot : MonoBehaviour
    {
        [SerializeField] private Bounds _bounds;
        public bool IsAvailableForReceive => _isContainBlock == false;
        public Bounds Bounds => _bounds;
        public Vector2Int Position => _position;

        private Vector2Int _position;
        private BlockView _currentBlock;
        private Transform _transform;
        private bool _isContainBlock;

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
            return block;
        }

        private void Awake() => 
            _transform = transform;

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