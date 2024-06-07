using System;
using Gameplay.LevelsLogic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class LevelSelectionScreen : CommonScreen
    {
        [SerializeField] private LocalizationParamsManager _bestResultLabelParams;
        [SerializeField] private Button _startLevelButton;
        [SerializeField] private CanvasGroup _levelInfoCanvasGroup;
        private Action _startLevelCallback;
        private LevelsService _levels;

        public void ShowLevelInfo(int levelScoreRecord, Action startLevelCallback)
        {
            _levelInfoCanvasGroup.alpha = 1;
            _levelInfoCanvasGroup.interactable = true;
            _bestResultLabelParams.SetParameterValue("VALUE", levelScoreRecord.ToString());
            _startLevelCallback = startLevelCallback;
        }

        public void HideLevelInfo()
        {
            _levelInfoCanvasGroup.alpha = 0;
            _levelInfoCanvasGroup.interactable = false;
        }
        
        protected override void Start()
        {
            base.Start();
            HideLevelInfo();
            _startLevelButton.onClick.AddListener(OnStartLevelClick);
        }

        private void OnDisable() => 
            _startLevelCallback = null;

        private void OnStartLevelClick() => 
            _startLevelCallback?.Invoke();
    }
}