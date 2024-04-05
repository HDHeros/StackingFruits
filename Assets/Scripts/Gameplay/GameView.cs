using Gameplay.GameLogic;
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

        private BlockReplacer _replacer;

        private void Start()
        {
            _game = new StackingGame<int>();
            _game.BlockReplaced += OnBlockReplaced;
            _game.Reinitialize(_levelData);
            _replacer = new BlockReplacer(_camera, _replacementDepth);
            InitField();
        }

        private void OnBlockReplaced(Vector2Int from, Vector2Int to)
        {
            BlockView view = _slots[from.x, from.y].RemoveCurrentBlock();
            _slots[to.x, to.y].SetBlock(view);
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
                    slot.SetBlock(blockView);
                }
            }
        }

        [Button]
        private void Move(Vector2Int from, Vector2Int to) => 
            _game.MoveBlock(from, to);
    }
}