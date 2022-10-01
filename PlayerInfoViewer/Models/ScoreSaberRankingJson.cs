using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayerInfoViewer.Configuration;
using PlayerInfoViewer.Util;

namespace PlayerInfoViewer.Models
{
    public class ScoreSaberRankingJson
    {
        public static readonly HttpClient RankingHttpClient = new HttpClient();
        public ScoreSaberRankingIndexJson _rankingIndex;
        public ScoreSaberRankingDataJson _rankingData;
        public int? _userIDindex = null;
        public bool _getDataActive = false;
        public DateTime _dataGetTime = DateTime.Now.AddYears(-1);
        public async Task GetUserRanking(string userID)
        {
            if (this._getDataActive)
                return;
            if (this._rankingIndex != null && this._rankingIndex.NextUpdateTime > new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
                return;
            if (DateTime.Now - this._dataGetTime < new TimeSpan(0, 15, 0))
                return;
            this._getDataActive = true;
            var scoresaberURL = "https://rynan4818.github.io/ScoreSaberRanking/json/scoresaber_rank_index.json";
            var resJsonString = await Utility.GetHttpContent(RankingHttpClient, scoresaberURL);
            if (resJsonString == null)
            {
                this._getDataActive = false;
                return;
            }
            this._rankingIndex = JsonConvert.DeserializeObject<ScoreSaberRankingIndexJson>(resJsonString);
            this._dataGetTime = DateTime.Now;
            List<int> userIndexData;
            if (!this._rankingIndex.UserIndexData.TryGetValue(userID, out userIndexData))
            {
                this._getDataActive = false;
                return;
            }
            this._userIDindex = userIndexData[1];
            var rankingFile = this._rankingIndex.RankingDataFile[userIndexData[0]];
            scoresaberURL = $"https://rynan4818.github.io/ScoreSaberRanking/json/{rankingFile}";
            resJsonString = await Utility.GetHttpContent(RankingHttpClient, scoresaberURL);
            if (resJsonString == null)
            {
                this._getDataActive = false;
                return;
            }
            this._rankingData = JsonConvert.DeserializeObject<ScoreSaberRankingDataJson>(resJsonString);
            Plugin.Log.Info("UserRankingData Get Complete!");
            this._getDataActive = false;
        }
        public object GetRankingData(string column)
        {
            if (this._userIDindex == null)
                return null;
            var column_index = this._rankingData.Column.IndexOf(column);
            if (column_index == -1)
                return null;
            return this._rankingData.UserData[(int)this._userIDindex][column_index];
        }
    }
    public class ScoreSaberRankingIndexJson
    {
        public int UpdateTime { get; set; }
        public int WeeklyChangeValueTime { get; set; }
        public int NextUpdateTime { get; set; }
        public List<string> RankingDataFile { get; set; }
        public Dictionary<string, List<int>> UserIndexData { get; set; }
    }

    public class ScoreSaberRankingDataJson
    {
        public List<string> Column { get; set; }
        public List<List<object>> UserData { get; set; }
    }
}
