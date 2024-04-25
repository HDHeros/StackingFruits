using DG.Tweening;
using HDH.Audio;
using HDH.Popups;
using HDH.UnityExt.Extensions;
using Infrastructure;
using Infrastructure.SoundsLogic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Popups.Settings
{
    public class SettingsPopup : PopupView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private RectTransform _content;
        [SerializeField] private CanvasGroup _contentCanvasGroup;
        private AudioService _audioService;
        private GlobalVolumeService _globalVolume;
        private Hud _hud;
        private Tween _appearingSequence;

        [Inject]
        private void Inject(AudioService audioService, GlobalVolumeService globalVolume, Hud hud)
        {
            _audioService = audioService;
            _globalVolume = globalVolume;
            _hud = hud;
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
        
        private void CustomClose()
        {
            _contentCanvasGroup.interactable = false;
            
            _appearingSequence?.Kill();
            _appearingSequence = DOTween.Sequence()
                .Append(_content.DOScale(1.3f, 0.1f).SetEase(Ease.InQuart))
                .Join(_contentCanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.InQuart))
                .AppendCallback(Close);
        }
        
        private void OnEnable()
        {
            _closeButton.onClick.AddListener(CustomClose);
            _musicSlider.onValueChanged.AddListener(OnMusicToggleChanged);
            _soundSlider.onValueChanged.AddListener(OnSoundToggleChanged);
            _musicSlider.SetValueWithoutNotify(GetMusicVolume());
            _soundSlider.SetValueWithoutNotify(GetSoundVolume());
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(CustomClose);
            _musicSlider.onValueChanged.RemoveListener(OnMusicToggleChanged);
            _soundSlider.onValueChanged.RemoveListener(OnSoundToggleChanged);
        }

        private void OnMusicToggleChanged(float value) => 
            _audioService.SetGroupVolumeByName(SoundGroupNames.MusicGroup, EaseExtension.OutCubic(value));

        private void OnSoundToggleChanged(float value) => 
            _audioService.SetGroupVolumeByName(SoundGroupNames.SoundsGroup, EaseExtension.OutCubic(value));
        
        private float GetMusicVolume() => 
            1 - Mathf.Pow(1 - _audioService.GetGroupVolume(SoundGroupNames.MusicGroup) , 1f/3);

        private float GetSoundVolume() => 
            1 - Mathf.Pow(1 - _audioService.GetGroupVolume(SoundGroupNames.SoundsGroup) , 1f/3);
    }
}