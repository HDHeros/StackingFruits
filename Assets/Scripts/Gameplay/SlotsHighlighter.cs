using System.Collections.Generic;
using Gameplay.Blocks;
using Gameplay.GameCore;
using UnityEngine;

namespace Gameplay
{
    public class SlotsHighlighter
    {
        private readonly StackingGame _game;
        private readonly List<BlockSlot> _highlightedSlots;
        public BlockSlot[,] Slots;

        public SlotsHighlighter(StackingGame game)
        {
            _game = game;
            _highlightedSlots = new List<BlockSlot>(game.LevelData.FieldSize.x * 2);
        }

        public void OnReplacementBegin(BlockView block)
        {
            IEnumerator<Vector2Int> positions = _game.GetCellsToReplaceFrom(block.Slot.Position);
            int counter = 0;
            while (positions.MoveNext())
            {
                BlockSlot slot = Slots[positions.Current.x, positions.Current.y];
                slot.SetHighlighting(true, delay: counter * 0.15f);
                _highlightedSlots.Add(slot);
                counter++;
            }
        }

        public void OnReplacementFinished()
        {
            foreach (BlockSlot slot in _highlightedSlots) 
                slot.SetHighlighting(false);
            
            _highlightedSlots.Clear();
        }
    }
}