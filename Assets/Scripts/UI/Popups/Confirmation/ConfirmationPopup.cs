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
        private GlobalVolumeService _globalVolume;
        private Hud _hud;
        private Tween _appearingSequence;
        private Action _onPositiveCallback;
        private Action _onNegativeCallback;
        private Action _onClosedCallback;

        [Inject]
        private void Inject(GlobalVolumeService globalVolume, Hud hud)
        {
            _globalVolume = globalVolume;
            _hud = hud;
        }

        private void Start()
        {
            Closed += OnFinalClose;
        }

        private void OnFinalClose()
        {
            _onClosedCallback?.Invoke();
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
            
            _onPositiveCallback = onPositiveCallback;
            _onNegativeCallback = onNegativeCallback;
            _onClosedCallback = onClosedCallback;
            _positiveBtn.Btn.onClick.AddListener(OnPositiveClick);
            _negativeBtn.Btn.onClick.AddListener(OnNegativeClick);
            _closeBtn.onClick.AddListener(CustomClose);
        }

        protected override void OnShown()
        {
            base.OnShown();
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

        private void OnPositiveClick()
        {
            _onClosedCallback += _onPositiveCallback;
            CustomClose();
        }

        private void OnNegativeClick()
        {
            _onClosedCallback += _onNegativeCallback;
            CustomClose();
        }
        
        private void CustomClose()
        {
            _contentCanvasGroup.interactable = false;
            
            _positiveBtn.Btn.onClick.RemoveAllListeners();
            _negativeBtn.Btn.onClick.RemoveAllListeners();
            _closeBtn.onClick.RemoveAllListeners();
            
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