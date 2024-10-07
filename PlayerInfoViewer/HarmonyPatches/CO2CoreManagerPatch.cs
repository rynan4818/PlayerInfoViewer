using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    public class CO2CoreManagerPatch
    {
        public static event Action<(int, double, double)> OnCO2Changed;
        public static bool Enable = false;
        public static void UpdateCO2Postfix(int co2, double hum, double tmp)
        {
            OnCO2Changed?.Invoke((co2, hum, tmp));
            Enable = true;
        }
    }
}
