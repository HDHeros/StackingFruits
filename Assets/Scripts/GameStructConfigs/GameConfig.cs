using HDH.Audio.Confgis;
using Infrastructure.SoundsLogic;
using UnityEngine;

namespace GameStructConfigs
{
    [CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        // public BlocksContainer BlocksContainer;
        public SectionConfig[] Sections;
        public AudioServiceConfig AudioServiceCfg;
        public SoundsContainer SoundsContainer;
    }
}