using System;
using UnityEngine;

namespace Gameplay.GameLogic
{
    [Serializable]
    public struct LevelData<TBlock> where TBlock : IEquatable<TBlock>
    {
        public TBlock EmptyBlockValue;
        public Vector2Int FieldSize;
        public TBlock[] Blocks;
    }
}