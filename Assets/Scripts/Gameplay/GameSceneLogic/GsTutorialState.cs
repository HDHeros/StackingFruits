using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HDH.Popups;
using UI;
using UI.Popups.Confirmation;

namespace Gameplay.GameSceneLogic
{
    public class GsTutorialState : GsBaseState
    {
        private CancellationTokenSource _ctSource;

        public override void Enter()
        {
            _ctSource = new CancellationTokenSource();
            Fields.TapToStartLabel.SetActive(false);
            Fields.Hud.PushScreen(Hud.ScreenType.CommonScreen);
            Fields.CameraController.ActivateInGameCamera();
            StartTutorialLoop(_ctSource.Token).Forget();
        }

        public override void Exit(Action onExit)
        {
            _ctSource.Cancel();
            Fields.Hud.PopScreen(Hud.ScreenType.CommonScreen);
            base.Exit(onExit);
        }

        private async UniTaskVoid StartTutorialLoop(CancellationToken ct)
        {
            await ShowInfoPopup("Поставь фрукты друг на друга", "OK", ct);
            await Fields.GameView.StartGame(Fields.TutorInfo.Config.Level1Cfg.GetLevelData());
        }
        
        private async UniTask ShowInfoPopup(string text, 
            string btnText, CancellationToken ct)
        {
            Fields.CameraController.ActivateGameLoseCamera();
            await UniTask.WaitWhile(() => Fields.CameraController.IsBlending, cancellationToken: ct);

            Popup popup = Fields.Popups[typeof(ConfirmationPopup)];
            var view = popup.View;
            view.Show();
            if (view is ConfirmationPopup cPopup == false)
                throw new InvalidCastException();
            
            cPopup.Setup(
                "", 
                text, 
                "",
                btnText, 
                ConfirmationButtonWrapper.Style.None,
                ConfirmationButtonWrapper.Style.Positive,
                null,
                null,
                null);
            await UniTask.WaitWhile(() => popup.IsShown, cancellationToken: ct);
            Fields.CameraController.ActivateInGameCamera();
        }
    }
}