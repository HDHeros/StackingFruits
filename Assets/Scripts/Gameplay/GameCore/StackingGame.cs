using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameCore
{
    public class StackingGame<TBlock> where TBlock : IEquatable<TBlock>
    {
        private struct CellInfo
        {
            public TBlock Value;
            public bool Counted;
        }
        
        public LevelData<TBlock> LevelData => _levelData;
        public int StacksPerformed => _stacksPerformed;
        private float GameProgress => (float)_stacksPerformed / _numberOfBlockTypesOnLevel;
        private LevelData<TBlock> _levelData;
        private CellInfo[,] _field;
        private Dictionary<TBlock, int> _blocksCount;
        private Dictionary<TBlock, bool> _blocksChecked;
        private List<PerformedMovement> _performedMovements;
        private int _numberOfBlockTypesOnLevel;
        private bool _isFieldNormalized;
        private int _stacksPerformed;


        public StackingGame() => 
            _performedMovements = null;

        public void Reinitialize(LevelData<TBlock> levelData)
        {
            _performedMovements ??= new List<PerformedMovement>(levelData.FieldSize.y);
            _levelData = levelData;
            _field = new CellInfo[levelData.FieldSize.x, levelData.FieldSize.y];
            _blocksCount = new Dictionary<TBlock, int>(8);
            _blocksChecked = new Dictionary<TBlock, bool>(8);
            for (var i = 0; i < levelData.Blocks.Length; i++)
            {
                TBlock block = levelData.Blocks[i];
                int x = i % levelData.FieldSize.x;
                int y = Mathf.FloorToInt((float) i / levelData.FieldSize.x);
                _field[x, y] = new CellInfo{Value = block};
                _blocksCount.TryAdd(block, 0);
                _blocksChecked.TryAdd(block, false);
                _blocksCount[block]++;
            }

            _numberOfBlockTypesOnLevel = _blocksCount.Count - 1;
        }

        public TBlock GetCellValue(int x, int y) => 
            _field[x, y].Value;

        public IEnumerator<GameEvent> MoveBlock(Vector2Int from, Vector2Int to)
        {
            if (IsInRange(from.x, from.y) == false || IsInRange(to.x, to.y) == false)
                throw new ArgumentOutOfRangeException();

            if (_field[to.x, to.y].Value.Equals(_levelData.EmptyBlockValue) == false)
                throw new Exception();

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
            
            if (_stacksPerformed == _numberOfBlockTypesOnLevel)
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

        private bool HandleStacks()
        {
            _blocksChecked = new Dictionary<TBlock, bool>();
            foreach (TBlock blockType in _blocksCount.Keys) 
                _blocksChecked[blockType] = false;
            
            for (int y = 0; y < _levelData.FieldSize.y; y++)
            for (int x = 0; x < _levelData.FieldSize.x; x++)
                _field[x, y].Counted = false;
            
            for (int y = 0; y < _levelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _levelData.FieldSize.x; x++)
                {
                    TBlock blockType = _field[x, y].Value;
                    if (blockType.Equals(_levelData.EmptyBlockValue) || _blocksChecked[blockType]) continue;
                    _blocksChecked[blockType] = true;
                    _performedMovements.Clear();
                    int stackSize = CountSelfAndNearBlocks(x, y);
                    if (stackSize != _blocksCount[blockType]) continue;

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
            TBlock block = GetCellValue(x, y);
            if (block.Equals(_levelData.EmptyBlockValue)) return false;
            int targetY = -1;
            for (int i = y - 1; i >= 0; i--)
            {
                if (TryGetBlock(x, i, out _)) break;
                targetY = i;
            }
            if (targetY == -1) return false;
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
            TBlock replacedBlockType = GetCellValue(from.x, from.y);
            SetCell(to.x, to.y, replacedBlockType);
            SetCell(from.x, from.y, _levelData.EmptyBlockValue);
            _blocksChecked[replacedBlockType] = false;
            return new PerformedMovement(from, to);
        }

        private void RemoveFromField(int x, int y) => 
            SetCell(x, y, _levelData.EmptyBlockValue);

        private void SetCell(int x, int y, TBlock value)
        {
            _isFieldNormalized = false;
            _field[x, y].Value = value;
        }

        private bool IsInRange(int x, int y)
        {
            if (x < 0 || x >= _levelData.FieldSize.x)
                return false;
            if (y < 0 || y >= _levelData.FieldSize.y)
                return false;
            return true;
        }
    }
}