using System;
using Lean.Touch;
using UI;
using UnityEngine;

namespace Gameplay.GameSceneLogic
{
    public class GsSelectSectionState : GsBaseState
    {
        public override void OnFieldsReceived()
        {
            Fields.SectionPicker.Initialize(Fields.GameConfig.Sections, Fields.Pool);
        }

        public override void Enter()
        {
            Fields.Hud.ActivateScreen(Hud.ScreenType.CommonScreen);
            Fields.CameraController.ActivateSelectSectionCamera();
            Fields.SectionPicker.Show(true);
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
            Fields.Input.OnSwipe += OnSwipe;
            Fields.Input.OnTap += OnTap;
        }

        public override void Exit(Action onExit)
        {
            Fields.Input.OnBackButtonPressed -= OnBackButtonPressed;
            Fields.Input.OnSwipe -= OnSwipe;
            Fields.Input.OnTap -= OnTap;
            base.Exit(onExit);
        }

        private void OnSwipe(Vector2Int direction)
        {
            if (direction.x < 0)
                Fields.SectionPicker.SelectNext();
            if (direction.x > 0)
                Fields.SectionPicker.SelectPrev();
        }

        private void OnBackButtonPressed()
        {
            StateSwitcher.SwitchState<GsHomeScreenState>();
            Fields.SectionPicker.Hide(true);
        }

        private void OnTap(LeanFinger finger)
        {
            if (Fields.SectionPicker.PickSelected(out Fields.PickedSection) == false) return;
            StateSwitcher.SwitchState<GsSelectLevelState>();
        }
    }
}