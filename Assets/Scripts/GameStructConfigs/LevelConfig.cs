using Gameplay.Blocks;
using Gameplay.GameCore;
using UnityEngine;

namespace GameStructConfigs
{
    [CreateAssetMenu(menuName = "Configs/Game/Level", fileName = "LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        public BlockView[] Level;
        public BlockView EmptyBlock;
        public Vector2Int FieldSize;

        public LevelData<BlockView> GetLevelData()
        {
            return new LevelData<BlockView>()
            {
                Blocks = Level,
                EmptyBlockValue = EmptyBlock,
                FieldSize = FieldSize,
            };
        }
    }
}