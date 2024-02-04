using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using PlayerInfoViewer.Util;
using PlayerInfoViewer.Models.BeatLeader;

namespace PlayerInfoViewer.Models
{
    public class BeatLeaderPlayerInfo
    {
        public bool _playerInfoGetActive { get; set; } = false;
        public PlayerResponseFullJson _playerInfo { get; set; }
        public async Task GetPlayerInfoAsync(string userID)
        {
            if (userID == null || this._playerInfoGetActive)
                return;
            this._playerInfoGetActive = true;
            this._playerInfo = null;
            var playerResponseFullURL = $"https://api.beatleader.xyz/player/{userID}?stats=true&keepOriginalId=false";
            try
            {
                var resJsonString = await HttpUtility.GetHttpContentAsync(playerResponseFullURL);
                if (resJsonString == null)
                    throw new Exception("BeatLeader Player info get error");
                this._playerInfo = JsonConvert.DeserializeObject<PlayerResponseFullJson>(resJsonString);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                this._playerInfo = null;
                this._playerInfoGetActive = false;
                return;
            }
            this._playerInfoGetActive = false;
            return;
        }
    }
}