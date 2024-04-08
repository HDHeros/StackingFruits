using System;
using Lean.Touch;
using UI;

namespace Gameplay.GameSceneLogic
{
    public class GsHomeScreenState : GsBaseState
    {
        public override void Enter()
        {
            Fields.CameraController.ActivateHomeScreenCamera();
            Fields.Hud.ActivateScreen(Hud.ScreenType.HomeScreen);
            Fields.Input.OnTap += OnTap;
        }

        public override void Exit(Action onExit)
        {
            Fields.Input.OnTap -= OnTap;
            base.Exit(onExit);
        }

        private void OnTap(LeanFinger finger)
        {
            StateSwitcher.SwitchState<GsSelectSectionState>();
        }
    }
}