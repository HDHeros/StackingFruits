using Infrastructure.LeaderboardLogic;
using YG;

namespace YandexGamesIntegration
{
    public class YandexLeaderboard : Leaderboard
    {
        public override void SetValue(int value)
        {
            YandexGame.NewLeaderboardScores("TopScoreLb", value);
        }
    }
}