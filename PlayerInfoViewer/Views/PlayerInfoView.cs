using HMUI;
using LeaderboardCore.Interfaces;
using PlayerInfoViewer.Configuration;
using PlayerInfoViewer.Models;
using PlayerInfoViewer.HarmonyPatches;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Threading.Tasks;

namespace PlayerInfoViewer.Views
{
    public class PlayerInfoView : MonoBehaviour, INotifyScoreUpload
    {
        //デンパ時計さんのCustomMenuMusicをコピーしています。
        //https://github.com/denpadokei/CustomMenuMusic
        //コピー元:https://github.com/denpadokei/CustomMenuMusic/blob/master/CustomMenuMusic/NowPlaying.cs
        //MITライセンス:https://github.com/denpadokei/CustomMenuMusic/blob/master/LICENSE
        private PlatformLeaderboardViewController _platformLeaderboardViewController;
        private PlayerDataModel _playerDataModel;
        private PlayerDataManager _playerDataManager;
        private ScoreSaberPlayerInfo _scoreSaberPlayerInfo;
        private BeatLeaderPlayerInfo _beatLeaderPlayerInfo;
        private ScoreSaberRanking _rankingData;
        public GameObject rootObject;
        private Canvas _canvas;
        private CurvedTextMeshPro _playerStatistics;
        private CurvedTextMeshPro _playCount;
        private CurvedTextMeshPro _rankPP;
        public float lastPlayed;
        public int _co2;
        public double _hum;
        public double _tmp;
        public bool _beatLeaderBoardEnabled = false;

        public static readonly Vector2 CanvasSize = new Vector2(100, 50);
        public static readonly Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);
        public static readonly Vector3 RightPosition = new Vector3(3.5f, 3.4f, 2.2f);
        public static readonly Vector3 RightRotation = new Vector3(0, 56, 0);

