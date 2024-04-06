using Gameplay.Blocks;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public BlocksContainer BlocksContainer;

        public void Initialize()
        {
            BlocksContainer.Initialize();
        }
    }
}