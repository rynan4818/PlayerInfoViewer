using PlayerInfoViewer.Configuration;
using PlayerInfoViewer.Util;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Zenject;
using System.Net.Http;

namespace PlayerInfoViewer.Models
{
    public class PlayerDataManager : IInitializable
    {
        private readonly IPlatformUserModel _userModel;
        private readonly PlayerDataModel _playerDataModel;
        private readonly HDTDataJson _hdtData;
        private readonly ScoreSaberRankingJson _rankingData;
        public static readonly HttpClient ScoresaberHttpClient = new HttpClient();
        public bool _playerInfoGetActive = false;
        public string _userID;
        public PlayerFullInfoJson _playerFullInfo;
        public event Action OnPlayerDataInitFinish;
        public bool _initFinish = false;

        public PlayerDataManager(IPlatformUserModel userModel, PlayerDataModel playerDataModel, HDTDataJson hdtData, ScoreSaberRankingJson rankingData)
        {
            this._userModel = userModel;
            this._playerDataModel = playerDataModel;
            this._hdtData = hdtData;
            this._rankingData = rankingData;
        }

        public async void Initialize()
        {
            Plugin.Log.Debug("PlayerDataManager Initialize");
            var userInfo = await _userModel.GetUserInfo();
            this._userID = userInfo.platformUserId;
            await GetPlayerFullInfo();
            this._hdtData.Load();
            await this._rankingData.GetUserRanking(this._userID);
            DateTime lastPlayTime;
            if (!DateTime.TryParse(PluginConfig.Instance.LastPlayTime, out lastPlayTime))
                lastPlayTime = DateTime.Now.AddYears(-1);
            if (DateTime.Now - lastPlayTime >= new TimeSpan(PluginConfig.Instance.IntervalTime, 0, 0))
            {
                LastPlayerStatisticsUpdate();
                LastPlayerInfoUpdate();
            }
            PluginConfig.Instance.LastPlayTime = DateTime.Now.ToString();
            this._initFinish = true;
            this.OnPlayerDataInitFinish?.Invoke();
        }
        public async Task GetPlayerFullInfo()
        {
            if (this._userID == null || this._playerInfoGetActive)
                return;
            this._playerInfoGetActive = true;
            this._playerFullInfo = null;
            var playerFullInfoURL = $"https://scoresaber.com/api/player/{_userID}/full";
            try
            {
                var resJsonString = await Utility.GetHttpContent(ScoresaberHttpClient, playerFullInfoURL);
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
            if (PluginConfig.Instance.LastTimePlayed == 0)
                LastUpdateStatisticsData();
            if (PluginConfig.Instance.BeforePP == 0)
            {
                PluginConfig.Instance.BeforePP = this._playerFullInfo.pp;
                PluginConfig.Instance.NowPP = this._playerFullInfo.pp;
            }
            if (PluginConfig.Instance.NowPP != this._playerFullInfo.pp)
                PluginConfig.Instance.BeforePP = PluginConfig.Instance.NowPP;
            PluginConfig.Instance.NowPP = this._playerFullInfo.pp;
            if (PluginConfig.Instance.LastPlayerInfoNoGet)
                LastUpdatePlayerInfo();
            this._playerInfoGetActive = false;
        }
        public void LastPlayerInfoUpdate()
        {
            DateTime lastGetTime;
            if (!DateTime.TryParse(PluginConfig.Instance.LastGetTime, out lastGetTime))
                lastGetTime = DateTime.Now.AddYears(-1);
            if (lastGetTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                if (this._playerFullInfo == null)
                {
                    PluginConfig.Instance.LastPlayerInfoNoGet = true;
                    return;
                }
                LastUpdatePlayerInfo();
            }
        }
        public void LastPlayerStatisticsUpdate()
        {
            DateTime lastGetStatisticsTime;
            if (!DateTime.TryParse(PluginConfig.Instance.LastGetStatisticsTime, out lastGetStatisticsTime))
                lastGetStatisticsTime = DateTime.Now.AddYears(-1);
            if (lastGetStatisticsTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                PluginConfig.Instance.LastGetStatisticsTime = DateTime.Now.ToString();
                LastUpdateStatisticsData();
            }
        }
        public void LastUpdatePlayerInfo()
        {
            PluginConfig.Instance.LastGetTime = DateTime.Now.ToString();
            PluginConfig.Instance.LastPlayerInfoNoGet = false;
            PluginConfig.Instance.LastPP = this._playerFullInfo.pp;
            PluginConfig.Instance.LastRank = this._playerFullInfo.rank;
            PluginConfig.Instance.LastCountryRank = this._playerFullInfo.countryRank;
            PluginConfig.Instance.LastTotalScore = this._playerFullInfo.scoreStats.totalScore;
            PluginConfig.Instance.LastTotalRankedScore = this._playerFullInfo.scoreStats.totalRankedScore;
            PluginConfig.Instance.LastAverageRankedAccuracy = this._playerFullInfo.scoreStats.averageRankedAccuracy;
            PluginConfig.Instance.LastTotalPlayCount = this._playerFullInfo.scoreStats.totalPlayCount;
            PluginConfig.Instance.LastRankedPlayCount = this._playerFullInfo.scoreStats.rankedPlayCount;
            PluginConfig.Instance.LastReplaysWatched = this._playerFullInfo.scoreStats.replaysWatched;
            PluginConfig.Instance.BeforePP = this._playerFullInfo.pp;
            PluginConfig.Instance.NowPP = this._playerFullInfo.pp;
        }
        public void LastUpdateStatisticsData()
        {
            PluginConfig.Instance.LastPlayedLevelsCount = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData.playedLevelsCount;
            PluginConfig.Instance.LastClearedLevelsCount = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData.clearedLevelsCount;
            PluginConfig.Instance.LastFailedLevelsCount = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData.failedLevelsCount;
            PluginConfig.Instance.LastFullComboCount = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData.fullComboCount;
            PluginConfig.Instance.LastTimePlayed = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData.timePlayed;
            PluginConfig.Instance.LastHandDistanceTravelled = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData.handDistanceTravelled;
            PluginConfig.Instance.LastHeadDistanceTravelled = this._hdtData.HeadDistanceTravelled;
        }
    }
}