        //MonoBehaviourはコンストラクタを使えないので、メソッドでインジェクションする
        [Inject]
        public void Constractor(PlayerDataManager playerDataManager,
            PlatformLeaderboardViewController platformLeaderboardViewController,
            PlayerDataModel playerDataModel,
            ScoreSaberPlayerInfo scoreSaberPlayerInfo,
            BeatLeaderPlayerInfo beatLeaderPlayerInfo,
            ScoreSaberRanking rankingData)
        {
            this._playerDataManager = playerDataManager;
            this._platformLeaderboardViewController = platformLeaderboardViewController;
            this._playerDataModel = playerDataModel;
            this._rankingData = rankingData;
            this._scoreSaberPlayerInfo = scoreSaberPlayerInfo;
            this._beatLeaderPlayerInfo = beatLeaderPlayerInfo;
        }
        private void Awake()
        {
            Plugin.Log.Debug("PlayerInfoView Awake");
            this.rootObject = new GameObject("PlayCount Canvas", typeof(Canvas), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            var sizeFitter = this.rootObject.GetComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            this._canvas = this.rootObject.GetComponent<Canvas>();
            this._canvas.sortingOrder = 3;
            this._canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = this._canvas.transform as RectTransform;
            rectTransform.sizeDelta = CanvasSize;
            this.rootObject.transform.position = RightPosition + new Vector3(0, PluginConfig.Instance.ViewYoffset, 0);
            this.rootObject.transform.eulerAngles = RightRotation;
            this.rootObject.transform.localScale = Scale;
            if (PluginConfig.Instance.ViewPlayerStatistics)
            {
                this._playerStatistics = this.CreateText(this._canvas.transform as RectTransform, string.Empty, new Vector2(10, 31));
                rectTransform = this._playerStatistics.transform as RectTransform;
                rectTransform.SetParent(this._canvas.transform, false);
                rectTransform.anchoredPosition = Vector2.zero;
                this._playerStatistics.fontSize = PluginConfig.Instance.ViewFontSize;
                this._playerStatistics.color = Color.white;
                this._playerStatistics.text = "Player Statistics Load data...";
            }
            if (PluginConfig.Instance.ViewPlayCount)
            {
                this._playCount = this.CreateText(this._canvas.transform as RectTransform, string.Empty, new Vector2(10, 31));
                rectTransform = this._playCount.transform as RectTransform;
                rectTransform.SetParent(this._canvas.transform, false);
                rectTransform.anchoredPosition = Vector2.zero;
                this._playCount.fontSize = PluginConfig.Instance.ViewFontSize;
                this._playCount.color = Color.white;
                this._playCount.text = "Play Count Load data...";
            }
            if (PluginConfig.Instance.ViewRankPP)
            {
                this._rankPP = this.CreateText(this._canvas.transform as RectTransform, string.Empty, new Vector2(10, 31));
                rectTransform = this._rankPP.transform as RectTransform;
                rectTransform.SetParent(this._canvas.transform, false);
                rectTransform.anchoredPosition = Vector2.zero;
                this._rankPP.fontSize = PluginConfig.Instance.ViewFontSize;
                this._rankPP.color = Color.white;
                this._rankPP.text = "Rank PP Load data...";
            }
            this._platformLeaderboardViewController.didActivateEvent += this.OnLeaderboardActivated;
            this._platformLeaderboardViewController.didDeactivateEvent += this.OnLeaderboardDeactivated;
            CO2CoreManagerPatch.OnCO2Changed += this.OnCO2Changed;
            HeadDistanceTravelledControllerPatch.OnHDTUpdate += this.OnHDTUpdate;
            CustomLeaderboardShowPatch.OnCustomLeaderboardShowed += this.OnCustomLeaderboardShowed;
            CustomLeaderboardHidePatch.OnCustomLeaderboardHidden += this.OnCustomLeaderboardHidden;
            UploadPlayRequestPatch.OnUploadPlayFinished += this.OnBLScoreUploaded;
            UploadReplayRequestPatch.OnUploadReplayFinished += this.OnBLScoreUploaded;
            this.rootObject.SetActive(false);
        }
        private void OnDestroy()
        {
            Plugin.Log.Debug("PlayerInfoView Destroy");
            this._platformLeaderboardViewController.didDeactivateEvent -= this.OnLeaderboardDeactivated;
            this._platformLeaderboardViewController.didActivateEvent -= this.OnLeaderboardActivated;
            CO2CoreManagerPatch.OnCO2Changed -= this.OnCO2Changed;
            HeadDistanceTravelledControllerPatch.OnHDTUpdate -= this.OnHDTUpdate;
            CustomLeaderboardShowPatch.OnCustomLeaderboardShowed -= this.OnCustomLeaderboardShowed;
            CustomLeaderboardHidePatch.OnCustomLeaderboardHidden -= this.OnCustomLeaderboardHidden;
            UploadPlayRequestPatch.OnUploadPlayFinished -= this.OnBLScoreUploaded;
            UploadReplayRequestPatch.OnUploadReplayFinished -= this.OnBLScoreUploaded;
            Destroy(this.rootObject);
        }
        private CurvedTextMeshPro CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return this.CreateText(parent, text, anchoredPosition, new Vector2(0, 0));
        }

