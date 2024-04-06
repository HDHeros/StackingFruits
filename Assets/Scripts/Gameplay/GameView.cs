using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Blocks;
using Gameplay.GameCore;
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
        [SerializeField] private Camera _camera;
        [SerializeField] private WallDecals _wallDecals;
        [SerializeField] private float _replacementDepth;
        private StackingGame<int> _game;
        private BlockSlot[,] _slots;
        private BlockReplacer _replacer;
        private IGoPool _pool;
        private BlocksContainer _blocksContainer;
        private bool _isMovementLocked;
        private bool _gameFinished;

        public void Initialize(InputService input, IGoPool pool, BlocksContainer blocksContainer)
        {
            _pool = pool;
            _blocksContainer = blocksContainer;
            _replacer = new BlockReplacer(_camera, input, _replacementDepth, Move);
            _wallDecals.Initialize(_camera, pool);
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
                    BlockView blockView = _pool.Get(_blocksContainer.BlocksDict[(BlockType)block], transform);
                    blockView.Setup(inGamePosition, _replacer);
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
                        await AnimateStackPerform(move.Current.Actions);
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
        
        private async UniTask AnimateStackPerform(IReadOnlyList<PerformedMovement> currentActions)
        {
            BlockView[] stackedFruits = currentActions.Select(a => _slots[a.From.x, a.From.y].ResetSlot()).ToArray();
            await MoveAllToStackPos();
            BlockView animatedFruit = SelectOneFruit();
            await animatedFruit.PlayOnStackAnimation(_pool);
            _wallDecals.SpawnDecal(animatedFruit.Transform.position, animatedFruit.DecalColor);
            RemoveBlock(animatedFruit);
            
            UniTask MoveAllToStackPos()
            {
                Vector3 avgPos = Vector3.zero;
                foreach (BlockView fruit in stackedFruits) 
                    avgPos += fruit.Transform.position;
                avgPos = new Vector3(avgPos.x / stackedFruits.Length, avgPos.y / stackedFruits.Length, -2);
                Vector3 stackPos = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));
                stackPos = Vector3.Lerp(avgPos, stackPos, 0.5f);

                return UniTask.WhenAll(stackedFruits.Select(f => f.MoveToStackingAnimationPos(stackPos)));
            }

            BlockView SelectOneFruit()
            {
                for (var i = 1; i < stackedFruits.Length; i++) 
                    RemoveBlock(stackedFruits[i]);
                return stackedFruits[0];
            }

            void RemoveBlock(BlockView block) => 
                _pool.Return(block, _blocksContainer.BlocksDict[block.Type]);
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

        // private async UniTask ClearSlot(Vector2Int position)
        // {
        //     BlockView block = await _slots[position.x, position.y].ResetSlot();
        //     _pool.Return(block, _blocksContainer.BlocksDict[block.Type]);
        // }
        
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
                    _pool.Return(block, _blocksContainer.BlocksDict[block.Type]);
                }
                _pool.Return(_slots[x, y], _slotPrefab);
            }
            _wallDecals.Clear();
            _gameFinished = true;
        }
    }
}