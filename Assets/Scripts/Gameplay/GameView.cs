using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.GameLogic;
using Infrastructure.SimpleInput;
using TriInspector;
using UnityEngine;

namespace Gameplay
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private BlockSlot _slotPrefab;
        [SerializeField] private BlockView _blockPrefab;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _replacementDepth;
        private StackingGame<int> _game;
        // private BlockView[,] _blocks;
        private BlockSlot[,] _slots;
        private BlockReplacer _replacer;
        private bool _isMovementLocked;
        private LevelData<int> _levelData = new()
        {
            EmptyBlockValue = 0,
            FieldSize = new Vector2Int(4, 6),
            Blocks = new []
            {
                1,1,2,3,
                1,3,2,3,
                0,2,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
            }
        };


        public void Setup(InputService input)
        {
            _game = new StackingGame<int>();
            _game.Reinitialize(_levelData);
            _replacer = new BlockReplacer(_camera, input, _replacementDepth, Move);
            InitField();
        }

        private void InitField()
        {
            _slots = new BlockSlot[_game.LevelData.FieldSize.x, _game.LevelData.FieldSize.y];
            for (int y = 0; y < _game.LevelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _game.LevelData.FieldSize.x; x++)
                {
                    Vector2Int inGamePosition = new Vector2Int(x, y);

                    BlockSlot slot = Instantiate(_slotPrefab, transform);
                    slot.Setup(inGamePosition);
                    _slots[x, y] = slot;
                    int block = _game.GetBlock(x, y);
                    if (block == 0) continue;
                    BlockView blockView = Instantiate(_blockPrefab, transform);
                    blockView.Setup(block, inGamePosition, _replacer);
                    slot.SetBlock(blockView, false);
                }
            }
        }

        [Button]
        private void Move(Vector2Int from, Vector2Int to)
        {
            if (_isMovementLocked) return;
            _isMovementLocked = true;
            HandleMovementAsync(from, to).Forget();
        }

        private async UniTaskVoid HandleMovementAsync(Vector2Int from, Vector2Int to)
        {
            IEnumerator<GameEvent> move = _game.MoveBlock(from, to);

            while (move.MoveNext())
            {
                switch (move.Current.Type)
                {
                    case GameEventType.BlockMovedByUser:
                        await MoveBlock(move.Current.Actions[0].From, move.Current.Actions[0].To);
                        break;
                    case GameEventType.BlocksFell:
                        await UniTask.WhenAll(move.Current.Actions.Select(m => MoveBlock(m.From, m.To)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _isMovementLocked = false;
        }
        
        private UniTask MoveBlock(Vector2Int from, Vector2Int to)
        {
            BlockView view = _slots[from.x, from.y].RemoveCurrentBlock();
            return _slots[to.x, to.y].SetBlock(view, true);
        }
    }
}