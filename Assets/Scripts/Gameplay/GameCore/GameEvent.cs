using System.Collections.Generic;

namespace Gameplay.GameCore
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