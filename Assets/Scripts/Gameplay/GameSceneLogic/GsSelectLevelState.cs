using System;

namespace Gameplay.GameSceneLogic
{
    public class GsSelectLevelState : GsBaseState
    {
        public override void Enter()
        {
            Fields.CameraController.ActivateSelectLevelCamera();
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
            base.Enter();
        }

        public override void Exit(Action onExit)
        {
            Fields.Input.OnBackButtonPressed -= OnBackButtonPressed;
            base.Exit(onExit);
        }

        private void OnBackButtonPressed()
        {
            if (Fields.SectionPicker.UnpickSelected() == false) return;
            StateSwitcher.SwitchState<GsSelectSectionState>();        
        }
    }
}