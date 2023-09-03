using Newtonsoft.Json;
using PlayerInfoViewer.Util;
using System.Threading.Tasks;
using System;

namespace PlayerInfoViewer.Models
{
    public class ScoreSaberPlayerInfo
    {
        public bool _playerInfoGetActive = false;
        public PlayerFullInfoJson _playerFullInfo;
        public async Task GetPlayerFullInfoAsync(string userID)
        {
            if (userID == null || this._playerInfoGetActive)
                return;
            this._playerInfoGetActive = true;
            this._playerFullInfo = null;
            var playerFullInfoURL = $"https://scoresaber.com/api/player/{userID}/full";
            try
            {
                var resJsonString = await HttpUtility.GetHttpContentAsync(playerFullInfoURL);
                if (resJsonString == null)
                    throw new Exception("Player full info get error");
                this._playerFullInfo = JsonConvert.DeserializeObject<PlayerFullInfoJson>(resJsonString);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                this._playerInfoGetActive = false;
                return;
            }
            this._playerInfoGetActive = false;
            return;
        }
    }
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
