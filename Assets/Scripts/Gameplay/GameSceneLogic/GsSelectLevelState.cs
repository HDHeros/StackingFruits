using System;
using Menu;
using UI;

namespace Gameplay.GameSceneLogic
{
    public class GsSelectLevelState : GsBaseState
    {
        public override void Enter()
        {
            Fields.CameraController.ActivateSelectLevelCamera();
            Fields.Hud.PushScreen(Hud.ScreenType.CommonScreen);
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
            Fields.PickedSection.OnLevelPreviewClick += OnLevelClicked;
            base.Enter();
        }

        public override void Exit(Action onExit)
        {
            Fields.Hud.PopScreen(Hud.ScreenType.CommonScreen);
            Fields.Input.OnBackButtonPressed -= OnBackButtonPressed;
            Fields.PickedSection.OnLevelPreviewClick -= OnLevelClicked;
            base.Exit(onExit);
        }

        private void OnBackButtonPressed()
        {
            if (Fields.SectionPicker.UnpickSelected() == false) return;
            StateSwitcher.SwitchState<GsSelectSectionState>();
        }

        private void OnLevelClicked(LevelPreview preview)
        {
            Fields.PickedLevel = preview;
            StateSwitcher.SwitchState<GsGameInProgressState>();
        }
    }
}