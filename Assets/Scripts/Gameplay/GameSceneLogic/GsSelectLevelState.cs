using System;
using HDH.UnityExt.Extensions;
using Menu;
using UI;
using UI.Screens;

namespace Gameplay.GameSceneLogic
{
    public class GsSelectLevelState : GsBaseState
    {
        private LevelSelectionScreen _uiScreen;

        public override void Enter()
        {
            Fields.CameraController.ActivateSelectLevelCamera();
            _uiScreen = Fields.Hud.PushScreen<LevelSelectionScreen>(Hud.ScreenType.LevelSelectionScreen);
            Fields.Input.OnBackButtonPressed += OnBackButtonPressed;
            Fields.PickedSection.OnLevelPreviewClick += OnLevelClicked;
            Fields.PickedLevel = null;
            base.Enter();
        }

        public override void Exit(Action onExit)
        {
            Fields.Hud.PopScreen(Hud.ScreenType.LevelSelectionScreen);
            Fields.Input.OnBackButtonPressed -= OnBackButtonPressed;
            Fields.PickedSection.OnLevelPreviewClick -= OnLevelClicked;
            _uiScreen.HideLevelInfo();
            if (Fields.PickedLevel.IsNotNull())
                Fields.PickedLevel.SetSelected(false);
            base.Exit(onExit);
        }

        private void OnBackButtonPressed()
        {
            if (Fields.SectionPicker.UnpickSelected() == false) return;
            StateSwitcher.SwitchState<GsSelectSectionState>();
        }

        private void OnLevelClicked(LevelPreview preview)
        {
            if (Fields.PickedLevel == preview)
            {
                OnStartLevelClick();
                return;
            }
            if (Fields.PickedLevel.IsNotNull())
                Fields.PickedLevel.SetSelected(false);
            Fields.PickedLevel = preview;
            Fields.PickedLevel.SetSelected(true);
            _uiScreen.ShowLevelInfo(Fields.LevelsService.GetLevelScore(Fields.PickedSection.Id, Fields.PickedLevel.Id), OnStartLevelClick);
        }

        private void OnStartLevelClick()
        {
            StateSwitcher.SwitchState<GsGameInProgressState>();
        }
    }
}