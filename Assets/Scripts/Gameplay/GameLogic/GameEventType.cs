namespace Gameplay.GameLogic
{
    public enum GameEventType
    {
        None = 0, 
        BlockMovedByUser = 1, 
        BlocksFell = 2,
        StackPerformed = 3,
        GameWon = 4,
        GameLost = 5,
    }
}