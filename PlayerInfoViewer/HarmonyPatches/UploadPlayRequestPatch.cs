using System;

namespace PlayerInfoViewer.HarmonyPatches
{
    public class UploadPlayRequestPatch
    {
        public static event Action OnUploadPlayFinished;
        public static void ParseResponsePostfix()
        {
            OnUploadPlayFinished?.Invoke();
        }
    }
}
