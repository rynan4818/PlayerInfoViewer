using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    public static class ScoreSaberPanelViewSetPromptPatch
    {
        public static event Action OnScoreUploaded;

        public static void Postfix(string status)
        {
            if (string.IsNullOrEmpty(status) || status.IndexOf("Score uploaded", StringComparison.OrdinalIgnoreCase) < 0)
                return;

            OnScoreUploaded?.Invoke();
        }
    }
}
