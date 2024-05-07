using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HDH.Popups;
using I2.Loc;
using Infrastructure;
using Infrastructure.AdLogic;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Popups.AdWarning
{
    public class AdWarningPopup : PopupView
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private CanvasGroup _contentCanvasGroup;
        [SerializeField] private LocalizationParamsManager _countdown;
        [SerializeField] private RectTransform _textTransform;
        private GlobalVolumeService _globalVolume;
        private Hud _hud;
        private Tween _appearingSequence;
        private AdService _adService;

        [Inject]
        private void Inject(GlobalVolumeService globalVolume, Hud hud, AdService adService)
        {
            _globalVolume = globalVolume;
            _hud = hud;
            _adService = adService;
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
                .AppendCallback(() =>
                {
                    _contentCanvasGroup.interactable = true;
                    StartCountdown().Forget();
                });
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _hud.PopScreen(Hud.ScreenType.EmptyScreen);
            _globalVolume.SetActiveBlur(false);
        }

        private async UniTaskVoid StartCountdown()
        {
            float countdownInterval = 0.85f;
            for (int i = 3; i >= 0; i--)
            {
                _countdown.SetParameterValue("VALUE", i.ToString());
                await UniTask.Delay(TimeSpan.FromSeconds(countdownInterval));
            }
            _adService.ShowAd();
            AnimateAwaiting();
            await _adService.AwaitAdFinish();
            Close();
        }

        private void AnimateAwaiting()
        {
            _textTransform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        protected override void ResetPopup()
        {
            _countdown.SetParameterValue("VALUE", 3.ToString());
            _textTransform.DOKill();
            _textTransform.localScale = Vector3.one;
        }

        private void CustomClose()
        {
            _contentCanvasGroup.interactable = false;
            _appearingSequence?.Kill();
            _appearingSequence = DOTween.Sequence()
                .Append(_content.DOScale(1.3f, 0.1f).SetEase(Ease.InQuart))
                .Join(_contentCanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.InQuart))
                .AppendCallback(Close);
        }
    }
}