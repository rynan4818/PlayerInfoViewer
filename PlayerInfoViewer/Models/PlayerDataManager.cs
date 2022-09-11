using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Zenject;

namespace PlayerInfoViewer.Models
{
    public class PlayerDataManager : IInitializable
    {
        private readonly IPlatformUserModel _userModel;
        public bool _playerInfoGetActive = false;
        public string _userID;
        public PlayerFullInfoJson _playerFullInfo;
        public event Action OnPlayCountChange;

        public PlayerDataManager(IPlatformUserModel userModel)
        {
            _userModel = userModel;
        }

        public async void Initialize()
        {
            var userInfo = await _userModel.GetUserInfo();
            _userID = userInfo.platformUserId;
            await GetPlayerFullInfo();
            OnPlayCountChange?.Invoke();
        }
        public void PlayerInfoCheck()
        {
            if (_playerFullInfo == null || _playerInfoGetActive) return;
            OnPlayCountChange?.Invoke();
        }
        public async Task GetPlayerFullInfo()
        {
            if (_userID == null || _playerInfoGetActive)
                return;
            _playerInfoGetActive = true;
            var playerFullInfoURL = $"https://scoresaber.com/api/player/{_userID}/full";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(playerFullInfoURL);
            var resJsonString = await response.Content.ReadAsStringAsync();
            _playerFullInfo = JsonConvert.DeserializeObject<PlayerFullInfoJson>(resJsonString);
            _playerInfoGetActive = false;
        }
    }
}
