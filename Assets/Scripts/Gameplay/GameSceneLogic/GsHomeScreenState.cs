using System;
using HDH.Popups;
using I2.Loc;
using Lean.Touch;
using UI;
using UI.Popups.Confirmation;
using UI.Screens;

namespace Gameplay.GameSceneLogic
{
    public class GsHomeScreenState : GsBaseState
    {
        private HomeScreen _homeScreen;

        public override void Enter()
        {
            Fields.CameraController.ActivateHomeScreenCamera();
            _homeScreen = Fields.Hud.PushScreen<HomeScreen>(Hud.ScreenType.HomeScreen);
            _homeScreen.TutorButtonClick += OnTutorButtonClick;
            Fields.Input.OnTap += OnTap;
            Fields.TapToStartLabel.SetActive(true);
        }

        public override void Exit(Action onExit)
        {
            _homeScreen.TutorButtonClick += OnTutorButtonClick;
            Fields.Input.OnTap -= OnTap;
            Fields.Hud.PopScreen(Hud.ScreenType.HomeScreen);
            base.Exit(onExit);
        }

        private void OnTap(LeanFinger finger)
        {
            if (Fields.TutorInfo.IsTutorCompleted)
                StateSwitcher.SwitchState<GsSelectSectionState>();
            else
                StartTutorial();
        }

        private void OnTutorButtonClick()
        {
            Popup popup = Fields.Popups[typeof(ConfirmationPopup)];
            if (popup.View is ConfirmationPopup cPopup == false)
                throw new InvalidCastException();
            popup.Open();
            cPopup.Setup(
                LocalizationManager.GetTranslation("TUTOR_START_CONFIRMATION_HEADER"),
                LocalizationManager.GetTranslation("TUTOR_START_CONFIRMATION_TEXT"),
                LocalizationManager.GetTranslation("START"),
                LocalizationManager.GetTranslation("CANCEL"), 
                ConfirmationButtonWrapper.Style.Positive,
                ConfirmationButtonWrapper.Style.Black,
                StartTutorial,
                null,
                null
                );
        }

        private void StartTutorial() => 
            StateSwitcher.SwitchState<GsTutorialState>();
    }
}