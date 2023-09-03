using PlayerInfoViewer.Configuration;
using System;
using System.Threading.Tasks;
using Zenject;

namespace PlayerInfoViewer.Models
{
    public class PlayerDataManager : IInitializable
    {
        private readonly IPlatformUserModel _userModel;
        private readonly PlayerDataModel _playerDataModel;
        private readonly ScoreSaberPlayerInfo _scoreSaberPlayerInfo;
        private readonly HDTDataJson _hdtData;
        private readonly ScoreSaberRanking _rankingData;
        public string _userID;
        public event Action OnPlayerDataInitFinish;
        public bool _initFinish { get; set; } = false;

        public PlayerDataManager(IPlatformUserModel userModel, PlayerDataModel playerDataModel, ScoreSaberPlayerInfo scoreSaberPlayerInfo, HDTDataJson hdtData, ScoreSaberRanking rankingData)
        {
            this._userModel = userModel;
            this._playerDataModel = playerDataModel;
            this._scoreSaberPlayerInfo = scoreSaberPlayerInfo;
            this._hdtData = hdtData;
            this._rankingData = rankingData;
        }

        public void Initialize()
        {
            Plugin.Log.Debug("PlayerDataManager Initialize");
            // async void警察に怒られないようにします(；・∀・) https://light11.hatenadiary.com/entry/2019/03/05/221311
            _ = this.InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            var userInfo = await _userModel.GetUserInfo();
            this._userID = userInfo.platformUserId;
            await this.GetPlayerInfoAsync();
            this._hdtData.Load();
            if (PluginConfig.Instance.LastTimePlayed == 0)
                LastUpdateStatisticsData();
            await this._rankingData.GetUserRankingAsync(this._userID);
            //日付更新処理
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
        public async Task GetPlayerInfoAsync()
        {
            await this._scoreSaberPlayerInfo.GetPlayerFullInfoAsync(this._userID);
            if (this._scoreSaberPlayerInfo._playerFullInfo == null)
                return;
            //最終記録が初期値の場合の更新
            if (PluginConfig.Instance.BeforePP == 0)
            {
                PluginConfig.Instance.BeforePP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
                PluginConfig.Instance.NowPP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
            }
            //pp更新時の更新
            if (PluginConfig.Instance.NowPP != this._scoreSaberPlayerInfo._playerFullInfo.pp)
                PluginConfig.Instance.BeforePP = PluginConfig.Instance.NowPP;
            PluginConfig.Instance.NowPP = this._scoreSaberPlayerInfo._playerFullInfo.pp;

            //サーバエラーで最終記録が未更新時で更新可能になった場合
            if (PluginConfig.Instance.LastPlayerInfoNoGet)
                LastUpdatePlayerInfo();
        }
        public void LastPlayerInfoUpdate()
        {
            DateTime lastGetTime;
            if (!DateTime.TryParse(PluginConfig.Instance.LastGetTime, out lastGetTime))
                lastGetTime = DateTime.Now.AddYears(-1);
            if (lastGetTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                if (this._scoreSaberPlayerInfo._playerFullInfo == null)
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
            var playerFullInfo = this._scoreSaberPlayerInfo._playerFullInfo;
            PluginConfig.Instance.LastPP = playerFullInfo.pp;
            PluginConfig.Instance.LastRank = playerFullInfo.rank;
            PluginConfig.Instance.LastCountryRank = playerFullInfo.countryRank;
            PluginConfig.Instance.LastTotalScore = playerFullInfo.scoreStats.totalScore;
            PluginConfig.Instance.LastTotalRankedScore = playerFullInfo.scoreStats.totalRankedScore;
            PluginConfig.Instance.LastAverageRankedAccuracy = playerFullInfo.scoreStats.averageRankedAccuracy;
            PluginConfig.Instance.LastTotalPlayCount = playerFullInfo.scoreStats.totalPlayCount;
            PluginConfig.Instance.LastRankedPlayCount = playerFullInfo.scoreStats.rankedPlayCount;
            PluginConfig.Instance.LastReplaysWatched = playerFullInfo.scoreStats.replaysWatched;
            PluginConfig.Instance.BeforePP = playerFullInfo.pp;
            PluginConfig.Instance.NowPP = playerFullInfo.pp;
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
