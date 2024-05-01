using HDH.Popups;
using Infrastructure.Pause;
using UI.Popups.AdWarning;

namespace Infrastructure.AdLogic
{
    public class AdServiceDummy : AdService
    {
        private readonly PopupsController _popups;

        public AdServiceDummy(PauseService pauseService, PopupsController popups) : base(pauseService)
        {
            _popups = popups;
        }

        public override void ShowAd()
        {
            Pause.AdUnpause();
        }

        public override void ShowAdWithCountdown()
        {
            Pause.AdPause();
            base.ShowAdWithCountdown();
            _popups[typeof(AdWarningPopup)].Open();
        }
    }
}