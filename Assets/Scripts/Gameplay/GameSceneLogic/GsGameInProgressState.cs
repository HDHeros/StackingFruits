using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gameplay.GameSceneLogic
{
    public class GsGameInProgressState : GsBaseState
    {
        private CancellationTokenSource _ctSource;

        public override void Enter()
        {
            _ctSource = new CancellationTokenSource();
            Fields.CameraController.ActivateInGameCamera();
            StartGameLoop(_ctSource.Token).Forget();
        }

        public override void Exit(Action onExit)
        {
            _ctSource.Cancel();
            base.Exit(onExit);
        }

        private async UniTaskVoid StartGameLoop(CancellationToken ct)
        {
            while (ct.IsCancellationRequested == false)
            {
                await Fields.GameView.StartGame(Fields.PickedLevel.LevelData);
                StateSwitcher.SwitchState<GsSelectLevelState>();
            }
        }
    }
}