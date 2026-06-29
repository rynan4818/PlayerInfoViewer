namespace PlayerInfoViewer.Models.ScoreSaber
{
    public class PlayerProfileV2Json
    {
        public string id { get; set; }
        public PlayerStatsV2Json stats { get; set; }
    }

    public class PlayerStatsV2Json
    {
        public int rank { get; set; }
        public int countryRank { get; set; }
        public double totalPP { get; set; }
        public string totalScore { get; set; }
        public string totalRankedScore { get; set; }
        public int totalPlayedLeaderboards { get; set; }
        public int totalPlayedRankedLeaderboards { get; set; }
        public int totalReplayViews { get; set; }
        public double averageAccuracy { get; set; }
    }
}
