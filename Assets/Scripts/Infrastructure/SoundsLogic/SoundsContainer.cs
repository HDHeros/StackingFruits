using HDH.Audio.Confgis;
using UnityEngine;
using Utils;

namespace Infrastructure.SoundsLogic
{
    [CreateAssetMenu(menuName = "Configs/Audio/SoundsContainer", fileName = "SoundsConfig", order = 0)]
    public class SoundsContainer : ScriptableObject
    {
        public KeyValuePair<EventId, AudioConfig>[] Pairs;
    }
}