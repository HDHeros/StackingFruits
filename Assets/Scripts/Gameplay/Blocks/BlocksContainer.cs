using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Blocks
{
    [CreateAssetMenu(menuName = "Configs/BlocksContainer", fileName = "BlocksContainer", order = 0)]
    public class BlocksContainer : ScriptableObject
    {
        public Dictionary<BlockType, BlockView> BlocksDict { get; private set; }
        public Block[] Blocks;
        
        
        [Serializable]
        public struct Block
        {
            public BlockType Type;
            public BlockView Prefab;
        }

        public void Initialize()
        {
            BlocksDict = Blocks.ToDictionary(b => b.Type, b => b.Prefab);
        }

    }
}