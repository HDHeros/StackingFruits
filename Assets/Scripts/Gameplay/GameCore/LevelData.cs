using System;
using Gameplay.Blocks;
using UnityEngine;

namespace Gameplay.GameCore
{
    [Serializable]
    public struct LevelData
    {
        public BlockData EmptyBlockValue;
        public Vector2Int FieldSize;
        public BlockData[] Blocks;
    }

    public struct BlockData
    {
        public readonly BlockType Type;
        public readonly BlockView ViewPrefab;
        public readonly bool IsStackable;
        public readonly bool IsMovable;

        public BlockData(BlockType type, BlockView prefab, bool isStackable, bool isMovable)
        {
            Type = type;
            ViewPrefab = prefab;
            IsStackable = isStackable;
            IsMovable = isMovable;
        }
    }
}