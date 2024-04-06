using System;
using Cysharp.Threading.Tasks;
using Infrastructure.SceneManagement;
using Infrastructure.SceneManagement.Roots;

namespace Infrastructure.GameStateMachine.States
{
    public class GameSceneState : BaseGameState
    {
        private GameSceneRoot _root;

        public override void Enter()
        {
            base.Enter();
            Setup().Forget();
        }

        public override void Exit(Action onExit)
        {
            _root.OnExit();
            base.Exit(onExit);
        }
        
        private async UniTaskVoid Setup()
        {
            _root = await Fields.SceneService.LoadSceneSingle<GameSceneRoot>(SceneId.GameScene);
            _root.OnEnter(Fields.InputService, Fields.GoPool, Fields.GameConfig);
        }
    }
}