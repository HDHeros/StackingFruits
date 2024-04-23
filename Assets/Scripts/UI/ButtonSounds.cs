using Infrastructure.SoundsLogic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI
{
    public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private SoundsService _sounds;
        [SerializeField] private EventId _onEnter = EventId.ButtonPointerEnter;
        [SerializeField] private EventId _onExit = EventId.ButtonPointerExit;
        [SerializeField] private EventId _onClick = EventId.ButtonClick;

        [Inject]
        private void Inject(SoundsService sounds) => 
            _sounds = sounds;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_onEnter == EventId.None) return;
            _sounds.RaiseEvent(_onEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_onExit == EventId.None) return;
            _sounds.RaiseEvent(_onExit);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_onClick == EventId.None) return;
            _sounds.RaiseEvent(_onClick);
        }
    }
}