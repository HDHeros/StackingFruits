﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Blocks;
using Gameplay.GameCore;
using HDH.GoPool;
using HDH.UnityExt.Extensions;
using Infrastructure.SimpleInput;
using Infrastructure.SoundsLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private BlockSlot _slotPrefab;
        [SerializeField] private Camera _camera;
        [SerializeField] private WallDecals _wallDecals;
        [SerializeField] private float _replacementDepth;
        [SerializeField] private Bounds _gameFieldBounds;
        private StackingGame<BlockView> _game;
        private BlockSlot[,] _slots;
        private BlockReplacer _replacer;
        private IGoPool _pool;
        private BlocksContainer _blocksContainer;
        private SoundsService _sounds;
        private bool _isMovementLocked;
        private GameResult? _gameResult;

        public void Initialize(InputService input, IGoPool pool, BlocksContainer blocksContainer, SoundsService sounds)
        {
            _pool = pool;
            _blocksContainer = blocksContainer;
            _sounds = sounds;
            _game = new StackingGame<BlockView>();
            _replacer = new BlockReplacer(_camera, input, _replacementDepth, Move);
            _wallDecals.Initialize(_camera, pool);
        }
        
        public async UniTask<GameResult> StartGame(LevelData<BlockView> levelData)
        {
            _wallDecals.Clear();
            _game.Reinitialize(levelData);
            SetupField();
            _gameResult = null;
            await UniTask.WaitUntil(() => _gameResult.HasValue);
            return _gameResult ?? new GameResult();
        }

        private void SetupField()
        {
            _slots = new BlockSlot[_game.LevelData.FieldSize.x, _game.LevelData.FieldSize.y];
            Vector3 parentPosition = transform.position;
            Vector2 itemSize = _slotPrefab.Bounds.size;
            
            for (int y = 0; y < _game.LevelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _game.LevelData.FieldSize.x; x++)
                {
                    Vector2Int inGamePosition = new Vector2Int(x, y);
                    BlockSlot slot = _pool.Get(_slotPrefab, transform);
                    Vector3 worldPosition = parentPosition + new Vector3(GetLocalXPos(x), GetLocalYPos(y));
                    slot.Setup(inGamePosition, worldPosition);
                    _slots[x, y] = slot;
                    BlockView block = _game.GetCellValue(x, y);
                    if (block.Type == BlockType.None) continue;
                    BlockView blockView = _pool.Get(_blocksContainer.BlocksDict[block.Type], transform);
                    blockView.Setup(inGamePosition, _replacer);
                    slot.SetBlock(blockView, false);
                }
            }

            float GetLocalXPos(int index)
            {
                float allElementsWidth = itemSize.x * _game.LevelData.FieldSize.x;
                float paddingLeft = (_gameFieldBounds.size.x - allElementsWidth) * 0.5f;
                float left = _gameFieldBounds.center.x - _gameFieldBounds.size.x * 0.5f;
                float itemHalfSize = itemSize.x * 0.5f;
                return left + itemSize.x * index + paddingLeft + itemHalfSize;
            }

            float GetLocalYPos(int index)
            {
                float bottom = _gameFieldBounds.center.y - _gameFieldBounds.size.y * 0.5f;
                return bottom + itemSize.y * index + itemSize.y * 0.5f;
            }
        }

        [Button]
        private void Move(Vector2Int from, Vector2Int to)
        {
            if (_isMovementLocked) return;
            _isMovementLocked = true;
            _replacer.IsReplacementLocked = true;
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
                        await MoveBlock(move.Current.Actions[0].From, move.Current.Actions[0].To, TimeSpan.Zero);
                        break;
                    case GameEventType.BlocksFell:
                        await HandleBlocksFell(move);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
                        break;
                    case GameEventType.StackPerformed:
                        await AnimateStackPerform(move.Current.Actions);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
                        break;
                    case GameEventType.GameLost:
                        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
                        await UniTask.WhenAll(move.Current.Actions.Select(m => DropSlotContent(m.From)));
                        HandleLoose(move.Current.GameProgress);
                        break;
                    case GameEventType.GameWon:
                        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
                        HandleWin(move.Current.GameProgress);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _isMovementLocked = false;
            _replacer.IsReplacementLocked = false;
        }

        private UniTask HandleBlocksFell(IEnumerator<GameEvent> move)
        {
            List<UniTask> uniTasks = new List<UniTask>(move.Current.Actions.Count);
            
            for (var i = 0; i < move.Current.Actions.Count; i++)
            {
                uniTasks.Add(MoveBlock(move.Current.Actions[i].From, move.Current.Actions[i].To, TimeSpan.FromSeconds(0.04 * i)));
            }

            return UniTask.WhenAll(uniTasks);
        }

        private async UniTask AnimateStackPerform(IReadOnlyList<PerformedMovement> currentActions)
        {
            BlockView[] stackedFruits = currentActions.Select(a => _slots[a.From.x, a.From.y].ResetSlot()).ToArray();
            await MoveAllToStackPos();
            BlockView animatedFruit = SelectOneFruit();
            _sounds.RaiseEvent(EventId.StackExplosion);
            await animatedFruit.PlayOnStackAnimation(_pool);
            PlayStackFinishSound(_game.StacksPerformed);
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

                List<UniTask> uniTasks = new List<UniTask>(stackedFruits.Length);
                    // stackedFruits.Select(f => f.MoveToStackingAnimationPos(stackPos));
                for (var i = 0; i < stackedFruits.Length; i++)
                {
                    TimeSpan delay = TimeSpan.FromSeconds(0.2f * i);
                    uniTasks.Add(stackedFruits[i].MoveToStackingAnimationPos(stackPos, delay));
                }
                return UniTask.WhenAll(uniTasks);
            }

            BlockView SelectOneFruit()
            {
                for (var i = 1; i < stackedFruits.Length; i++) 
                    RemoveBlock(stackedFruits[i]);
                return stackedFruits[0];
            }

            void RemoveBlock(BlockView block) => 
                _pool.Return(block, _blocksContainer.BlocksDict[block.Type]);

            void PlayStackFinishSound(int stackNum)
            {
                switch (stackNum)
                {
                    case 1: _sounds.RaiseEvent(EventId.Stack1); return;
                    case 2: _sounds.RaiseEvent(EventId.Stack2); return;
                    case 3: _sounds.RaiseEvent(EventId.Stack3); return;
                    case 4: _sounds.RaiseEvent(EventId.Stack4); return;
                    case 5: _sounds.RaiseEvent(EventId.Stack5); return;
                    default: _sounds.RaiseEvent(EventId.Stack5); return;
                }
            }
        }

        private UniTask DropSlotContent(Vector2Int position)
        {
            return _slots[position.x, position.y].DropContent();
        }

        private async UniTask MoveBlock(Vector2Int from, Vector2Int to, TimeSpan delay)
        {
            BlockView view = _slots[from.x, from.y].RemoveCurrentBlock();
            await UniTask.Delay(delay);
            _sounds.RaiseEvent(EventId.FruitFell);
            await _slots[to.x, to.y].SetBlock(view, true);
        }

        private void HandleLoose(float progress) => 
            HandleFinishGame(progress);

        private void HandleWin(float progress) => 
            HandleFinishGame(progress);

        private void HandleFinishGame(float progress, bool isForceFinish = false)
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
            _gameResult = new GameResult{Progress = progress, WasForceFinished = isForceFinish};
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + _gameFieldBounds.center, _gameFieldBounds.size);
        }

        public struct GameResult
        {
            public float Progress;
            public bool WasForceFinished;
            public bool IsWin => Progress >= 1;
        }
        
        public void Pause()
        {
            _replacer.ForceFinishReplacement();
        }

        public void Unpause()
        {
            
        }

        public void ForceFinishGame()
        {
            _replacer.ForceFinishReplacement();
            HandleFinishGame(0, true);
        }
    }
}