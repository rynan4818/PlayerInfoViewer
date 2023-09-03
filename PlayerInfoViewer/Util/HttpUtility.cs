using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerInfoViewer.Util
{
    public static class HttpUtility
    {
        public static readonly HttpClient httpClient = new HttpClient();
        public static async Task<string> GetHttpContentAsync(string url)
        {
            try
            {
                return await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Plugin.Log.Error($"{url} Http Error : {e.Message}");
                return null;
            }
            catch (TaskCanceledException e)
            {
                Plugin.Log.Error($"{url} Http Cancel : {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Plugin.Log.Error($"{url} Http other Error : {e.Message}");
                return null;
            }
        }
    }
}
