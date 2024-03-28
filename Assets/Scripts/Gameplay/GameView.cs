using Gameplay.GameLogic;
using TriInspector;
using UnityEngine;

namespace Gameplay
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private BlockView _blockPrefab;
        private StackingGame<int> _game;
        private BlockView[,] _views;

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

        private void Start()
        {
            _game = new StackingGame<int>();
            _game.BlockReplaced += OnBlockReplaced;
            _game.Reinitialize(_levelData);
            SetViews();
        }

        private void OnBlockReplaced(Vector2Int from, Vector2Int to)
        {
            _views[from.x, from.y].MoveTo(to);
            _views[to.x, to.y] = _views[from.x, from.y];
            _views[from.x, from.y] = null;
        }

        private void SetViews()
        {
            _views = new BlockView[_game.LevelData.FieldSize.x, _game.LevelData.FieldSize.y];
            for (int y = 0; y < _game.LevelData.FieldSize.y; y++)
            {
                for (int x = 0; x < _game.LevelData.FieldSize.x; x++)
                {
                    int block = _game.GetBlock(x, y);
                    if (block == 0) continue;
                    BlockView blockView = Instantiate(_blockPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                    _views[x, y] = blockView;
                    blockView.Setup(block);
                }
            }
        }

        [Button]
        private void Move(Vector2Int from, Vector2Int to) => 
            _game.MoveBlock(from, to);
    }
}