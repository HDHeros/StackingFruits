using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameStructConfigs;
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
            Fields.Hud.PushScreen(Hud.ScreenType.EmptyScreen);
            Fields.CameraController.ActivateInGameCamera();
            StartTutorialLoop(_ctSource.Token).Forget();
        }

        public override void Exit(Action onExit)
        {
            _ctSource.Cancel();
            Fields.Hud.PopScreen(Hud.ScreenType.EmptyScreen);
            base.Exit(onExit);
        }

        private async UniTaskVoid StartTutorialLoop(CancellationToken ct)
        {
            await ShowInfoPopup("Поставь фрукты друг на друга", "OK", ct);
            await StartLevel(Fields.TutorInfo.Config.Level1);            
            
            await ShowInfoPopup("Ставь рядом одинаковые фрукты, чтобы стакнуть их ", "OK", ct);
            await StartLevel(Fields.TutorInfo.Config.Level2);
            await StartLevel(Fields.TutorInfo.Config.Level3);
            
            await ShowInfoPopup("Фрукты упадут, если останутся висеть ", "OK", ct);
            await StartLevel(Fields.TutorInfo.Config.Level4);
            await StartLevel(Fields.TutorInfo.Config.Level5);
            
            await ShowInfoPopup("Фркуты стакаются по вертикали и по горизонтали", "OK", ct);
            await StartLevel(Fields.TutorInfo.Config.Level6);
            await StartLevel(Fields.TutorInfo.Config.Level7);
            
            await ShowInfoPopup("Игра заканчивается, когда сделан первый стак.", "OK", ct);
            await StartLevel(Fields.TutorInfo.Config.Level8);
            
            StateSwitcher.SwitchState<GsHomeScreenState>();
        }

        private async UniTask StartLevel(LevelConfig config)
        {
            GameView.GameResult result = default;
            while (result.IsWin == false)
            {
                result = await Fields.GameView.StartGame(config.GetLevelData());
            }
        }
        
        private async UniTask ShowInfoPopup(string text, 
            string btnText, CancellationToken ct)
        {
            Fields.CameraController.ActivateGameLoseCamera();

            Popup popup = Fields.Popups[typeof(ConfirmationPopup)];
            popup.Open();
            var view = popup.View;
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
            await UniTask.WaitUntil(() => cPopup.IsClosing, cancellationToken: ct);
            Fields.CameraController.ActivateInGameCamera();
        }
    }
}