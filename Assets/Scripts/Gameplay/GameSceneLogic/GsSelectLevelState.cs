using System;
using Menu;

namespace Gameplay.GameSceneLogic
{
    public class GsSelectLevelState : GsBaseState
    {
        public override void Enter()
        {
            Fields.CameraController.ActivateSelectLevelCamera();
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
            Fields.PickedSection.OnLevelPreviewClick += OnLevelClicked;
            base.Enter();
        }

        public override void Exit(Action onExit)
        {
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