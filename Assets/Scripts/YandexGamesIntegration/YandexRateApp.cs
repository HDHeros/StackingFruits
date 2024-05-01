using System;
using HDH.Popups;
using I2.Loc;
using Infrastructure.RateAppLogic;
using UI.Popups.Confirmation;
using YG;

namespace YandexGamesIntegration
{
    public class YandexRateApp : RateAppDummy
    {
        private readonly PopupsController _popups;

        public YandexRateApp(PopupsController popups) : base(popups) => 
            _popups = popups;

        public override bool IsRateAvailable() => 
            YandexGame.EnvironmentData.reviewCanShow;

        public override void RateApp() => 
            ShowRateAppPopup();

        protected override void OnPositiveBtnClick()
        {
            if (YandexGame.auth == false)
            {
                var popup = _popups[typeof(ConfirmationPopup)];
                popup.Open();
                if (popup.View is ConfirmationPopup confirmPopup == false)
                    throw new Exception();
                confirmPopup.Setup(
                    LocalizationManager.GetTranslation("AUTH"),
                    LocalizationManager.GetTranslation("RATE_APP_AUTH_TEXT"),
                    LocalizationManager.GetTranslation("LOG_IN"),
                    LocalizationManager.GetTranslation("NOT_NOW"),
                    ConfirmationButtonWrapper.Style.Positive,
                    ConfirmationButtonWrapper.Style.Negative,
                    OpenRateUs,
                    null,
                    null
                );
            }
            else
                OpenRateUs();
        }

        private void OpenRateUs()
        {
            YandexGame.ReviewShow(true);
        }
    }
}