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
            Fields.Hud.PushScreen(Hud.ScreenType.HomeScreen);
            Fields.Input.OnTap += OnTap;
            Fields.TapToStartLabel.SetActive(true);
        }

        public override void Exit(Action onExit)
        {
            Fields.Input.OnTap -= OnTap;
            Fields.Hud.PopScreen(Hud.ScreenType.HomeScreen);
            base.Exit(onExit);
        }

        private void OnTap(LeanFinger finger)
        {
            if (Fields.TutorInfo.IsTutorCompleted)
                StateSwitcher.SwitchState<GsSelectSectionState>();
            else
                StateSwitcher.SwitchState<GsTutorialState>();
        }
    }
}