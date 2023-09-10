using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    public class UploadReplayRequestPatch
    {
        public static event Action OnUploadReplayFinished;
        public static void ParseResponsePostfix()
        {
            OnUploadReplayFinished?.Invoke();
        }
    }
}
