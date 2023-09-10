using HarmonyLib;
using LeaderboardCore.Models;
using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomLeaderboard))]
    [HarmonyPatch("Show", MethodType.Normal)]
    public class CustomLeaderboardShowPatch
    {
        public static event Action<string> OnCustomLeaderboardShowed;
        public static void Postfix(object __instance)
        {
            OnCustomLeaderboardShowed?.Invoke(Traverse.Create(__instance).Property("LeaderboardId").GetValue() as string);
        }
    }
}
