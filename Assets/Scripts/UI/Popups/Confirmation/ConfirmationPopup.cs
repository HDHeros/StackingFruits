using System;
using HDH.Popups;
using HDH.UnityExt.Extensions;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Popups.Confirmation
{
    public class ConfirmationPopup : PopupView
    {
        [SerializeField] private RectTransform _window;
        [SerializeField] private RectTransform _topPanel;
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private ConfirmationButtonWrapper _positiveBtn;
        [SerializeField] private ConfirmationButtonWrapper _negativeBtn;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _confirmationText;
        private GlobalVolumeService _globalVolume;

        [Inject]
        private void Inject(GlobalVolumeService globalVolume) => 
            _globalVolume = globalVolume;
        
        public void Setup(
            string headerText,
            string confirmationText,
            string positiveBtnText,
            string negativeBtnText,
            ConfirmationButtonWrapper.Style positiveBtnStyle,
            ConfirmationButtonWrapper.Style negativeBtnStyle,
            Action onPositiveCallback,
            Action onNegativeCallback,
            Action onClosedCallback)
        {
            _header.SetText(headerText);
            _confirmationText.SetText(confirmationText);
            AdjustWindowSizeByConfirmationText();
            _positiveBtn.SetText(positiveBtnText);
            _negativeBtn.SetText(negativeBtnText);
            _positiveBtn.SetStyle(positiveBtnStyle);
            _negativeBtn.SetStyle(negativeBtnStyle);
            
            _positiveBtn.Btn.onClick.AddListener(Close);
            _negativeBtn.Btn.onClick.AddListener(Close);
            _closeBtn.onClick.AddListener(Close);
            if (onPositiveCallback != null)
                _positiveBtn.Btn.onClick.AddListener(onPositiveCallback.Invoke);
            if (onNegativeCallback != null)
                _negativeBtn.Btn.onClick.AddListener(onNegativeCallback.Invoke);
            if (onClosedCallback != null)
                _closeBtn.onClick.AddListener(onClosedCallback.Invoke);
        }

        protected override void OnShown()
        {
            base.OnShown();
            _globalVolume.SetActiveBlur(true);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _globalVolume.SetActiveBlur(false);
        }

        protected override void ResetPopup()
        {
            _positiveBtn.Btn.onClick.RemoveAllListeners();
            _negativeBtn.Btn.onClick.RemoveAllListeners();
            _closeBtn.onClick.RemoveAllListeners();
        }
        
        [Sirenix.OdinInspector.Button]
        private void AdjustWindowSizeByConfirmationText()
        {
            _confirmationText.ForceMeshUpdate();
            _window.sizeDelta = _window.sizeDelta.WithY(_topPanel.sizeDelta.y + _bottomPanel.sizeDelta.y +
                                                        _confirmationText.textBounds.size.y);
        }
    }
}