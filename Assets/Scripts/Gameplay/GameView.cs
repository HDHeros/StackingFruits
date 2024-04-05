using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.GameLogic;
using HDH.GoPool;
using HDH.UnityExt.Extensions;
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
        private BlockSlot[,] _slots;
        private BlockReplacer _replacer;
        private IGoPool _pool;
        private bool _isMovementLocked;
        private bool _gameFinished;

        public void Initialize(InputService input, IGoPool pool)
        {
            _pool = pool;
            _replacer = new BlockReplacer(_camera, input, _replacementDepth, Move);
        }
        
        public UniTask StartGame(LevelData<int> levelData)
        {
            _game = new StackingGame<int>();
            _game.Reinitialize(levelData);
            SetupField();
            _gameFinished = false;
            return UniTask.WaitUntil(() => _gameFinished);
        }

        private void SetupField()
        {
            _slots = new BlockSlot[_game.LevelData.FieldSize.x, _game.LevelData.FieldSize.y];
            for (int y = 0; y < _game.LevelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _game.LevelData.FieldSize.x; x++)
                {
                    Vector2Int inGamePosition = new Vector2Int(x, y);

                    BlockSlot slot = _pool.Get(_slotPrefab, transform);
                    slot.Setup(inGamePosition);
                    _slots[x, y] = slot;
                    int block = _game.GetCellValue(x, y);
                    if (block == 0) continue;
                    BlockView blockView = _pool.Get(_blockPrefab, transform);
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
                Debug.Log($"{move.Current.Type} - {move.Current.Actions.Count}");
                switch (move.Current.Type)
                {
                    case GameEventType.BlockMovedByUser:
                        await MoveBlock(move.Current.Actions[0].From, move.Current.Actions[0].To);
                        break;
                    case GameEventType.BlocksFell:
                        await UniTask.WhenAll(move.Current.Actions.Select(m => MoveBlock(m.From, m.To)));
                        break;
                    case GameEventType.StackPerformed:
                        await UniTask.WhenAll(move.Current.Actions.Select(m => ClearSlot(m.From)));
                        break;
                    case GameEventType.GameLost:
                        await UniTask.WhenAll(move.Current.Actions.Select(m => DropSlotContent(m.From)));
                        HandleLoose();
                        break;
                    case GameEventType.GameWon:
                        HandleWin();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _isMovementLocked = false;
        }

        private UniTask DropSlotContent(Vector2Int position)
        {
            return _slots[position.x, position.y].DropContent();
        }

        private UniTask MoveBlock(Vector2Int from, Vector2Int to)
        {
            BlockView view = _slots[from.x, from.y].RemoveCurrentBlock();
            return _slots[to.x, to.y].SetBlock(view, true);
        }

        private async UniTask ClearSlot(Vector2Int position)
        {
            BlockView block = await _slots[position.x, position.y].StackContainingBlock();
            _pool.Return(block, _blockPrefab);
        }
        
        private void HandleLoose() => 
            HandleFinishGame();

        private void HandleWin() => 
            HandleFinishGame();

        private void HandleFinishGame()
        {
            for (int y = 0; y < _game.LevelData.FieldSize.y; y++)
            for (int x = 0; x < _game.LevelData.FieldSize.x; x++)
            {
                BlockView block = _slots[x, y].ResetSlot();
                if (block.IsNotNull())
                {
                    block.ResetBlock();
                    _pool.Return(block, _blockPrefab);
                }
                _pool.Return(_slots[x, y], _slotPrefab);
            }
            _gameFinished = true;
        }
    }
}