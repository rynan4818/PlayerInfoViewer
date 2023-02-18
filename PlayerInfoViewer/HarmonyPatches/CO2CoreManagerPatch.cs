using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    public class CO2CoreManagerPatch
    {
        public static void UpdateCO2Postfix(ref object __instance)
        {
            var co2 = (int)__instance.GetType().GetProperty("CO2").GetValue(__instance);
            var hum = (double)__instance.GetType().GetProperty("HUM").GetValue(__instance);
            var tmp = (double)__instance.GetType().GetProperty("TMP").GetValue(__instance);
            OnCO2Changed?.Invoke((co2, hum, tmp));
            Enable = true;
        }
        public static event Action<(int, double, double)> OnCO2Changed;
        public static bool Enable = false;
    }
}
