using System.Collections.Generic;

namespace Gameplay.GameCore
{
    public struct GameEvent
    {
        public readonly GameEventType Type;
        public readonly IReadOnlyList<PerformedMovement> Actions;
        public readonly float GameProgress;
                
        public GameEvent(GameEventType type, IReadOnlyList<PerformedMovement> actions)
        {
            Type = type;
            Actions = actions;
            GameProgress = 0f;
        }
        
        public GameEvent(GameEventType type, IReadOnlyList<PerformedMovement> actions, float gameProgress)
        {
            Type = type;
            Actions = actions;
            GameProgress = gameProgress;
        }
    }
}