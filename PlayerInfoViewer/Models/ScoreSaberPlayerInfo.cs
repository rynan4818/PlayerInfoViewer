using Newtonsoft.Json;
using PlayerInfoViewer.Models.ScoreSaber;
using PlayerInfoViewer.Util;
using System;
using System.Globalization;
using System.Threading.Tasks;

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
            var playerFullInfoURL = $"https://scoresaber.com/api/v2/players/{userID}";
            try
            {
                var resJsonString = await HttpUtility.GetHttpContentAsync(playerFullInfoURL);
                if (resJsonString == null)
                    throw new Exception("ScoreSaber Player full info get error");

                var playerProfile = JsonConvert.DeserializeObject<PlayerProfileV2Json>(resJsonString);
                if (playerProfile == null || playerProfile.stats == null)
                    throw new Exception("ScoreSaber Player v2 info parse error");

                this._playerFullInfo = ConvertToPlayerFullInfo(playerProfile);
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

        private static PlayerFullInfoJson ConvertToPlayerFullInfo(PlayerProfileV2Json playerProfile)
        {
            return new PlayerFullInfoJson
            {
                id = playerProfile.id,
                pp = Convert.ToSingle(playerProfile.stats.totalPP),
                rank = playerProfile.stats.rank,
                countryRank = playerProfile.stats.countryRank,
                scoreStats = new ScoreSaberScoreStats
                {
                    totalScore = ParseLong(playerProfile.stats.totalScore),
                    totalRankedScore = ParseLong(playerProfile.stats.totalRankedScore),
                    averageRankedAccuracy = Convert.ToSingle(playerProfile.stats.averageAccuracy),
                    totalPlayCount = playerProfile.stats.totalPlayedLeaderboards,
                    rankedPlayCount = playerProfile.stats.totalPlayedRankedLeaderboards,
                    replaysWatched = playerProfile.stats.totalReplayViews
                }
            };
        }

        private static long ParseLong(string value)
        {
            long parsedValue;
            return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue) ? parsedValue : 0;
        }
    }
}
