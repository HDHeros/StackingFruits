using GameStructConfigs;
using UnityEngine;

namespace Infrastructure.Tutor
{
    [CreateAssetMenu(menuName = "Configs/TutorConfig", fileName = "TutorConfig", order = 0)]
    public class TutorConfig : ScriptableObject
    {
        public LevelConfig Level1Cfg;
    }
}