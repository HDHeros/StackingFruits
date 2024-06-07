using Infrastructure.SimpleInput;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class CommonScreen : BaseScreen
    {
        [SerializeField] private Button _backButton;
        private InputService _inputService;

        [Inject]
        private void Inject(InputService inputService) =>
            _inputService = inputService;
        
        protected virtual void Start()
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked() => 
            _inputService.RaiseOnBackButtonClicked();
    }
}