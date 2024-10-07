using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    public class HeadDistanceTravelledControllerPatch
    {
        public static event Action<float> OnHDTUpdate;
        public static bool Enable = false;
        public static void OnDestroyPrefix(object __instance)
        {
            OnHDTUpdate?.Invoke((float)__instance.GetType().GetProperty("HMDDistance").GetValue(__instance));
        }
    }
}
