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
            Fields.Hud.PushScreen(Hud.ScreenType.CommonScreen);
            StartGameLoop(_ctSource.Token).Forget();
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
        }

        public override void Exit(Action onExit)
        {
            _ctSource.Cancel();
            Fields.Hud.PopScreen(Hud.ScreenType.CommonScreen);
            Fields.Input.OnBackButtonPressed -= OnBackButtonPressed;
            base.Exit(onExit);
        }

        private void OnBackButtonPressed()
        {
            PauseGame();
            ShowConfirmationPopup(
                LocalizationManager.GetTranslation("LEAVE_LEVEL_POPUP_HEADER"), 
                LocalizationManager.GetTranslation("LEAVE_LEVEL_POPUP_TEXT"), 
                LocalizationManager.GetTranslation("LEAVE_LEVEL_POPUP_BTN_POS"), 
                LocalizationManager.GetTranslation("LEAVE_LEVEL_POPUP_BTN_NEG"),
                ConfirmationButtonWrapper.Style.Negative,
                ConfirmationButtonWrapper.Style.Positive,
                FinishGame,
                ContinueGame,
                ContinueGame,
                _ctSource.Token).Forget();
        }

        private void PauseGame() => 
            Fields.GameView.Pause();

        private void ContinueGame()
        {
            Fields.CameraController.ActivateInGameCamera();
            Fields.GameView.Unpause();
        }

        private void FinishGame() => 
            Fields.GameView.ForceFinishGame();

        private async UniTaskVoid StartGameLoop(CancellationToken ct)
        {
            Fields.CameraController.ActivateInGameCamera();
            GameView.GameResult result = await Fields.GameView.StartGame(Fields.PickedLevel.LevelData);
            HandleFinishGame(result, ct);
        }

        private void HandleFinishGame(GameView.GameResult result, CancellationToken ct)
        {
            Action returnToSelectLevelState = () => ReturnToSelectLevelAsync(_ctSource.Token, result).Forget();
            if (result.IsWin || result.WasForceFinished)
            {
                returnToSelectLevelState.Invoke();
                return;
            }
            
            ShowConfirmationPopup(
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_HEADER"), 
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_TEXT"), 
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_BTN_RETRY"), 
                LocalizationManager.GetTranslation("LOSE_GAME_POPUP_BTN_BACK"),
                ConfirmationButtonWrapper.Style.Positive,
                ConfirmationButtonWrapper.Style.Negative,
                ReloadLevel,
                returnToSelectLevelState,
                returnToSelectLevelState,
                ct).Forget();
        }

        private async UniTaskVoid ShowConfirmationPopup(string headerText, string confirmationText,
            string positiveBtnText, string negativeBtnText, ConfirmationButtonWrapper.Style positiveBtnStyle,
            ConfirmationButtonWrapper.Style negativeBtnStyle, Action positiveCallback, Action negativeCallback,
            Action onClosedCallback, CancellationToken ct)
        {
            Fields.CameraController.ActivateGameLoseCamera();
            await UniTask.WaitWhile(() => Fields.CameraController.IsBlending, cancellationToken: ct);
            
            var view = Fields.Popups[typeof(ConfirmationPopup)].View;
            view.Show();
            if (view is ConfirmationPopup popup == false)
                throw new InvalidCastException();
            
            popup.Setup(
                headerText, 
                confirmationText, 
                positiveBtnText, 
                negativeBtnText,
                positiveBtnStyle,
                negativeBtnStyle,
                positiveCallback,
                negativeCallback,
                onClosedCallback);
        }

        private void ReloadLevel() => 
            StartGameLoop(_ctSource.Token).Forget();

        private async UniTaskVoid ReturnToSelectLevelAsync(CancellationToken ct, GameView.GameResult result)
        {
            Fields.CameraController.ActivateSelectLevelCamera();
            await UniTask.WaitWhile(() => Fields.CameraController.IsBlending, cancellationToken: ct);
            if (result.Progress > Fields.LevelsService.GetLevelProgress(Fields.PickedSection.Id, Fields.PickedLevel.Id))
            {
                Fields.LevelsService.SetLevelProgress(Fields.PickedSection.Id, Fields.PickedLevel.Id, result.Progress);
                Fields.PickedLevel.SetLevelProgress(result.Progress);
                if (result.IsWin)
                    Fields.SectionPicker.CheckNextSectionAvailability(Fields.PickedSection);
            }
            StateSwitcher.SwitchState<GsSelectLevelState>();
        }
    }
}