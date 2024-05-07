using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Infrastructure.Pause;

namespace Infrastructure.AdLogic
{
    public abstract class AdService
    {
        protected readonly PauseService Pause;
        public event Action OnCountdownAdRequestReceived;

        public AdService(PauseService pauseService) => Pause = pauseService;

        public abstract void ShowAd();
        
        public virtual void ShowAdWithCountdown() => 
            OnCountdownAdRequestReceived?.Invoke();

        public abstract UniTask AwaitAdFinish();
    }
}