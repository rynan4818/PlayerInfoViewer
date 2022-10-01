using HMUI;
using LeaderboardCore.Interfaces;
using PlayerInfoViewer.Configuration;
using PlayerInfoViewer.Models;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        private ScoreSaberRankingJson _rankingData;
        private HDTDataJson _hdtDataJson;
        public GameObject rootObject;
        private Canvas _canvas;
        private CurvedTextMeshPro _playerStatistics;
        private CurvedTextMeshPro _playCount;
        private CurvedTextMeshPro _rankPP;
        public float lastPlayed;

        public static readonly Vector2 CanvasSize = new Vector2(100, 50);
        public static readonly Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);
        public static readonly Vector3 RightPosition = new Vector3(3.5f, 3.4f, 2.2f);
        public static readonly Vector3 RightRotation = new Vector3(0, 56, 0);

        //MonoBehaviourはコンストラクタを使えないので、メソッドでインジェクションする
        [Inject]
        public void Constractor(PlayerDataManager playerDataManager, PlatformLeaderboardViewController platformLeaderboardViewController, PlayerDataModel playerDataModel, HDTDataJson hdtDataJson, ScoreSaberRankingJson rankingData)
        {
            this._playerDataManager = playerDataManager;
            this._platformLeaderboardViewController = platformLeaderboardViewController;
            this._playerDataModel = playerDataModel;
            this._hdtDataJson = hdtDataJson;
            this._rankingData = rankingData;
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
            this._playerDataManager.OnPlayerDataInitFinish += this.OnPlayerDataInitFinish;
            this.OnPlayerDataInitFinish();
            this.rootObject.SetActive(false);
        }
        private void OnDestroy()
        {
            Plugin.Log.Debug("PlayerInfoView Destroy");
            this._playerDataManager.OnPlayerDataInitFinish -= this.OnPlayerDataInitFinish;
            this._platformLeaderboardViewController.didDeactivateEvent -= this.OnLeaderboardDeactivated;
            this._platformLeaderboardViewController.didActivateEvent -= this.OnLeaderboardActivated;
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
        public void PlyerStatisticsChange()
        {
            if (!PluginConfig.Instance.ViewPlayerStatistics)
                return;
            var allOverallStatsData = this._playerDataModel.playerData.playerAllOverallStatsData.allOverallStatsData;
            if (this.lastPlayed == allOverallStatsData.playedLevelsCount)
                return;
            this.lastPlayed = allOverallStatsData.playedLevelsCount;
            var todayPlayed = String.Format("{0:+0;-0;+0}", allOverallStatsData.playedLevelsCount - PluginConfig.Instance.LastPlayedLevelsCount);
            var todayCleared = String.Format("{0:+0;-0;+0}", allOverallStatsData.clearedLevelsCount - PluginConfig.Instance.LastClearedLevelsCount);
            var todayFailed = String.Format("{0:+0;-0;+0}", allOverallStatsData.failedLevelsCount - PluginConfig.Instance.LastFailedLevelsCount);
            var todayFullCombo = String.Format("{0:+0;-0;+0}", allOverallStatsData.fullComboCount - PluginConfig.Instance.LastFullComboCount);
            var todayTimePlayed = String.Format("{0:+0.#;-0.#;+0}", (allOverallStatsData.timePlayed - PluginConfig.Instance.LastTimePlayed) / 60f);
            var todayHandDistanceTravelled = String.Format("{0:+0.#;-0.#;+0}", (float)(allOverallStatsData.handDistanceTravelled - PluginConfig.Instance.LastHandDistanceTravelled) / 1000f);
            var todayHeadDistanceTravelled = "";
            if (this._hdtDataJson.hdtEnable)
                todayHeadDistanceTravelled = String.Format("  Head : {0:+0;-0;+0}m", this._hdtDataJson.HeadDistanceTravelled - PluginConfig.Instance.LastHeadDistanceTravelled);
            this._playerStatistics.text = $"Play : {todayPlayed}  Clear : {todayCleared}  Fail : {todayFailed}  FC : {todayFullCombo}  Time : {todayTimePlayed}m  Hand : {todayHandDistanceTravelled}km{todayHeadDistanceTravelled}";
        }
        public void OnPlayCountChange()
        {
            if (!PluginConfig.Instance.ViewPlayCount)
                return;
            if (this._playerDataManager._playerFullInfo == null)
            {
                this._playCount.color = Color.red;
                this._playCount.text = " ScoreSaber Error!";
                return;
            }
            else
                this._playCount.color = Color.white;
            var playCount = this._playerDataManager._playerFullInfo.scoreStats.totalPlayCount;
            var rankedPlayCount = this._playerDataManager._playerFullInfo.scoreStats.rankedPlayCount;
            var todayPlayCount = String.Format("{0:+#;-#;#}", playCount - PluginConfig.Instance.LastTotalPlayCount);
            var todayRankedPlayCount = String.Format("{0:+#;-#;#}", rankedPlayCount - PluginConfig.Instance.LastRankedPlayCount);
            var totalPlayCountRankObject = this._rankingData.GetRankingData("TotalPlayCountRank");
            string totalPlayCountRank = "";
            if (totalPlayCountRankObject != null)
                totalPlayCountRank = $"   #{totalPlayCountRankObject}";
            var totalPlayCountLocalRankObject = this._rankingData.GetRankingData("TotalPlayCountLocalRank");
            if (totalPlayCountLocalRankObject != null)
                totalPlayCountRank += $"/#{totalPlayCountLocalRankObject}";
            var rankedPlayCountRankObject = this._rankingData.GetRankingData("RankedPlayCountRank");
            string rankedPlayCountRank = "";
            if (rankedPlayCountRankObject != null)
                rankedPlayCountRank = $"   #{rankedPlayCountRankObject}";
            var rankedPlayCountLocalRankObject = this._rankingData.GetRankingData("RankedPlayCountLocalRank");
            if (rankedPlayCountLocalRankObject != null)
                rankedPlayCountRank += $"/#{rankedPlayCountLocalRankObject}";
            this._playCount.text = $"Play Count    Total : {playCount} {todayPlayCount}{totalPlayCountRank}    Ranked : {rankedPlayCount} {todayRankedPlayCount}{rankedPlayCountRank}";
        }
        public void OnRankPpChange()
        {
            if (!PluginConfig.Instance.ViewRankPP)
                return;
            if (this._playerDataManager._playerFullInfo == null)
            {
                this._rankPP.color = Color.red;
                this._rankPP.text = " ScoreSaber Error!";
                return;
            }
            else
                this._rankPP.color = Color.white;
            var pp = this._playerDataManager._playerFullInfo.pp;
            var localRank = this._playerDataManager._playerFullInfo.countryRank;
            var rank = this._playerDataManager._playerFullInfo.rank;
            var todayRankUp = String.Format("{0:+#;-#;+0}", PluginConfig.Instance.LastRank - rank);
            var todayLocalRankUp = String.Format("{0:+#;-#;#}", PluginConfig.Instance.LastCountryRank - localRank);
            var todayPpUp = String.Format("{0:+0.##;-0.##;+0.##}", Math.Round(pp - PluginConfig.Instance.LastPP, 2, MidpointRounding.AwayFromZero));
            var lastChangePp = String.Format("{0:+0.##;-0.##;+0.##}", Math.Round(PluginConfig.Instance.NowPP - PluginConfig.Instance.BeforePP, 2, MidpointRounding.AwayFromZero));
            this._rankPP.text = $"Global : {todayRankUp}    Local : #{localRank} {todayLocalRankUp}    Today : {todayPpUp}pp  Last : {lastChangePp}pp";
        }
        public void OnPlayerDataInitFinish()
        {
            this.PlyerStatisticsChange();
            this.OnPlayCountChange();
            this.OnRankPpChange();
        }
        public void OnLeaderboardActivated(bool firstactivation, bool addedtohierarchy, bool screensystemenabling)
        {
            this.rootObject.SetActive(true);
            this.PlyerStatisticsChange();
            this._hdtDataJson.Load();
            if (this._playerDataManager._playerFullInfo == null && !this._playerDataManager._playerInfoGetActive)
                this.OnScoreUploaded();
        }
        public void OnLeaderboardDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            this.rootObject.SetActive(false);
        }
        public async void OnScoreUploaded()
        {
            await this._playerDataManager.GetPlayerFullInfo();
            await this._rankingData.GetUserRanking(this._playerDataManager._userID);
            this.OnPlayCountChange();
            this.OnRankPpChange();
        }
    }
}