        private CurvedTextMeshPro CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            var textMesh = gameObj.AddComponent<CurvedTextMeshPro>();
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.overrideColorTags = true;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0f, 0f);
            textMesh.rectTransform.anchorMax = new Vector2(0f, 0f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }
        public void OnPlyerStatisticsChange(bool change = false)
        {
            if (this._playerDataManager._userID == null)
                return;
            if (!PluginConfig.Instance.ViewPlayerStatistics || !this._playerDataManager._initFinish)
                return;
            var allOverallStatsData = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData;
            if (!change && this.lastPlayed == allOverallStatsData.playedLevelsCount)
                return;
            this.lastPlayed = allOverallStatsData.playedLevelsCount;
            var userdata = PluginConfig.Instance.UserInfoDatas[this._playerDataManager._userID];
            var todayPlayed = String.Format("{0:+0;-0;+0}", allOverallStatsData.playedLevelsCount - userdata.LastPlayedLevelsCount);
            var todayCleared = String.Format("{0:+0;-0;+0}", allOverallStatsData.clearedLevelsCount - userdata.LastClearedLevelsCount);
            var todayFailed = String.Format("{0:+0;-0;+0}", allOverallStatsData.failedLevelsCount - userdata.LastFailedLevelsCount);
            var todayFullCombo = String.Format("{0:+0;-0;+0}", allOverallStatsData.fullComboCount - userdata.LastFullComboCount);
            var todayTimePlayed = String.Format("{0:+0.#;-0.#;+0}", (allOverallStatsData.timePlayed - userdata.LastTimePlayed) / 60f);
            var todayHandDistanceTravelled = String.Format("{0:+0.#;-0.#;+0}", (float)(allOverallStatsData.handDistanceTravelled - userdata.LastHandDistanceTravelled) / 1000f);
            var todayHeadDistanceTravelled = "";
            if (HeadDistanceTravelledControllerPatch.Enable)
                todayHeadDistanceTravelled = String.Format("  Head : {0:+0;-0;+0}m", userdata.TodayHeadDistanceTravelled);
            this._playerStatistics.text = $"Play : {todayPlayed}  Clear : {todayCleared}  Fail : {todayFailed}  FC : {todayFullCombo}  Time : {todayTimePlayed}m  Hand : {todayHandDistanceTravelled}km{todayHeadDistanceTravelled}";
        }
        public void OnPlayCountChange()
        {
            if (this._playerDataManager._userID == null)
                return;
            if (!PluginConfig.Instance.ViewPlayCount || !this._playerDataManager._initFinish)
                return;
            if (this._beatLeaderBoardEnabled)
            {
                if (this._beatLeaderPlayerInfo._playerInfoGetActive)
                    return;
                if (this._beatLeaderPlayerInfo._playerInfo == null || this._beatLeaderPlayerInfo._playerInfo.id == null)
                {
                    this._playCount.color = Color.red;
                    this._playCount.text = " BeatLeader Access Error!";
                    return;
                }
            }
            else
            {
                if (this._scoreSaberPlayerInfo._playerInfoGetActive)
                    return;
                if (this._scoreSaberPlayerInfo._playerFullInfo == null || this._scoreSaberPlayerInfo._playerFullInfo.id == null)
                {
                    this._playCount.color = Color.red;
                    this._playCount.text = " ScoreSaber Access Error!";
                    return;
                }
            }
            this._playCount.color = Color.white;
            int playCount;
            int lastPlayCount;
            int rankedPlayCount;
            int lastRankedPlayCount;
            string leaderBoard = "";
            var userdata = PluginConfig.Instance.UserInfoDatas[this._playerDataManager._userID];
            if (this._beatLeaderBoardEnabled)
            {
                playCount = this._beatLeaderPlayerInfo._playerInfo.scoreStats.totalPlayCount;
                rankedPlayCount = this._beatLeaderPlayerInfo._playerInfo.scoreStats.rankedPlayCount;
                lastPlayCount = userdata.LastBLTotalPlayCount;
                lastRankedPlayCount = userdata.LastBLRankedPlayCount;
                leaderBoard = "BeatLeader ";
            }
            else
            {
                playCount = this._scoreSaberPlayerInfo._playerFullInfo.scoreStats.totalPlayCount;
                rankedPlayCount = this._scoreSaberPlayerInfo._playerFullInfo.scoreStats.rankedPlayCount;
                lastPlayCount = userdata.LastTotalPlayCount;
                lastRankedPlayCount = userdata.LastRankedPlayCount;
            }
            var todayPlayCount = String.Format("{0:+#;-#;#}", playCount - lastPlayCount);
            var todayRankedPlayCount = String.Format("{0:+#;-#;#}", rankedPlayCount - lastRankedPlayCount);
            string totalPlayCountRank = "";
            string rankedPlayCountRank = "";
            if (this._beatLeaderBoardEnabled ==  false)
            {
                var totalPlayCountRankObject = this._rankingData.GetRankingData("TotalPlayCountRank");
                if (totalPlayCountRankObject != null)
                    totalPlayCountRank = $"   #{totalPlayCountRankObject}";
                var totalPlayCountLocalRankObject = this._rankingData.GetRankingData("TotalPlayCountLocalRank");
                if (totalPlayCountLocalRankObject != null)
                    totalPlayCountRank += $"/#{totalPlayCountLocalRankObject}";
                var rankedPlayCountRankObject = this._rankingData.GetRankingData("RankedPlayCountRank");
                if (rankedPlayCountRankObject != null)
                    rankedPlayCountRank = $"   #{rankedPlayCountRankObject}";
                var rankedPlayCountLocalRankObject = this._rankingData.GetRankingData("RankedPlayCountLocalRank");
                if (rankedPlayCountLocalRankObject != null)
                    rankedPlayCountRank += $"/#{rankedPlayCountLocalRankObject}";
            }
            this._playCount.text = $"{leaderBoard}Play Count    Total : {playCount} {todayPlayCount}{totalPlayCountRank}    Ranked : {rankedPlayCount} {todayRankedPlayCount}{rankedPlayCountRank}";
        }
        public void OnRankPpChange()
        {
            if (this._playerDataManager._userID == null)
                return;
            if (!PluginConfig.Instance.ViewRankPP || !this._playerDataManager._initFinish)
                return;
            if (this._beatLeaderBoardEnabled)
            {
                if (this._beatLeaderPlayerInfo._playerInfoGetActive)
                    return;
                if (this._beatLeaderPlayerInfo._playerInfo == null || this._beatLeaderPlayerInfo._playerInfo.id == null)
                {
                    this._rankPP.color = Color.red;
                    this._rankPP.text = " BeatLeader Access Error!";
                    return;
                }
            }
            else
            {
                if (this._scoreSaberPlayerInfo._playerInfoGetActive)
                    return;
                if (this._scoreSaberPlayerInfo._playerFullInfo == null || this._scoreSaberPlayerInfo._playerFullInfo.id == null)
                {
                    this._rankPP.color = Color.red;
                    this._rankPP.text = " ScoreSaber Access Error!";
                    return;
                }
            }
            this._rankPP.color = Color.white;
            float pp;
            int localRank;
            string localRankText;
            int rank;
            int lastRank;
            int lastCountryRank;
            float lastPP;
            float nowPP;
            float beforePP;
            var userdata = PluginConfig.Instance.UserInfoDatas[this._playerDataManager._userID];
            if (this._beatLeaderBoardEnabled)
            {
                pp = this._beatLeaderPlayerInfo._playerInfo.pp;
                localRank = this._beatLeaderPlayerInfo._playerInfo.countryRank;
                localRankText = "";
                rank = this._beatLeaderPlayerInfo._playerInfo.rank;
                lastRank = userdata.LastBLRank;
                lastCountryRank = userdata.LastBLCountryRank;
                lastPP = userdata.LastBLPP;
                nowPP = userdata.BLNowPP;
                beforePP = userdata.BLBeforePP;
            }
            else
            {
                pp = this._scoreSaberPlayerInfo._playerFullInfo.pp;
                localRank = this._scoreSaberPlayerInfo._playerFullInfo.countryRank;
                localRankText = $"#{localRank} ";
                rank = this._scoreSaberPlayerInfo._playerFullInfo.rank;
                lastRank = userdata.LastRank;
                lastCountryRank = userdata.LastCountryRank;
                lastPP = userdata.LastPP;
                nowPP = userdata.NowPP;
                beforePP = userdata.BeforePP;
            }
            var todayRankUp = String.Format("{0:+#;-#;+0}", lastRank - rank);
            var todayLocalRankUp = String.Format("{0:+#;-#;+0}", lastCountryRank - localRank);
            var todayPpUp = String.Format("{0:+0.##;-0.##;+0.##}", Math.Round(pp - lastPP, 2, MidpointRounding.AwayFromZero));
            var lastChangePp = String.Format("{0:+0.##;-0.##;+0.##}", Math.Round(nowPP - beforePP, 2, MidpointRounding.AwayFromZero));
            var text = $"Global : {todayRankUp}    Local : {localRankText}{todayLocalRankUp}    Today : {todayPpUp}pp  Last : {lastChangePp}pp";
            if (CO2CoreManagerPatch.Enable)
                text += $"    {this._co2}ppm  {this._tmp}℃  {this._hum}%";
            this._rankPP.text = text;
        }
        public void CustomLeaderboardChanged(string leaderboardId, bool show)
        {
            bool beatLeaderBoard;
            if (show && leaderboardId.Contains("BeatLeader"))
                beatLeaderBoard = true;
            else
                beatLeaderBoard = false;
            if (beatLeaderBoard != _beatLeaderBoardEnabled)
            {
                _beatLeaderBoardEnabled = beatLeaderBoard;
                this.OnPlayCountChange();
                this.OnRankPpChange();
            }
        }
        public void OnCustomLeaderboardShowed(string leaderboardId)
        {
            CustomLeaderboardChanged(leaderboardId, true);
        }
        public void OnCustomLeaderboardHidden(string leaderboardId)
        {
            CustomLeaderboardChanged(leaderboardId, false);
        }
        public void OnCO2Changed((int, double, double) co2data)
        {
            this._co2 = co2data.Item1;
            this._hum = co2data.Item2;
            this._tmp = co2data.Item3;
            this.OnRankPpChange();
        }
        public void OnHDTUpdate(float hmdDistance)
        {
            if (this._playerDataManager._userID == null)
                return;
            PluginConfig.Instance.UserInfoDatas[this._playerDataManager._userID].TodayHeadDistanceTravelled += hmdDistance;
            this.OnPlyerStatisticsChange(true);
        }
        public void OnLeaderboardActivated(bool firstactivation, bool addedtohierarchy, bool screensystemenabling)
        {
            // async void警察に怒られないようにします(；・∀・) https://light11.hatenadiary.com/entry/2019/03/05/221311
            _ = LeaderboardActivatedAsync();
        }
        public async Task LeaderboardActivatedAsync()
        {
            this.rootObject.SetActive(true);
            if (!this._playerDataManager._initActive || !this._playerDataManager._initFinish)
            {
                await this._playerDataManager.InitiAsync();
                this.OnPlayCountChange();
                this.OnRankPpChange();
            }
            this.OnPlyerStatisticsChange();
            if (!this._playerDataManager._initFinish)
                return;
            if (!this._scoreSaberPlayerInfo._playerInfoGetActive && (this._scoreSaberPlayerInfo._playerFullInfo == null || this._scoreSaberPlayerInfo._playerFullInfo.id == null))
                this.OnScoreUploaded();
            if (!this._beatLeaderPlayerInfo._playerInfoGetActive && (this._beatLeaderPlayerInfo._playerInfo == null || this._beatLeaderPlayerInfo._playerInfo.id == null))
                this.OnBLScoreUploaded();
        }
        public void OnLeaderboardDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            this.rootObject.SetActive(false);
        }
        public void OnScoreUploaded()
        {
            if (!this._playerDataManager._initFinish)
                return;
            _ = this.ScoreUploadedAsync();
        }
        public async Task ScoreUploadedAsync()
        {
            await this._playerDataManager.GetSSPlayerInfoAsync();
            await this._rankingData.GetUserRankingAsync(this._playerDataManager._userID);
            this.OnPlayCountChange();
            this.OnRankPpChange();
        }
        public void OnBLScoreUploaded()
        {
            if (!this._playerDataManager._initFinish)
                return;
            _ = this.BLScoreUploadedAsync();
        }
        public async Task BLScoreUploadedAsync()
        {
            await this._playerDataManager.GetBLPlayerInfoAsync();
            this.OnPlayCountChange();
            this.OnRankPpChange();
        }
    }
}
