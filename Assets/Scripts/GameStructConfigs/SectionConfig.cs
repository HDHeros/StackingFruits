using Menu;
using UnityEngine;

namespace GameStructConfigs
{
    [CreateAssetMenu(menuName = "Configs/Game/Section", fileName = "SectionConfig", order = 0)]
    public class SectionConfig : ScriptableObject
    {
        public enum Type{None, Section1, Section2, Section3, Section4, Section5}

        public Type SectionType;
        public SectionView ViewPrefab;
        public LevelConfig[] Levels;
    }
}