using UnityEngine;

namespace Gameplay.GameCore
{
    public struct PerformedMovement
    {
        public readonly Vector2Int From;
        public readonly Vector2Int To;

        public PerformedMovement(Vector2Int from, Vector2Int to)
        {
            From = from;
            To = to;
        }
    }
}