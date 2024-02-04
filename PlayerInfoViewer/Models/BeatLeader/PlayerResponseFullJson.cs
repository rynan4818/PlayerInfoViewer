using System.Collections.Generic;

namespace PlayerInfoViewer.Models.BeatLeader
{
    //API変更の影響を最小限にするため、使用しないプロパティはコメントアウトしています
    //また、コメントアウトした物はNULL仕様の確認がいい加減なので、確認してからコメントアウトを外してください
    //https://api.beatleader.xyz/swagger/index.html
    public class PlayerResponseFullJson
    {
        public string id { get; set; }
        //public string name { get; set; }
        //public string platform { get; set; }
        //public string avatar { get; set; }
        //public string country { get; set; }
        //public bool bot { get; set; }
        public float pp { get; set; }
        public int rank { get; set; }
        public int countryRank { get; set; }
        //public string role { get; set; }
        //public List<PlayerSocial> socials { get; set; }
        //public PatreonFeatures patreonFeatures { get; set; }
        //public ProfileSettings profileSettings { get; set; }
        //public List<ClanResponse> clans { get; set; }
        //public float accPp { get; set; }
        //public float passPp { get; set; }
        //public float techPp { get; set; }
        public PlayerScoreStats scoreStats { get; set; }
        //public float lastWeekPp { get; set; }
        //public int lastWeekRank { get; set; }
        //public int lastWeekCountryRank { get; set; }
        //public List<EventPlayer> eventsParticipating { get; set; }
        //public int mapperId { get; set; }
        //public bool banned { get; set; }
        //public bool inactive { get; set; }
        //public Ban banDescription { get; set; }
        //public string externalProfileUrl { get; set; }
        //public List<PlayerScoreStatsHistory> history { get; set; }
        //public List<Badge> badges { get; set; }
        //public List<object> pinnedScores { get; set; }
        //public List<PlayerChange> changes { get; set; }
    }
    public class PlayerSocial
    {
        //public int id { get; set; }
        //public string service { get; set; }
        //public string link { get; set; }
        //public string user { get; set; }
        //public string userId { get; set; }
        //public string playerId { get; set; }
    }
    public class PatreonFeatures
    {
        //public int id { get; set; }
        //public string bio { get; set; }
        //public string message { get; set; }
        //public string leftSaberColor { get; set; }
        //public string rightSaberColor { get; set; }
    }
    public class ProfileSettings
    {
        //public int id { get; set; }
        //public string bio { get; set; }
        //public string message { get; set; }
        //public string effectName { get; set; }
        //public string profileAppearance { get; set; }
        //public float hue { get; set; }
        //public float saturation { get; set; }
        //public object leftSaberColor { get; set; }
        //public object rightSaberColor { get; set; }
        //public object profileCover { get; set; }
        //public string starredFriends { get; set; }
        //public bool showBots { get; set; }
        //public bool showAllRatings { get; set; }
    }
    public class ClanResponse
    {
        //public int id { get; set; }
        //public string tag { get; set; }
        //public string color { get; set; }
    }
    public class PlayerScoreStats
    {
        //public int id { get; set; }
        //public long totalScore { get; set; }
        //public long totalUnrankedScore { get; set; }
        //public long totalRankedScore { get; set; }
        //public int lastScoreTime { get; set; }
        //public int lastUnrankedScoreTime { get; set; }
        //public int lastRankedScoreTime { get; set; }
        //public float averageRankedAccuracy { get; set; }
        //public float averageWeightedRankedAccuracy { get; set; }
        //public float averageUnrankedAccuracy { get; set; }
        //public float averageAccuracy { get; set; }
        //public float medianRankedAccuracy { get; set; }
        //public float medianAccuracy { get; set; }
        //public float topRankedAccuracy { get; set; }
        //public float topUnrankedAccuracy { get; set; }
        //public float topAccuracy { get; set; }
        //public float topPp { get; set; }
        //public float topBonusPP { get; set; }
        //public float topPassPP { get; set; }
        //public float topAccPP { get; set; }
        //public float topTechPP { get; set; }
        //public float peakRank { get; set; }
        //public int rankedMaxStreak { get; set; }
        //public int unrankedMaxStreak { get; set; }
        //public int maxStreak { get; set; }
        //public float averageLeftTiming { get; set; }
        //public float averageRightTiming { get; set; }
        public int rankedPlayCount { get; set; }
        //public int unrankedPlayCount { get; set; }
        public int totalPlayCount { get; set; }
        //public int rankedImprovementsCount { get; set; }
        //public int unrankedImprovementsCount { get; set; }
        //public int totalImprovementsCount { get; set; }
        //public int rankedTop1Count { get; set; }
        //public int unrankedTop1Count { get; set; }
        //public int top1Count { get; set; }
        //public int rankedTop1Score { get; set; }
        //public int unrankedTop1Score { get; set; }
        //public int top1Score { get; set; }
        //public float averageRankedRank { get; set; }
        //public float averageWeightedRankedRank { get; set; }
        //public float averageUnrankedRank { get; set; }
        //public float averageRank { get; set; }
        //public int sspPlays { get; set; }
        //public int ssPlays { get; set; }
        //public int spPlays { get; set; }
        //public int sPlays { get; set; }
        //public int aPlays { get; set; }
        //public string topPlatform { get; set; }
        //public int topHMD { get; set; }
        //public int dailyImprovements { get; set; }
        //public int authorizedReplayWatched { get; set; }
        //public int anonimusReplayWatched { get; set; }
        //public int watchedReplays { get; set; }
    }
    public class EventPlayer
    {
        //public int id { get; set; }
        //public int eventId { get; set; }
        //public string name { get; set; }
        //public string playerId { get; set; }
        //public string country { get; set; }
        //public int rank { get; set; }
        //public int countryRank { get; set; }
        //public float pp { get; set; }
    }
    public class Ban
    {
        //public int id { get; set; }
        //public string playerId { get; set; }
        //public string bannedBy { get; set; }
        //public string banReason { get; set; }
        //public int timeset { get; set; }
        //public int duration { get; set; }
    }
    public class PlayerScoreStatsHistory
    {
        //public int id { get; set; }
        //public int timestamp { get; set; }
        //public string playerId { get; set; }
        //public float pp { get; set; }
        //public int rank { get; set; }
        //public int countryRank { get; set; }
        //public long totalScore { get; set; }
        //public long totalUnrankedScore { get; set; }
        //public long totalRankedScore { get; set; }
        //public int lastScoreTime { get; set; }
        //public int lastUnrankedScoreTime { get; set; }
        //public int lastRankedScoreTime { get; set; }
        //public float averageRankedAccuracy { get; set; }
        //public float averageWeightedRankedAccuracy { get; set; }
        //public float averageUnrankedAccuracy { get; set; }
        //public float averageAccuracy { get; set; }
        //public float medianRankedAccuracy { get; set; }
        //public float medianAccuracy { get; set; }
        //public float topRankedAccuracy { get; set; }
        //public float topUnrankedAccuracy { get; set; }
        //public float topAccuracy { get; set; }
        //public float topPp { get; set; }
        //public float topBonusPP { get; set; }
        //public float peakRank { get; set; }
        //public int maxStreak { get; set; }
        //public float averageLeftTiming { get; set; }
        //public float averageRightTiming { get; set; }
        //public int rankedPlayCount { get; set; }
        //public int unrankedPlayCount { get; set; }
        //public int totalPlayCount { get; set; }
        //public int rankedImprovementsCount { get; set; }
        //public int unrankedImprovementsCount { get; set; }
        //public int totalImprovementsCount { get; set; }
        //public float averageRankedRank { get; set; }
        //public float averageWeightedRankedRank { get; set; }
        //public float averageUnrankedRank { get; set; }
        //public float averageRank { get; set; }
        //public int sspPlays { get; set; }
        //public int ssPlays { get; set; }
        //public int spPlays { get; set; }
        //public int sPlays { get; set; }
        //public int aPlays { get; set; }
        //public string topPlatform { get; set; }
        //public int topHMD { get; set; }
        //public int dailyImprovements { get; set; }
        //public int replaysWatched { get; set; }
        //public int watchedReplays { get; set; }
    }
    public class Badge
    {
        //public int id { get; set; }
        //public string description { get; set; }
        //public string image { get; set; }
        //public string link { get; set; }
        //public int timeset { get; set; }
        //public bool hidden { get; set; }
    }
    public class PlayerChange
    {
        //public int id { get; set; }
        //public int timestamp { get; set; }
        //public string playerId { get; set; }
        //public string oldName { get; set; }
        //public string newName { get; set; }
        //public string oldCountry { get; set; }
        //public string newCountry { get; set; }
        //public string changer { get; set; }
    }
}
