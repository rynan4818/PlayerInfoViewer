using BS_Utils.Gameplay;
using PlayerInfoViewer.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlayerInfoViewer.Models
{
    public class PlayerDataManager : IDisposable
    {
        private bool _disposedValue = false;
        private readonly PlayerDataModel _playerDataModel;
        private readonly ScoreSaberPlayerInfo _scoreSaberPlayerInfo;
        private readonly BeatLeaderPlayerInfo _beatLeaderPlayerInfo;
        private readonly HDTDataJson _hdtData;
        private readonly ScoreSaberRanking _rankingData;
        public string _userID = null;
        private readonly CancellationTokenSource connectionClosed = new CancellationTokenSource();
        public bool _initFinish { get; set; } = false;
        public bool _initActive = false;

        public PlayerDataManager(
            PlayerDataModel playerDataModel,
            ScoreSaberPlayerInfo scoreSaberPlayerInfo,
            HDTDataJson hdtData,
            ScoreSaberRanking rankingData,
            BeatLeaderPlayerInfo beatLeaderPlayerInfo)
        {
            this._playerDataModel = playerDataModel;
            this._scoreSaberPlayerInfo = scoreSaberPlayerInfo;
            this._hdtData = hdtData;
            this._rankingData = rankingData;
            this._beatLeaderPlayerInfo = beatLeaderPlayerInfo;
        }

        public virtual void Dispose()
        {
            if (this._disposedValue)
                return;
            if (this._initFinish)
               PluginConfig.Instance.UserInfoDatas[this._userID].LastPlayTime = DateTime.Now.ToString();
            this.connectionClosed.Cancel();
            this._disposedValue = true;
        }

        public async Task InitiAsync()
        {
            if (this._initFinish || this._initActive)
                return;
            this._initActive = true;
            GetUserInfo.UpdateUserInfo();
            //GetUserInfo.GetUserAsync()を使えばよいけど、BS1.29.1との互換性のためGetUserID()を使用する。
            //GetUserID()は非推奨なので、1.29.1のサポートを外すときにGetUserAsync()に直す。
            var token = connectionClosed.Token;
            try
            {
                while (GetUserInfo.GetUserID() == null)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(10);
                }
            }
            catch (Exception)
            {
                return;
            }
            this._userID = GetUserInfo.GetUserID();
            if (!PluginConfig.Instance.UserInfoDatas.ContainsKey(this._userID))
                PluginConfig.Instance.UserInfoDatas.Add(this._userID, new UserInfoData());
            Plugin.Log.Debug("PlayerDataManager Initialize");
            this._hdtData.Load();
            if (PluginConfig.Instance.UserInfoDatas[this._userID].LastTimePlayed == 0)
                LastUpdateStatisticsData();
            await this.GetSSPlayerInfoAsync();
            await this.GetBLPlayerInfoAsync();
            await this._rankingData.GetUserRankingAsync(this._userID);
            //日付更新処理
            DateTime lastPlayTime;
            if (!DateTime.TryParse(PluginConfig.Instance.UserInfoDatas[this._userID].LastPlayTime, out lastPlayTime))
                lastPlayTime = DateTime.Now.AddYears(-1);
            if (DateTime.Now - lastPlayTime >= new TimeSpan(PluginConfig.Instance.IntervalTime, 0, 0))
            {
                LastPlayerStatisticsUpdate();
                LastSSPlayerInfoUpdate();
                LastBLPlayerInfoUpdate();
            }
            PluginConfig.Instance.UserInfoDatas[this._userID].LastPlayTime = DateTime.Now.ToString();
            this._initFinish = true;
            this._initActive = false;
        }
        public async Task GetSSPlayerInfoAsync()
        {
            if (this._userID == null)
                return;
            await this._scoreSaberPlayerInfo.GetPlayerFullInfoAsync(this._userID);
            if (this._scoreSaberPlayerInfo._playerFullInfo == null)
                return;
            //最終記録が初期値の場合
            if (PluginConfig.Instance.UserInfoDatas[this._userID].BeforePP == 0)
            {
                PluginConfig.Instance.UserInfoDatas[this._userID].BeforePP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
                PluginConfig.Instance.UserInfoDatas[this._userID].NowPP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
            }
            //pp更新時
            if (PluginConfig.Instance.UserInfoDatas[this._userID].NowPP != this._scoreSaberPlayerInfo._playerFullInfo.pp)
                PluginConfig.Instance.UserInfoDatas[this._userID].BeforePP = PluginConfig.Instance.UserInfoDatas[this._userID].NowPP;
            PluginConfig.Instance.UserInfoDatas[this._userID].NowPP = this._scoreSaberPlayerInfo._playerFullInfo.pp;
            //サーバエラーで最終記録が未更新時に更新可能になった場合
            if (PluginConfig.Instance.UserInfoDatas[this._userID].LastPlayerInfoNoGet)
                LastSSUpdatePlayerInfo();
        }
        public async Task GetBLPlayerInfoAsync()
        {
            if (this._userID == null)
                return;
            await this._beatLeaderPlayerInfo.GetPlayerInfoAsync(this._userID);
            if (this._beatLeaderPlayerInfo._playerInfo == null)
                return;
            //最終記録が初期値の場合
            if (PluginConfig.Instance.UserInfoDatas[this._userID].BLBeforePP == 0)
            {
                PluginConfig.Instance.UserInfoDatas[this._userID].BLBeforePP = this._beatLeaderPlayerInfo._playerInfo.pp;
                PluginConfig.Instance.UserInfoDatas[this._userID].BLNowPP = this._beatLeaderPlayerInfo._playerInfo.pp;
            }
            //pp更新時
            if (PluginConfig.Instance.UserInfoDatas[this._userID].BLNowPP != this._beatLeaderPlayerInfo._playerInfo.pp)
                PluginConfig.Instance.UserInfoDatas[this._userID].BLBeforePP = PluginConfig.Instance.UserInfoDatas[this._userID].BLNowPP;
            PluginConfig.Instance.UserInfoDatas[this._userID].BLNowPP = this._beatLeaderPlayerInfo._playerInfo.pp;
            //サーバエラーで最終記録が未更新時に更新可能になった場合
            if (PluginConfig.Instance.UserInfoDatas[this._userID].LastBLPlayerInfoNoGet)
                LastBLUpdatePlayerInfo();
        }
        public void LastSSPlayerInfoUpdate()
        {
            if (this._userID == null)
                return;
            DateTime lastGetTime;
            if (!DateTime.TryParse(PluginConfig.Instance.UserInfoDatas[this._userID].LastGetTime, out lastGetTime))
                lastGetTime = DateTime.Now.AddYears(-1);
            if (lastGetTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                if (this._scoreSaberPlayerInfo._playerFullInfo == null)
                {
                    PluginConfig.Instance.UserInfoDatas[this._userID].LastPlayerInfoNoGet = true;
                    return;
                }
                LastSSUpdatePlayerInfo();
            }
        }
        public void LastBLPlayerInfoUpdate()
        {
            if (this._userID == null)
                return;
            DateTime lastGetTime;
            if (!DateTime.TryParse(PluginConfig.Instance.UserInfoDatas[this._userID].LastBLGetTime, out lastGetTime))
                lastGetTime = DateTime.Now.AddYears(-1);
            if (lastGetTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                if (this._beatLeaderPlayerInfo._playerInfo == null)
                {
                    PluginConfig.Instance.UserInfoDatas[this._userID].LastBLPlayerInfoNoGet = true;
                    return;
                }
                LastBLUpdatePlayerInfo();
            }
        }
        public void LastPlayerStatisticsUpdate()
        {
            if (this._userID == null)
                return;
            DateTime lastGetStatisticsTime;
            if (!DateTime.TryParse(PluginConfig.Instance.UserInfoDatas[this._userID].LastGetStatisticsTime, out lastGetStatisticsTime))
                lastGetStatisticsTime = DateTime.Now.AddYears(-1);
            if (lastGetStatisticsTime < DateTime.Today.AddHours(PluginConfig.Instance.DateChangeTime))
            {
                PluginConfig.Instance.UserInfoDatas[this._userID].LastGetStatisticsTime = DateTime.Now.ToString();
                LastUpdateStatisticsData();
            }
        }
        public void LastSSUpdatePlayerInfo()
        {
            if (this._userID == null)
                return;
            var playerFullInfo = this._scoreSaberPlayerInfo._playerFullInfo;
            var config = PluginConfig.Instance.UserInfoDatas[this._userID];
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
            if (this._userID == null)
                return;
            var playerInfo = this._beatLeaderPlayerInfo._playerInfo;
            var config = PluginConfig.Instance.UserInfoDatas[this._userID];
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
            if (this._userID == null)
                return;
            var allOverallStatsData = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData;
            var config = PluginConfig.Instance.UserInfoDatas[this._userID];
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
