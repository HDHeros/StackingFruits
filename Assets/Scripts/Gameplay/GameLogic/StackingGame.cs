using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameLogic
{
    public class StackingGame<TBlock> where TBlock : IEquatable<TBlock>
    {
        public LevelData<TBlock> LevelData => _levelData;
        private LevelData<TBlock> _levelData;
        private TBlock[,] _field;
        private Dictionary<TBlock, int> _blocksCount;
        private List<PerformedMovement> _performedMovements;

        public StackingGame() => 
            _performedMovements = null;

        public void Reinitialize(LevelData<TBlock> levelData)
        {
            _performedMovements ??= new List<PerformedMovement>(levelData.FieldSize.y);
            _levelData = levelData;
            _field = new TBlock[levelData.FieldSize.x, levelData.FieldSize.y];
            _blocksCount = new Dictionary<TBlock, int>(8);
            for (var i = 0; i < levelData.Blocks.Length; i++)
            {
                TBlock block = levelData.Blocks[i];
                int x = i % levelData.FieldSize.x;
                int y = Mathf.FloorToInt((float) i / levelData.FieldSize.x);
                _field[x, y] = block;
                _blocksCount.TryAdd(block, 0);
                _blocksCount[block]++;
            }
        }

        public struct GameEvent
        {
            public readonly GameEventType Type;
            public readonly IReadOnlyList<PerformedMovement> Actions;
                
            public GameEvent(GameEventType type, IReadOnlyList<PerformedMovement> actions)
            {
                Type = type;
                Actions = actions;
            }
        }

        public struct PerformedMovement
        {
            public readonly Vector2Int From;
            public readonly Vector2Int To;

            public PerformedMovement(Vector2Int from, Vector2Int to)
            {
                From = from;
                To = to;
            }
        }
        
        public enum GameEventType{None, BlockMovedByUser, BlocksFell}
        
        public IEnumerator<GameEvent> MoveBlock(Vector2Int from, Vector2Int to)
        {
            if (IsInRange(from.x, from.y) == false || IsInRange(to.x, to.y) == false)
                throw new ArgumentOutOfRangeException();

            _performedMovements.Clear();
            _performedMovements.Add(ReplaceBlock(from, to));
            yield return new GameEvent(GameEventType.BlockMovedByUser, _performedMovements);
            
            _performedMovements.Clear();
            if (NormalizeField())
                yield return new GameEvent(GameEventType.BlocksFell, _performedMovements);
        }

        public TBlock GetBlock(int x, int y) => 
            _field[x, y];

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

            return _performedMovements.Count > 0;
        }

        private bool TryShiftDown(int x, int y, out PerformedMovement movement)
        {
            movement = default;
            TBlock block = GetBlock(x, y);
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

        private bool TryGetBlock(int x, int y, out TBlock block)
        {
            block = default;
            if (IsInRange(x, y) == false || GetBlock(x, y).Equals(_levelData.EmptyBlockValue)) return false;
            block = GetBlock(x, y);
            return true;
        }

        private PerformedMovement ReplaceBlock(Vector2Int from, Vector2Int to)
        {
            SetBlock(to.x, to.y, GetBlock(from.x, from.y));
            SetBlock(from.x, from.y, _levelData.EmptyBlockValue);
            return new PerformedMovement(from, to);
        }

        private void SetBlock(int x, int y, TBlock value) =>
            _field[x, y] = value;

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