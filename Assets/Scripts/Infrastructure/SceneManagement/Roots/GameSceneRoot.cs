using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.GameLogic;
using HDH.GoPool;
using Infrastructure.SimpleInput;
using UnityEngine;

namespace Infrastructure.SceneManagement.Roots
{
    public class GameSceneRoot : SceneRoot
    {
        [SerializeField] private GameView _gameView;
        private CancellationTokenSource _ctSource;
        private InputService _input;
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

        public void OnEnter(InputService inputService, IGoPool pool)
        {
            _ctSource = new CancellationTokenSource();
            _input = inputService;
            _gameView.Initialize(_input, pool);
            StartLoop(_ctSource.Token).Forget();
        }

        public void OnExit()
        {
            _ctSource.Cancel();
        }

        private async UniTaskVoid StartLoop(CancellationToken ct)
        {
            while (ct.IsCancellationRequested == false)
            {
                await _gameView.StartGame(_levelData);
            }
        }
    }
}