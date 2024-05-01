using System;
using DG.Tweening;
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
        [SerializeField] private RectTransform _content;
        [SerializeField] private CanvasGroup _contentCanvasGroup;
        public bool IsClosing { get; private set; }
        private GlobalVolumeService _globalVolume;
        private Hud _hud;
        private Tween _appearingSequence;

        [Inject]
        private void Inject(GlobalVolumeService globalVolume, Hud hud)
        {
            _globalVolume = globalVolume;
            _hud = hud;
        }

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
            
            if (onPositiveCallback != null)
                _positiveBtn.Btn.onClick.AddListener(onPositiveCallback.Invoke);
            if (onNegativeCallback != null)
                _negativeBtn.Btn.onClick.AddListener(onNegativeCallback.Invoke);
            if (onClosedCallback != null)
                _closeBtn.onClick.AddListener(onClosedCallback.Invoke);
            _positiveBtn.Btn.onClick.AddListener(CustomClose);
            _negativeBtn.Btn.onClick.AddListener(CustomClose);
            _closeBtn.onClick.AddListener(CustomClose);
        }

        protected override void OnShown()
        {
            base.OnShown();
            IsClosing = false;
            _globalVolume.SetActiveBlur(true);
            _hud.PushScreen(Hud.ScreenType.EmptyScreen);
            _content.localScale = Vector3.one * 1.3f;
            _contentCanvasGroup.alpha = 0f;
            _appearingSequence?.Kill();
            _appearingSequence = DOTween.Sequence()
                .Append(_content.DOScale(1f, 0.1f).SetEase(Ease.InQuart))
                .Join(_contentCanvasGroup.DOFade(1f, 0.1f).SetEase(Ease.InQuart))
                .AppendCallback(() => _contentCanvasGroup.interactable = true);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _hud.PopScreen(Hud.ScreenType.EmptyScreen);
            _globalVolume.SetActiveBlur(false);
        }

        protected override void ResetPopup()
        {
            _positiveBtn.Btn.onClick.RemoveAllListeners();
            _negativeBtn.Btn.onClick.RemoveAllListeners();
            _closeBtn.onClick.RemoveAllListeners();
        }

        private void CustomClose()
        {
            _contentCanvasGroup.interactable = false;
            IsClosing = true;
            
            _appearingSequence?.Kill();
            _appearingSequence = DOTween.Sequence()
                .Append(_content.DOScale(1.3f, 0.1f).SetEase(Ease.InQuart))
                .Join(_contentCanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.InQuart))
                .AppendCallback(Close);
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