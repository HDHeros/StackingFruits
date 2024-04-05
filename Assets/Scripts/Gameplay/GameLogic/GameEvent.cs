using System.Collections.Generic;

namespace Gameplay.GameLogic
{
    public struct GameEvent
    {
        public readonly GameEventType Type;
        public readonly IReadOnlyList<PerformedMovement> Actions;
                
        public GameEvent(GameEventType type, IReadOnlyList<PerformedMovement> actions)
        {
            Type = type;
            Actions = actions;
        }
    }
}