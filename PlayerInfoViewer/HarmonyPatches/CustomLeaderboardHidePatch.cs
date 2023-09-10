using HarmonyLib;
using LeaderboardCore.Models;
using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLeaderboard))]
    [HarmonyPatch("Hide", MethodType.Normal)]
    public class CustomLeaderboardHidePatch
    {
        public static event Action<string> OnCustomLeaderboardHidden;
        public static void Postfix(object __instance)
        {
            OnCustomLeaderboardHidden?.Invoke(Traverse.Create(__instance).Property("LeaderboardId").GetValue() as string);
        }
    }
}
