using HDH.UnityExt.Extensions;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Settings
{
    public class I2LanguageSwitcher : MonoBehaviour 
    {
        [SerializeField] private Button _nextLanguageButton;
        [SerializeField] private Button _prevLanguageButton;
        [SerializeField] private TextMeshProUGUI _languageLabel;

        private void OnEnable()
        {
            if (_nextLanguageButton.IsNotNull())
                _nextLanguageButton.onClick.AddListener(OnNextLanguageButtonClick);
            if (_prevLanguageButton.IsNotNull())
                _prevLanguageButton.onClick.AddListener(OnPreviousLanguageButtonClick);
            
            UpdateLanguageLabel();
        }

        private void OnDisable()
        {
            if (_nextLanguageButton.IsNotNull())
                _nextLanguageButton.onClick.RemoveListener(OnNextLanguageButtonClick);
            if (_prevLanguageButton.IsNotNull())
                _prevLanguageButton.onClick.RemoveListener(OnPreviousLanguageButtonClick);
        }
        
        private void OnNextLanguageButtonClick() => OnLanguageButtonClick(true);
        
        private void OnPreviousLanguageButtonClick() => OnLanguageButtonClick(false);
        
        private void OnLanguageButtonClick(bool isNextBtn)
        {
            var currentLang = LocalizationManager.CurrentLanguageCode;
            var langList = LocalizationManager.GetAllLanguagesCode();
            var currentLangId = langList.FindIndex(l => l.Equals(currentLang));
            var newLangCode = langList[(int)Mathf.Repeat(currentLangId + (isNextBtn ? 1 : -1), langList.Count)];
            LocalizationManager.SetLanguageAndCode(LocalizationManager.GetLanguageFromCode(newLangCode), newLangCode, Force: true);
            UpdateLanguageLabel();
        }
        
        private void UpdateLanguageLabel()
        {
            if (_languageLabel.IsNull()) return;
            _languageLabel.SetText(
                LocalizationManager.GetTranslation($"LANG_{LocalizationManager.CurrentLanguageCode}"));
        }
    }
}