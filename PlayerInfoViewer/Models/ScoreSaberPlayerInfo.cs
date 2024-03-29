﻿using Newtonsoft.Json;
using PlayerInfoViewer.Util;
using System.Threading.Tasks;
using System;
using PlayerInfoViewer.Models.ScoreSaber;

namespace PlayerInfoViewer.Models
{
    public class ScoreSaberPlayerInfo
    {
        public bool _playerInfoGetActive = false;
        public PlayerFullInfoJson _playerFullInfo;
        public async Task GetPlayerFullInfoAsync(string userID)
        {
            if (userID == null || this._playerInfoGetActive)
                return;
            this._playerInfoGetActive = true;
            this._playerFullInfo = null;
            var playerFullInfoURL = $"https://scoresaber.com/api/player/{userID}/full";
            try
            {
                var resJsonString = await HttpUtility.GetHttpContentAsync(playerFullInfoURL);
                if (resJsonString == null)
                    throw new Exception("ScoreSaber Player full info get error");
                this._playerFullInfo = JsonConvert.DeserializeObject<PlayerFullInfoJson>(resJsonString);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                this._playerInfoGetActive = false;
                return;
            }
            this._playerInfoGetActive = false;
            return;
        }
    }
}
