using HarmonyLib;
using LeaderboardCore.Models;

namespace PlayerInfoViewer.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLeaderboard))]
    [HarmonyPatch("Show", MethodType.Normal)]
    public class CustomLeaderboardShowPatch
    {
        public static string LeaderboardId;
        public static void Postfix(object __instance)
        {
            LeaderboardId = Traverse.Create(__instance).Property("LeaderboardId").GetValue() as string;
            Plugin.Log.Debug("Show:" + LeaderboardId);
        }
    }
}
