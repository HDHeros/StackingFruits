using System;
using System.Collections.Generic;
using Gameplay.Blocks;
using UnityEngine;

namespace Gameplay.GameCore
{
    public class StackingGame
    {
        private struct CellInfo
        {
            public BlockData Value;
            public bool Counted;
        }
        
        public LevelData LevelData => _levelData;
        public int StacksPerformed => _stacksPerformed;
        private float GameProgress => (float)_stacksPerformed / _totalStackAmountOnLevel;
        private LevelData _levelData;
        private CellInfo[,] _field;
        private Dictionary<BlockType, int> _blocksCount;
        private Dictionary<BlockType, bool> _blocksChecked;
        private List<PerformedMovement> _performedMovements;
        private int _totalStackAmountOnLevel;
        private bool _isFieldNormalized;
        private int _stacksPerformed;


        public StackingGame() => 
            _performedMovements = null;

        public void Reinitialize(LevelData levelData)
        {
            _performedMovements ??= new List<PerformedMovement>(levelData.FieldSize.y);
            _levelData = levelData;
            _field = new CellInfo[levelData.FieldSize.x, levelData.FieldSize.y];
            _blocksCount = new Dictionary<BlockType, int>(8);
            _blocksChecked = new Dictionary<BlockType, bool>(8);
            _totalStackAmountOnLevel = 0;
            for (var i = 0; i < levelData.Blocks.Length; i++)
            {
                BlockData block = levelData.Blocks[i];
                int x = i % levelData.FieldSize.x;
                int y = Mathf.FloorToInt((float) i / levelData.FieldSize.x);
                _field[x, y] = new CellInfo{Value = block};
                if (_blocksCount.TryAdd(block.Type, 0))
                {
                    _totalStackAmountOnLevel += block.IsStackable ? 1 : 0;
                }
                _blocksCount[block.Type]++;
                _blocksChecked.TryAdd(block.Type, false);
            }

        }

        public BlockData GetCellValue(int x, int y) => 
            _field[x, y].Value;

        public IEnumerator<GameEvent> MoveBlock(Vector2Int from, Vector2Int to)
        {
            if (IsInRange(from.x, from.y) == false || IsInRange(to.x, to.y) == false)
            {
                yield return new GameEvent(GameEventType.WrongActionRequested, _performedMovements);
                yield break;
            }

            if (_field[to.x, to.y].Value.Equals(_levelData.EmptyBlockValue) == false)
            {
                yield return new GameEvent(GameEventType.WrongActionRequested, _performedMovements);
                yield break;
            }

            _performedMovements.Clear();
            _performedMovements.Add(ReplaceBlock(from, to));
            yield return new GameEvent(GameEventType.BlockMovedByUser, _performedMovements);
            
            if (NormalizeField())
                yield return new GameEvent(GameEventType.BlocksFell, _performedMovements);

            _stacksPerformed = 0;
            bool isStackPerformed = HandleStacks();
            while (isStackPerformed || _isFieldNormalized == false)
            {
                if (isStackPerformed)
                {
                    _stacksPerformed++;
                    isStackPerformed = false;
                    yield return new GameEvent(GameEventType.StackPerformed, _performedMovements);
                    continue;
                }

                if (NormalizeField())
                {
                    yield return new GameEvent(GameEventType.BlocksFell, _performedMovements);
                }
                
                isStackPerformed = HandleStacks();
            }
            
            if (_stacksPerformed == 0)
                yield break;
            
            if (_stacksPerformed == _totalStackAmountOnLevel)
            {
                yield return new GameEvent(GameEventType.GameWon, _performedMovements, GameProgress);
                yield break;
            }
            
            _performedMovements.Clear();
            for (int y = 0; y < _levelData.FieldSize.y; y++)
            for (int x = 0; x < _levelData.FieldSize.x; x++)
                if(_field[x, y].Value.Equals(_levelData.EmptyBlockValue) == false)
                    _performedMovements.Add(new PerformedMovement(new Vector2Int(x, y), default));
                    
            yield return new GameEvent(GameEventType.GameLost, _performedMovements, GameProgress);
        }

        public IEnumerator<Vector2Int> GetCellsToReplaceFrom(Vector2Int position)
        {
            for (int x = 0; x < _levelData.FieldSize.x; x++)
            {
                for (int y = -1; y < _levelData.FieldSize.y; y++)
                {
                    if (IsInRange(x, y + 1) && GetCellValue(x, y + 1).Equals(_levelData.EmptyBlockValue))
                    {
                        yield return new Vector2Int(x, y + 1);
                        break;
                    }
                }
            }
        }

