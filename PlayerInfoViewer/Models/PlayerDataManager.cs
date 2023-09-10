using PlayerInfoViewer.Configuration;
using System;
using System.Threading.Tasks;

namespace PlayerInfoViewer.Models
{
    public class PlayerDataManager
    {
        private readonly IPlatformUserModel _userModel;
        private readonly PlayerDataModel _playerDataModel;
        private readonly ScoreSaberPlayerInfo _scoreSaberPlayerInfo;
        private readonly BeatLeaderPlayerInfo _beatLeaderPlayerInfo;
        private readonly HDTDataJson _hdtData;
        private readonly ScoreSaberRanking _rankingData;
        public string _userID;
        public bool _initFinish { get; set; } = false;
        public bool _initActive = false;

        public PlayerDataManager(IPlatformUserModel userModel,
            PlayerDataModel playerDataModel,
            ScoreSaberPlayerInfo scoreSaberPlayerInfo,
            HDTDataJson hdtData,
            ScoreSaberRanking rankingData,
            BeatLeaderPlayerInfo beatLeaderPlayerInfo)
        {
            this._userModel = userModel;
            this._playerDataModel = playerDataModel;
            this._scoreSaberPlayerInfo = scoreSaberPlayerInfo;
            this._hdtData = hdtData;
            this._rankingData = rankingData;
            this._beatLeaderPlayerInfo = beatLeaderPlayerInfo;
        }

