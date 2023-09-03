namespace PlayerInfoViewer.Models
{
    public class PlayerFullInfoJson
    {
        public string id { get; set; }
        public string name { get; set; }
        public string profilePicture { get; set; }
        public string bio { get; set; }
        public string country { get; set; }
        public float pp { get; set; }
        public int rank { get; set; }
        public int countryRank { get; set; }
        public string role { get; set; }
        public string histories { get; set; }
        public int permissions { get; set; }
        public bool banned { get; set; }
        public bool inactive { get; set; }
        public ScoreStats scoreStats { get; set; }
    }

    public class ScoreStats
    {
        public long totalScore { get; set; }
        public long totalRankedScore { get; set; }
        public float averageRankedAccuracy { get; set; }
        public int totalPlayCount { get; set; }
        public int rankedPlayCount { get; set; }
        public int replaysWatched { get; set; }
    }
}
