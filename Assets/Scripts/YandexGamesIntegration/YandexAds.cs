using System;
using System.Threading;
using HDH.Popups;
using Infrastructure.AdLogic;
using Infrastructure.Pause;
using Infrastructure.SceneManagement;
using UI.Popups.AdWarning;
using UnityEngine;
using YG;

namespace YandexGamesIntegration
{
    public class YandexAds : AdService, IDisposable
    {
        private readonly CancellationTokenSource _ctSource;
        private readonly SceneService _sceneService;
        private readonly PopupsController _popups;

        public YandexAds(PauseService pauseService, SceneService sceneService, PopupsController popups) : base(pauseService)
        {
            _ctSource = new CancellationTokenSource();
            _sceneService = sceneService;
            _popups = popups;
            YandexGame.OpenFullAdEvent += OnOpenFullAd;
            YandexGame.CloseFullAdEvent += OnCloseFullAd;
            _sceneService.SceneLoaded += OnSceneLoaded;
        }

        public void Dispose()
        {
            _ctSource?.Dispose();
            YandexGame.OpenFullAdEvent -= OnOpenFullAd;
            YandexGame.CloseFullAdEvent -= OnCloseFullAd;
        }

        private void OnSceneLoaded()
        {
            YandexGame.GameReadyAPI();
            _sceneService.SceneLoaded -= OnSceneLoaded;
        }

        public override void ShowAdWithCountdown()
        {
            float timeToAd = YandexGame.Instance.infoYG.fullscreenAdInterval - YandexGame.timerShowAd;
            if (timeToAd > 0)
            {
                Debug.Log($"Ad with countdown was rejected. Before new ad {timeToAd} seconds");
                return;
            }
            
            Pause.AdPause();
            base.ShowAdWithCountdown();
            _popups[typeof(AdWarningPopup)].Open();
        }

        public override void ShowAd()
        {
            YandexGame.FullscreenShow();
        }
        
        private void OnOpenFullAd()
        {
            Pause.AdPause();
        }

        private void OnCloseFullAd()
        {
            Pause.AdUnpause();
        }
    }
}