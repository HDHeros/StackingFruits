using GameStructConfigs;
using UnityEngine;

namespace Infrastructure.Tutor
{
    [CreateAssetMenu(menuName = "Configs/TutorConfig", fileName = "TutorConfig", order = 0)]
    public class TutorConfig : ScriptableObject
    {
        public LevelConfig Level1;
        public LevelConfig Level2;
        public LevelConfig Level3;
        public LevelConfig Level4;
        public LevelConfig Level5;
        public LevelConfig Level6;
        public LevelConfig Level7;
        public LevelConfig Level8;
    }
}