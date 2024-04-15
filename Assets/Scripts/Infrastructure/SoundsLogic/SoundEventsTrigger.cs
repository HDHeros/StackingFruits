using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure.SoundsLogic
{
    public class SoundEventsTrigger : MonoBehaviour
    {
        private SoundsService _sounds;

        [Inject]
        private void Inject(SoundsService sounds) => 
            _sounds = sounds;

        [Button]
        private void RaiseEvent(EventId id) => 
            _sounds.RaiseEvent(id);
    }
}