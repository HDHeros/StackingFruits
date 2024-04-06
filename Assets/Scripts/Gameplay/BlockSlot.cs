using System;
using Cysharp.Threading.Tasks;
using Gameplay.Blocks;
using UnityEngine;

namespace Gameplay
{
    public class BlockSlot : MonoBehaviour
    {
        public bool IsAvailableForReceive => _isContainBlock == false;
        public Vector3 WorldPosition => _transform.position;
        public Vector2Int Position => _position;
        private Vector2Int _position;
        private BlockView _currentBlock;
        private Transform _transform;
        private bool _isContainBlock;

        public void Setup(Vector2Int position)
        {
            gameObject.SetActive(true);
            _position = position;
            transform.position = new Vector3(_position.x, _position.y);
        }

        public UniTask SetBlock(BlockView block, bool animated)
        {
            if (_isContainBlock)
                throw new Exception();
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

        public UniTask FitCurrentBlockInside(bool animated) => 
            _currentBlock.MoveTo(_position, animated);

        public async UniTask<BlockView> StackContainingBlock()
        {
            var block = _currentBlock;
            _currentBlock = null;
            _isContainBlock = false;
            await block.AnimateStacking();
            return block;
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
    }
}