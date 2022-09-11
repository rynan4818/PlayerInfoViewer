using HMUI;
using LeaderboardCore.Interfaces;
using PlayerInfoViewer.Models;
using System.Linq;
using TMPro;
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
        private GameObject rootObject;
        private Canvas _canvas;
        private CurvedCanvasSettings _curvedCanvasSettings;
        private CurvedTextMeshPro _playCount;
        private PlayerDataManager _playerDataManager;

        private static readonly Vector2 CanvasSize = new Vector2(100, 50);
        private static readonly Vector3 scale = new Vector3(0.01f, 0.01f, 0.01f);
        private static readonly Vector3 RightPosition = new Vector3(3.5f, 3.3f, 2.5f);
        private static readonly Vector3 RightRotation = new Vector3(0, 56, 0);

        //MonoBehaviourはコンストラクタを使えないので、メソッドでインジェクションする
        [Inject]
        public void Constractor(PlayerDataManager playerDataManager, PlatformLeaderboardViewController platformLeaderboardViewController)
        {
            this._playerDataManager = playerDataManager;
            this._platformLeaderboardViewController = platformLeaderboardViewController;
        }
        private void Awake()
        {
            this.rootObject = new GameObject("PlayCount Canvas", typeof(Canvas), typeof(CurvedCanvasSettings), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            var sizeFitter = this.rootObject.GetComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            this._canvas = this.rootObject.GetComponent<Canvas>();
            this._canvas.sortingOrder = 3;
            this._curvedCanvasSettings = this.rootObject.GetComponent<CurvedCanvasSettings>();
            this._curvedCanvasSettings.SetRadius(600f);
            this._canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = this._canvas.transform as RectTransform;
            rectTransform.sizeDelta = CanvasSize;
            this.rootObject.transform.position = RightPosition;
            this.rootObject.transform.eulerAngles = RightRotation;
            this.rootObject.transform.localScale = scale;
            this._playCount = this.CreateText(this._canvas.transform as RectTransform, string.Empty, new Vector2(10, 31));
            rectTransform = this._playCount.transform as RectTransform;
            rectTransform.SetParent(this._canvas.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            this._playCount.fontSize = 14;
            this._playCount.color = Color.white;
            this._playCount.text = "PlayCountViewer: Load data...";
            this._platformLeaderboardViewController.didActivateEvent += OnLeaderboardActivated;
            this._platformLeaderboardViewController.didDeactivateEvent += OnLeaderboardDeactivated;
            this._playerDataManager.OnPlayCountChange += OnPlayCountChange;
            this._playerDataManager.PlayerInfoCheck();
            rootObject.SetActive(false);
        }
        private void OnDestroy()
        {
            this._playerDataManager.OnPlayCountChange -= OnPlayCountChange;
            this._platformLeaderboardViewController.didDeactivateEvent -= OnLeaderboardDeactivated;
            this._platformLeaderboardViewController.didActivateEvent -= OnLeaderboardActivated;
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
            textMesh.font = Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF Numbers Monospaced Curved"));
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
        public void OnPlayCountChange()
        {
            if (this._playerDataManager._playerFullInfo == null)
                return;
            var playCount = this._playerDataManager._playerFullInfo.scoreStats.totalPlayCount;
            this._playCount.text = $"Play Count:{playCount}";
        }
        public void OnLeaderboardActivated(bool firstactivation, bool addedtohierarchy, bool screensystemenabling)
        {
            rootObject.SetActive(true);
        }
        public void OnLeaderboardDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            rootObject.SetActive(false);
        }
        public async void OnScoreUploaded()
        {
            await this._playerDataManager.GetPlayerFullInfo();
            OnPlayCountChange();
        }
    }
}
