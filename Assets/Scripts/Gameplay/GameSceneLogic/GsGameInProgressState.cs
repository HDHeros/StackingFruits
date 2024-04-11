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
                GameView.GameResult result = await Fields.GameView.StartGame(Fields.PickedLevel.LevelData);

                Fields.CameraController.ActivateSelectLevelCamera();
                await UniTask.WaitWhile(() => Fields.CameraController.IsBlending, cancellationToken: ct);

                if (result.Progress > Fields.LevelsService.GetLevelProgress(Fields.PickedSection.Id, Fields.PickedLevel.Id))
                {
                    Fields.LevelsService.SetLevelProgress(Fields.PickedSection.Id, Fields.PickedLevel.Id, result.Progress);
                    Fields.PickedLevel.SetLevelProgress(result.Progress);
                }
                StateSwitcher.SwitchState<GsSelectLevelState>();
            }
        }
    }
}