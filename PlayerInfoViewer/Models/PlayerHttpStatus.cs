using HttpSiraStatus.Enums;
using HttpSiraStatus.Interfaces;
using HttpSiraStatus.Util;

namespace PlayerInfoViewer.Models
{
    public class PlayerHttpStatus
    {
        private readonly IStatusManager _statusManager;
        private ScoreSaberPlayerInfo _scoreSaberPlayerInfo;
        public PlayerHttpStatus(IStatusManager statusManager, ScoreSaberPlayerInfo scoreSaberPlayerInfo)
        {
            this._statusManager = statusManager;
            this._scoreSaberPlayerInfo = scoreSaberPlayerInfo;
        }

        public void OnPlayerSend()
        {
            var rootObj = new JSONObject();
            rootObj["totalPlayCount"] = this._scoreSaberPlayerInfo._playerFullInfo.scoreStats.totalPlayCount;
            rootObj["pp"] = this._scoreSaberPlayerInfo._playerFullInfo.pp;
            rootObj["name"] = this._scoreSaberPlayerInfo._playerFullInfo.name;
            rootObj["country"] = this._scoreSaberPlayerInfo._playerFullInfo.country;
            rootObj["countryRank"] = this._scoreSaberPlayerInfo._playerFullInfo.countryRank;
            rootObj["rank"] = this._scoreSaberPlayerInfo._playerFullInfo.rank;
            this._statusManager.OtherJSON["playerInfo"] = rootObj;
            this._statusManager.EmitStatusUpdate(ChangedProperty.Other, BeatSaberEvent.Other);
        }
    }
}
