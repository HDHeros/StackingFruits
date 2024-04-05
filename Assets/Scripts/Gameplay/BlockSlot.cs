using System;
using UnityEngine;

namespace Gameplay
{
    public class BlockSlot : MonoBehaviour
    {
        private Vector2Int _position;
        private BlockView _currentBlock;
        private bool _isContainBlock;
        public bool IsAvailableForReceive => _isContainBlock == false;
        public Transform Transform { get; private set; }

        public void Setup(Vector2Int position)
        {
            _position = position;
            transform.position = new Vector3(_position.x, _position.y);
        }

        public void SetBlock(BlockView block)
        {
            if (_isContainBlock)
                throw new Exception();
            _currentBlock = block;
            block.SetSlot(this);
            _currentBlock.MoveTo(_position);
        }

        public BlockView RemoveCurrentBlock()
        {
            BlockView block = _currentBlock;
            _currentBlock = null;
            _isContainBlock = false;
            return block;
        }

        private void Awake() => 
            Transform = transform;

        public void FitCurrentBlockInside()
        {
            _currentBlock.Transform.position = Transform.position;
            _currentBlock.Transform.rotation = Transform.rotation;
        }
    }
}