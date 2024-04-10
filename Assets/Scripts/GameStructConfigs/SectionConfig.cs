using Menu;
using UnityEngine;

namespace GameStructConfigs
{
    [CreateAssetMenu(menuName = "Configs/Game/Section", fileName = "SectionConfig", order = 0)]
    public class SectionConfig : ScriptableObject
    {
        public SectionId Id = SectionId.None;
        public SectionView ViewPrefab;
        public LevelConfig[] Levels;
    }
    
    public enum SectionId
    {
        None = 0, 
        Section1 = 1, 
        Section2 = 2, 
        Section3 = 3, 
        Section4 = 4, 
        Section5 = 5,
    }
}