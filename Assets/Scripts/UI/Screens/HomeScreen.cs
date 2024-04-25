using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class HomeScreen : BaseScreen
    {
        public event Action SettingsButtonClick;
        public event Action TutorButtonClick;
        
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _tutorStartButton;

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