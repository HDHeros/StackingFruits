using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using I2.Loc;
using UI;
using UI.Popups.Confirmation;

namespace Gameplay.GameSceneLogic
{
    public class GsGameInProgressState : GsBaseState
    {
        private CancellationTokenSource _ctSource;

        public override void Enter()
        {
            _ctSource = new CancellationTokenSource();
            Fields.TapToStartLabel.SetActive(false);
            Fields.Hud.ActivateScreen(Hud.ScreenType.CommonScreen);
            StartGameLoop(_ctSource.Token).Forget();
        }

        public override void Exit(Action onExit)
        {
            _ctSource.Cancel();
            base.Exit(onExit);
        }

        private async UniTaskVoid StartGameLoop(CancellationToken ct)
        {
            Fields.CameraController.ActivateInGameCamera();
            GameView.GameResult result = await Fields.GameView.StartGame(Fields.PickedLevel.LevelData);
            HandleFinishGame(result, ct).Forget();
        }

        private async UniTaskVoid HandleFinishGame(GameView.GameResult result, CancellationToken ct)
        {
            if (result.Progress > Fields.LevelsService.GetLevelProgress(Fields.PickedSection.Id, Fields.PickedLevel.Id))
            {
                Fields.LevelsService.SetLevelProgress(Fields.PickedSection.Id, Fields.PickedLevel.Id, result.Progress);
                Fields.PickedLevel.SetLevelProgress(result.Progress);
            }
            
            Action returnToSelectLevelState = () => ReturnToSelectLevelAsync(_ctSource.Token).Forget();
            if (result.IsWin)
            {
                returnToSelectLevelState.Invoke();
                return;
            }
            
            Fields.CameraController.ActivateGameLoseCamera();
            await UniTask.WaitWhile(() => Fields.CameraController.IsBlending, cancellationToken: ct);
            
            var view = Fields.Popups[typeof(ConfirmationPopup)].View;
            view.Show();
            if (view is ConfirmationPopup popup == false)
                throw new InvalidCastException();

            
            popup.Setup(
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_HEADER"), 
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_TEXT"), 
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_BTN_RETRY"), 
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_BTN_BACK"),
                ConfirmationButtonWrapper.Style.Positive,
                ConfirmationButtonWrapper.Style.Black,
                ReloadLevel,
                returnToSelectLevelState,
                returnToSelectLevelState);
            Fields.Hud.ActivateScreen(Hud.ScreenType.EmptyScreen);
        }

        private void ReloadLevel() => 
            StartGameLoop(_ctSource.Token).Forget();

        private async UniTaskVoid ReturnToSelectLevelAsync(CancellationToken ct)
        {
            Fields.CameraController.ActivateSelectLevelCamera();
            await UniTask.WaitWhile(() => Fields.CameraController.IsBlending, cancellationToken: ct);
            StateSwitcher.SwitchState<GsSelectLevelState>();
        }
    }
}