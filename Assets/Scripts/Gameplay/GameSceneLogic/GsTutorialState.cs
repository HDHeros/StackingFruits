﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameStructConfigs;
using HDH.Popups;
using I2.Loc;
using UI;
using UI.Popups.Confirmation;
using UI.Screens;

namespace Gameplay.GameSceneLogic
{
    public class GsTutorialState : GsBaseState
    {
        private CancellationTokenSource _ctSource;
        private IGameCounterView _counterDummy;

        public override void OnFieldsReceived() => 
            _counterDummy = new GameCounterDummy();

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
            await ShowInfoPopup(LocalizationManager.GetTranslation("TUTOR1_STEP1"), LocalizationManager.GetTranslation("OK"), ct);
            await StartLevel(Fields.TutorInfo.Config.Level1, false);            
            
            await ShowInfoPopup(LocalizationManager.GetTranslation("TUTOR1_STEP2"), LocalizationManager.GetTranslation("OK"), ct);
            await StartLevel(Fields.TutorInfo.Config.Level2, false);
            await StartLevel(Fields.TutorInfo.Config.Level3, false);
            
            await ShowInfoPopup(LocalizationManager.GetTranslation("TUTOR1_STEP3"), LocalizationManager.GetTranslation("OK"), ct);
            await StartLevel(Fields.TutorInfo.Config.Level4, false);
            await StartLevel(Fields.TutorInfo.Config.Level5, false);
            
            await ShowInfoPopup(LocalizationManager.GetTranslation("TUTOR1_STEP4"), LocalizationManager.GetTranslation("OK"), ct);
            await StartLevel(Fields.TutorInfo.Config.Level6, false);
            
            await ShowInfoPopup(LocalizationManager.GetTranslation("TUTOR1_STEP4_1"), LocalizationManager.GetTranslation("OK"), ct);
            await StartLevel(Fields.TutorInfo.Config.Level7, true);
            
            await ShowInfoPopup(LocalizationManager.GetTranslation("TUTOR1_STEP5"), LocalizationManager.GetTranslation("OK"), ct);
            await StartLevel(Fields.TutorInfo.Config.Level8, true);

            Fields.TutorInfo.IsTutorCompleted = true;
            StateSwitcher.SwitchState<GsHomeScreenState>();
        }

        private async UniTask StartLevel(LevelConfig config, bool damageNumbersEnabled)
        {
            GameView.GameResult result = default;
            while (result.IsWin == false)
            {
                result = await Fields.GameView.StartGame(config.GetLevelData(), _counterDummy, damageNumbersEnabled);
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
                () => Fields.CameraController.ActivateInGameCamera());
        }
    }
}