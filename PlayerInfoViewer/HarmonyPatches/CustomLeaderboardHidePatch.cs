using HarmonyLib;
using LeaderboardCore.Models;

namespace PlayerInfoViewer.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLeaderboard))]
    [HarmonyPatch("Hide", MethodType.Normal)]
    public class CustomLeaderboardHidePatch
    {
        public static void Postfix(object __instance)
        {
            CustomLeaderboardShowPatch.LeaderboardId = Traverse.Create(__instance).Property("LeaderboardId").GetValue() as string;
            Plugin.Log.Debug("Hide:" + CustomLeaderboardShowPatch.LeaderboardId);
        }
    }
}
