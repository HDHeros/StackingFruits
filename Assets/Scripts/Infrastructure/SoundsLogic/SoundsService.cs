using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HDH.Audio;
using HDH.Audio.Confgis;
using UnityEngine;

namespace Infrastructure.SoundsLogic
{
    public class SoundsService
    {
        private readonly AudioService _audioService;
        private readonly Dictionary<EventId, AudioConfig> _configs;

        public SoundsService(SoundsContainer container, AudioService audioService)
        {
            _audioService = audioService;
            _configs = new Dictionary<EventId, AudioConfig>();
            foreach (var pair in container.Pairs) 
                _configs.Add(pair.Key, pair.Value);
        }

        public AudioSource RaiseEvent(EventId id)
        {
            if (TryGetConfig(id, out var config) == false) return null;
            return _audioService.Player.PlaySound(config);
        }

        public void Raise3DEvent(EventId id, Transform client, AudioService.Priority priority = AudioService.Priority.Default)
        {
            if (TryGetConfig(id, out var config) == false) return;
            _audioService.Player3D.Play(config, client, priority);
        }

        public UniTask RaiseEventAndAwaitFinish(EventId id, CancellationToken token)
        {
            if (TryGetConfig(id, out var config) == false) return UniTask.CompletedTask;
            AudioSource source = _audioService.Player.PlaySound(config);
            return UniTask.WaitWhile(() => source.isPlaying, cancellationToken: token);
        }

        private bool TryGetConfig(EventId id, out AudioConfig config)
        {
            if (_configs.TryGetValue(id, out config)) return true;
            Debug.LogWarning($"There is no configs configs for '{id.ToString()}' event");
            return false;
        }
        
        public void Pause()
        {
            _audioService.SetPauseGroup(SoundGroupNames.SoundsGroup, true);
        }

        public void Unpause()
        {
            _audioService.SetPauseGroup(SoundGroupNames.SoundsGroup, false);
        }
    }
}