        public async Task InitiAsync()
        {
            if (this._initFinish || this._initActive)
                return;
            this._initActive = true;
            Plugin.Log.Debug("PlayerDataManager Initialize");
            this._hdtData.Load();
            if (PluginConfig.Instance.LastTimePlayed == 0)
                LastUpdateStatisticsData();
            var userInfo = await _userModel.GetUserInfo();
            this._userID = userInfo.platformUserId;
            await this.GetSSPlayerInfoAsync();
            await this.GetBLPlayerInfoAsync();
            await this._rankingData.GetUserRankingAsync(this._userID);
            //日付更新処理
            DateTime lastPlayTime;
            if (!DateTime.TryParse(PluginConfig.Instance.LastPlayTime, out lastPlayTime))
                lastPlayTime = DateTime.Now.AddYears(-1);
            if (DateTime.Now - lastPlayTime >= new TimeSpan(PluginConfig.Instance.IntervalTime, 0, 0))
            {
                LastPlayerStatisticsUpdate();
                LastSSPlayerInfoUpdate();
                LastBLPlayerInfoUpdate();
            }
            PluginConfig.Instance.LastPlayTime = DateTime.Now.ToString();
            this._initFinish = true;
            this._initActive = false;
        }
        public async Task GetSSPlayerInfoAsync()
        {
            await this._scoreSaberPlayerInfo.GetPlayerFullInfoAsync(this._userID);
            if (this._scoreSaberPlayerInfo._playerFullInfo == null)
                return;
            //最終記録が初期値の場合
            if (PluginConfig.Instance.BeforePP == 0)
            {
                PluginConfig.Instance.BeforePP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
                PluginConfig.Instance.NowPP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
            }
            //pp更新時
            if (PluginConfig.Instance.NowPP != this._scoreSaberPlayerInfo._playerFullInfo.pp)
                PluginConfig.Instance.BeforePP = PluginConfig.Instance.NowPP;
            PluginConfig.Instance.NowPP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
            //サーバエラーで最終記録が未更新時に更新可能になった場合
            if (PluginConfig.Instance.LastPlayerInfoNoGet)
                LastSSUpdatePlayerInfo();
        }
        public async Task GetBLPlayerInfoAsync()
        {
            await this._beatLeaderPlayerInfo.GetPlayerInfoAsync(this._userID);
            if (this._beatLeaderPlayerInfo._playerInfo == null)
                return;
            //最終記録が初期値の場合
            if (PluginConfig.Instance.BLBeforePP == 0)
            {
                PluginConfig.Instance.BLBeforePP = this._beatLeaderPlayerInfo._playerInfo.pp;
                PluginConfig.Instance.BLNowPP = this._beatLeaderPlayerInfo._playerInfo.pp;
            }
            //pp更新時
            if (PluginConfig.Instance.BLNowPP != this._beatLeaderPlayerInfo._playerInfo.pp)
                PluginConfig.Instance.BLBeforePP = PluginConfig.Instance.BLNowPP;
            PluginConfig.Instance.BLNowPP = this._beatLeaderPlayerInfo._playerInfo.pp;
            //サーバエラーで最終記録が未更新時に更新可能になった場合
            if (PluginConfig.Instance.LastBLPlayerInfoNoGet)
                LastBLUpdatePlayerInfo();
        }
        public void LastSSPlayerInfoUpdate()
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
                LastSSUpdatePlayerInfo();
            }
        }
        public void LastBLPlayerInfoUpdate()
        {
            DateTime lastGetTime;
            if (!DateTime.TryParse(PluginConfig.Instance.LastBLGetTime, out lastGetTime))
                lastGetTime = DateTime.Now.AddYears(-1);
            if (lastGetTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                if (this._beatLeaderPlayerInfo._playerInfo == null)
                {
                    PluginConfig.Instance.LastBLPlayerInfoNoGet = true;
                    return;
                }
                LastBLUpdatePlayerInfo();
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
        public void LastSSUpdatePlayerInfo()
        {
            var playerFullInfo = this._scoreSaberPlayerInfo._playerFullInfo;
            var config = PluginConfig.Instance;
            config.LastGetTime = DateTime.Now.ToString();
            config.LastPlayerInfoNoGet = false;
            config.LastPP = playerFullInfo.pp;
            config.LastRank = playerFullInfo.rank;
            config.LastCountryRank = playerFullInfo.countryRank;
            config.LastTotalScore = playerFullInfo.scoreStats.totalScore;
            config.LastTotalRankedScore = playerFullInfo.scoreStats.totalRankedScore;
            config.LastAverageRankedAccuracy = playerFullInfo.scoreStats.averageRankedAccuracy;
            config.LastTotalPlayCount = playerFullInfo.scoreStats.totalPlayCount;
            config.LastRankedPlayCount = playerFullInfo.scoreStats.rankedPlayCount;
            config.LastReplaysWatched = playerFullInfo.scoreStats.replaysWatched;
            config.BeforePP = playerFullInfo.pp;
            config.NowPP = playerFullInfo.pp;
        }
        public void LastBLUpdatePlayerInfo()
        {
            var playerInfo = this._beatLeaderPlayerInfo._playerInfo;
            var config = PluginConfig.Instance;
            config.LastBLGetTime = DateTime.Now.ToString();
            config.LastBLPlayerInfoNoGet = false;
            config.LastBLPP = playerInfo.pp;
            config.LastBLRank = playerInfo.rank;
            config.LastBLCountryRank = playerInfo.countryRank;
            config.LastBLTotalPlayCount = playerInfo.scoreStats.totalPlayCount;
            config.LastBLRankedPlayCount = playerInfo.scoreStats.rankedPlayCount;
            config.BLBeforePP = playerInfo.pp;
            config.BLNowPP = playerInfo.pp;
        }
        public void LastUpdateStatisticsData()
        {
            var allOverallStatsData = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData;
            var config = PluginConfig.Instance;
            config.LastPlayedLevelsCount = allOverallStatsData.playedLevelsCount;
            config.LastClearedLevelsCount = allOverallStatsData.clearedLevelsCount;
            config.LastFailedLevelsCount = allOverallStatsData.failedLevelsCount;
            config.LastFullComboCount = allOverallStatsData.fullComboCount;
            config.LastTimePlayed = allOverallStatsData.timePlayed;
            config.LastHandDistanceTravelled = allOverallStatsData.handDistanceTravelled;
            config.LastHeadDistanceTravelled = this._hdtData.HeadDistanceTravelled;
        }
    }
}
