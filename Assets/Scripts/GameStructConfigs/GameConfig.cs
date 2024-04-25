using HDH.Audio.Confgis;
using Infrastructure.SoundsLogic;
using Infrastructure.Tutor;
using UnityEngine;

namespace GameStructConfigs
{
    [CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public SectionConfig[] Sections;
        public AudioServiceConfig AudioServiceCfg;
        public SoundsContainer SoundsContainer;
        public AudioConfig BackgroundMusic;
        public TutorConfig TutorConfig;
    }
}