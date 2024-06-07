using UnityEngine;

namespace Infrastructure.LeaderboardLogic
{
    public class LeaderboardDummy : Leaderboard
    {
        public override void SetValue(int value)
        {
            Debug.Log($"Leaderboard set value request: {value}");
        }
    }
}