using System;
using Gameplay.LevelsLogic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class HomeScreen : BaseScreen
    {
        public event Action SettingsButtonClick;
        public event Action TutorButtonClick;
        
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _tutorStartButton;
        [SerializeField] private LocalizationParamsManager _totalScoreLabelParams;

        public void SetTotalScore(int value)
        {
            _totalScoreLabelParams.SetParameterValue("VALUE", value.ToString());
        }

        private void Awake()
        {
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _tutorStartButton.onClick.AddListener(OnTutorButtonClick);
        }

        private void OnTutorButtonClick() => 
            TutorButtonClick?.Invoke();

        private void OnSettingsButtonClick() => 
            SettingsButtonClick?.Invoke();
    }
}