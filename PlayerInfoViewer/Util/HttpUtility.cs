using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerInfoViewer.Util
{
    public static class HttpUtility
    {
        public static readonly HttpClient httpClient = new HttpClient();
        public static async Task<string> GetHttpContent(string url)
        {
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(url);
            }
            catch (HttpRequestException)
            {
                Plugin.Log.Error($"{url} Http Error");
                return null;
            }
            catch (TaskCanceledException)
            {
                Plugin.Log.Error($"{url} Http Cancel");
                return null;
            }
            catch (Exception)
            {
                Plugin.Log.Error($"{url} Other Error");
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
