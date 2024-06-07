using DG.Tweening;
using Extensions;
using Infrastructure.SimpleInput;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class GameInProgressScreen : BaseScreen, IGameCounterView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _pointLabel;
        [SerializeField] private float _animationDurationPerPoint;
        [SerializeField] private Color _defaultLabelColor = Color.white;
        [SerializeField] private Color _counterRaiseColor = new((float)154 / 255, (float)210 / 255, (float)0 / 255, (float)255/255);
        [SerializeField] private Color _counterFallColor = new((float)224 / 255, (float)122 / 255, (float)0 / 255, (float)255/255);
        private InputService _inputService;
        private Coroutine _counterAnimationCoroutine;
        private int _currentPointerValue;
        private Tween _counterAnimationTween;

        [Inject]
        private void Inject(InputService inputService) =>
            _inputService = inputService;
        
        private void Start()
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked() => 
            _inputService.RaiseOnBackButtonClicked();

        public void SetPointsCount(int value, bool animated = false)
        {
            int prevValue = _currentPointerValue;
            _currentPointerValue = value;
            if (prevValue == _currentPointerValue)
            {
                UpdateView(false, prevValue);
                return;
            }
            UpdateView(animated, prevValue);
        }

        private void UpdateView(bool animated, int prevValue)
        {
            if (animated)
            {
                int diff = Mathf.Abs(_currentPointerValue - prevValue);
                if (_counterAnimationCoroutine != null)
                    StopCoroutine(_counterAnimationCoroutine);
                _counterAnimationTween?.Kill();
                var duration = _animationDurationPerPoint * diff;
                _counterAnimationCoroutine = StartCoroutine(
                    _pointLabel.DOCustomCounter(
                        prevValue, 
                        _currentPointerValue, 
                        duration - (_animationDurationPerPoint / 2), 
                        d => ((int)d).ToString()));
                bool isNewValueBigger = _currentPointerValue > prevValue;
                _pointLabel.transform.localScale = Vector3.one;
                _pointLabel.color = _defaultLabelColor;
                if (isNewValueBigger)
                    _counterAnimationTween = DOTween.Sequence()
                        .Append(_pointLabel.transform.DOScale(Vector3.one * 1.2f, _animationDurationPerPoint / 2))
                        .Join(_pointLabel.DOColor(_counterRaiseColor, duration / 2).SetLoops(2, LoopType.Yoyo))
                        .Append(_pointLabel.transform.DOScale(Vector3.one, _animationDurationPerPoint / 2));
                else
                    _counterAnimationTween = DOTween.Sequence()
                        .Append(_pointLabel.transform.DOScale(Vector3.one * 1.2f, _animationDurationPerPoint / 2))
                        .Join(_pointLabel.DOColor(_counterFallColor, duration / 2).SetLoops(2, LoopType.Yoyo).SetLoops(2, LoopType.Yoyo))
                        .Append(_pointLabel.transform.DOScale(Vector3.one, _animationDurationPerPoint / 2));
            }
            else
            {
                _pointLabel.SetText(_currentPointerValue.ToString());
            }
        }

        private void OnDisable()
        {
            if (_counterAnimationCoroutine != null)
                StopCoroutine(_counterAnimationCoroutine);
            _counterAnimationTween?.Kill();        }
    }

    public interface IGameCounterView
    {
        public void SetPointsCount(int value, bool animated = false);
    }

    public class GameCounterDummy : IGameCounterView
    {
        public void SetPointsCount(int value, bool animated = false) { }
    }
}