        public bool IsInRange(int x, int y)
        {
            if (x < 0 || x >= _levelData.FieldSize.x)
                return false;
            if (y < 0 || y >= _levelData.FieldSize.y)
                return false;
            return true;
        }

        private bool HandleStacks()
        {
            _blocksChecked = new Dictionary<BlockType, bool>();
            foreach (BlockType blockType in _blocksCount.Keys) 
                _blocksChecked[blockType] = false;
            
            for (int y = 0; y < _levelData.FieldSize.y; y++)
            for (int x = 0; x < _levelData.FieldSize.x; x++)
                _field[x, y].Counted = false;
            
            for (int y = 0; y < _levelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _levelData.FieldSize.x; x++)
                {
                    BlockData blockData = _field[x, y].Value;
                    if (blockData.Equals(_levelData.EmptyBlockValue) || blockData.IsStackable == false || _blocksChecked[blockData.Type]) continue;
                    _blocksChecked[blockData.Type] = true;
                    _performedMovements.Clear();
                    int stackSize = CountSelfAndNearBlocks(x, y);
                    if (stackSize != _blocksCount[blockData.Type]) continue;

                    foreach (PerformedMovement performedMovement in _performedMovements) 
                        RemoveFromField(performedMovement.From.x, performedMovement.From.y);
                    
                    return true;
                }
            }
            

            return false;
        }

        private int CountSelfAndNearBlocks(int x, int y)
        {
            int count = 1;
            CellInfo selfInfo = _field[x, y];
            _field[x, y].Counted = true;
            _performedMovements.Add(new PerformedMovement(new Vector2Int(x, y), default));
            if (TryGetBlock(x, y - 1, out var nearInfo) && nearInfo.Value.Equals(selfInfo.Value) && nearInfo.Counted == false)
                count += CountSelfAndNearBlocks(x, y - 1);
    
            if (TryGetBlock(x - 1, y, out nearInfo) && nearInfo.Value.Equals(selfInfo.Value) && nearInfo.Counted == false)
                count += CountSelfAndNearBlocks(x - 1, y);
    
            if (TryGetBlock(x, y + 1, out nearInfo) && nearInfo.Value.Equals(selfInfo.Value) && nearInfo.Counted == false)
                count += CountSelfAndNearBlocks(x, y + 1);
    
            if (TryGetBlock(x + 1, y, out nearInfo) && nearInfo.Value.Equals(selfInfo.Value) && nearInfo.Counted == false)
                count += CountSelfAndNearBlocks(x + 1, y);

            return count;
        }

        private bool NormalizeField()
        {
            _performedMovements.Clear();
            for (int y = 0; y < _levelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _levelData.FieldSize.x; x++)
                {
                    if (TryShiftDown(x, y, out PerformedMovement movement) == false) continue;
                    _performedMovements.Add(movement);
                }
            }

            _isFieldNormalized = true;
            return _performedMovements.Count > 0;
        }

        private bool TryShiftDown(int x, int y, out PerformedMovement movement)
        {
            movement = default;
            BlockData block = GetCellValue(x, y);
            if (block.IsMovable == false) return false;
            if (block.Equals(_levelData.EmptyBlockValue)) return false;
            int targetY = -1;
            for (int i = y - 1; i >= 0; i--)
            {
                if (TryGetBlock(x, i, out var foundedBlock) && foundedBlock.Value.Type != BlockType.Blocker) break;
                if (foundedBlock.Value.Type != BlockType.Blocker)
                    targetY = i;
            }
            if (targetY == -1 || (GetCellValue(x, targetY).IsMovable == false)) return false;
            movement = ReplaceBlock(new Vector2Int(x, y), new Vector2Int(x, targetY));
            return true;
        }

        private bool TryGetBlock(int x, int y, out CellInfo cell)
        {
            cell = default;
            if (IsInRange(x, y) == false || GetCellValue(x, y).Equals(_levelData.EmptyBlockValue)) return false;
            cell = _field[x, y];
            return true;
        }

        private PerformedMovement ReplaceBlock(Vector2Int from, Vector2Int to)
        {
            BlockData replacedBlock = GetCellValue(from.x, from.y);
            SetCell(to.x, to.y, replacedBlock);
            SetCell(from.x, from.y, _levelData.EmptyBlockValue);
            _blocksChecked[replacedBlock.Type] = false;
            return new PerformedMovement(from, to);
        }

        private void RemoveFromField(int x, int y) => 
            SetCell(x, y, _levelData.EmptyBlockValue);

        private void SetCell(int x, int y, BlockData value)
        {
            _isFieldNormalized = false;
            _field[x, y].Value = value;
        }
    }
}