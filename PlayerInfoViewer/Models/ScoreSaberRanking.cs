﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayerInfoViewer.Util;

namespace PlayerInfoViewer.Models
{
    public class ScoreSaberRanking
    {
        public ScoreSaberRankingIndexJson _rankingIndex;
        public ScoreSaberRankingDataJson _rankingData;
        public int? _userIDindex = null;
        public bool _getDataActive = false;
        public DateTime _dataGetTime = DateTime.Now.AddYears(-1);
        public async Task GetUserRankingAsync(string userID)
        {
            if (userID == null || this._getDataActive)
                return;
            if (this._rankingIndex != null && this._rankingIndex.NextUpdateTime > new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
                return;
            if (DateTime.Now - this._dataGetTime < new TimeSpan(0, 15, 0))
                return;
            this._getDataActive = true;
            try
            {
                var rankingURL = "https://rynan4818.github.io/ScoreSaberRanking/json/scoresaber_rank_index.json";
                var resJsonString = await HttpUtility.GetHttpContentAsync(rankingURL);
                if (resJsonString == null)
                    throw new Exception("Ranking index get error");
                this._rankingIndex = JsonConvert.DeserializeObject<ScoreSaberRankingIndexJson>(resJsonString);
                this._dataGetTime = DateTime.Now;
                List<int> userIndexData;
                if (!this._rankingIndex.UserIndexData.TryGetValue(userID, out userIndexData))
                    throw new Exception("UserID not found in ranking");
                this._userIDindex = userIndexData[1];
                var rankingFile = this._rankingIndex.RankingDataFile[userIndexData[0]];
                rankingURL = $"https://rynan4818.github.io/ScoreSaberRanking/json/{rankingFile}";
                resJsonString = await HttpUtility.GetHttpContentAsync(rankingURL);
                if (resJsonString == null)
                    throw new Exception("Ranking data get error");
                this._rankingData = JsonConvert.DeserializeObject<ScoreSaberRankingDataJson>(resJsonString);
                Plugin.Log.Info("UserRankingData Get Complete!");
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
            }
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
