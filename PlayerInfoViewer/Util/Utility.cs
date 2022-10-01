using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlayerInfoViewer.Util
{
    public static class Utility
    {
        public static async Task<string> GetHttpContent(HttpClient HttpClient, string url)
        {
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.GetAsync(url);
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
