using System;
using UI;

namespace Gameplay.GameSceneLogic
{
    public class GsSelectSectionState : GsBaseState
    {
        public override void Enter()
        {
            Fields.Hud.ActivateScreen(Hud.ScreenType.CommonScreen);
            Fields.CameraController.ActivateSelectSectionCamera();
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
        }

        public override void Exit(Action onExit)
        {
            Fields.Input.OnBackButtonPressed -= OnBackButtonPressed;
            base.Exit(onExit);
        }

        private void OnBackButtonPressed()
        {
            StateSwitcher.SwitchState<GsHomeScreenState>();
        }
    }
}