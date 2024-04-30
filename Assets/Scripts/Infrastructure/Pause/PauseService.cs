using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.SoundsLogic;
using UnityEngine;
using Zenject;

namespace Infrastructure.Pause
{
    public class PauseService
    {
        private readonly MusicPlayer _music;
        private readonly SoundsService _sounds;
        private readonly List<IPauseHandler> _handlers = new List<IPauseHandler>();
        private bool _isPaused;

        public bool IsPaused => _isPaused;
        public float TimeSinceLevelLoadWithoutPauseTime => Time.timeSinceLevelLoad - _totalOnPauseTime;
        public float DeltaTime => _isPaused ? 0 : Time.deltaTime;
        public float FixedDeltaTime => _isPaused ? 0 : Time.fixedDeltaTime;
        
        private float _totalOnPauseTime;
        private float _pauseStartTime;
        private long _lastPauseStartTime;

        [Inject]
        public PauseService(MusicPlayer music, SoundsService sounds)
        {
            _music = music;
            _sounds = sounds;
        }
        
        public void Register(IPauseHandler handler)
        {
            _handlers.Add(handler);
        }

        public void UnRegister(IPauseHandler handler)
        {
            _handlers.Remove(handler);
        }

        public void Pause()
        {
            if (_isPaused) return;
            _isPaused = true;
            _pauseStartTime = Time.timeSinceLevelLoad;
            _lastPauseStartTime = DateTime.UtcNow.Ticks;
            _handlers.ForEach(h => h.SetPaused(true));
        }

        public void Continue()
        {
            if (_isPaused == false) return;
            CalculateOnPauseTime();
            _isPaused = false;
            foreach (var h in _handlers.ToList())
            {
                h.SetPaused(false);
            }
        }

        public void AdPause()
        {
            Pause();
            _music.Pause();
            _sounds.Pause();
        }

        public void AdUnpause()
        {
            Continue();
            _music.Unpause();
            _sounds.Unpause();
        }
        
        public void SwitchPauseState()
        {
            if (_isPaused) Continue();
            else Pause();
        }

        public float GetTimeInLastPause() => 
            (float)(DateTime.UtcNow.Ticks - _lastPauseStartTime) / TimeSpan.TicksPerSecond;

        private void CalculateOnPauseTime()
        {
            _totalOnPauseTime += Time.timeSinceLevelLoad - _pauseStartTime;
        }
    }
}
