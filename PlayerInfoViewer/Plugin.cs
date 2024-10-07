using PlayerInfoViewer.Installers;
using PlayerInfoViewer.Configuration;
using PlayerInfoViewer.HarmonyPatches;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;
using System.Reflection;
using IPA.Loader;

namespace PlayerInfoViewer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Harmony _harmony;
        public const string HARMONY_ID = "com.github.rynan4818.PlayerInfoViewer";
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal PluginMetadata leaderboardCore { get; private set; }

        /// <summary>
        /// IPAによってプラグインが最初にロードされたときに呼び出されます。
        /// （ゲームが開始されたとき、またはプラグインが無効の状態で開始された場合は有効化されたときのいずれか）
        /// [Init]はコンストラクタのメソッド、InitWithConfig のような通常のメソッドの前に呼び出されます。
        /// [Init]は１つのコンストラクタのみを使用して下さい。
        /// </summary>
        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            Log.Debug("Initialized.");
            _harmony = new Harmony(HARMONY_ID);
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            zenjector.Install<PlayerInfoAppInstaller>(Location.App);
            zenjector.Install<PlayerInfoMenuInstaller>(Location.Menu);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Info("OnApplicationStart");
            MethodInfo patch;
            var orginal = AccessTools.Method("CO2Core.Models.CO2CoreManager:UpdateCO2");
            if (orginal != null)
            {
                patch = AccessTools.Method(typeof(CO2CoreManagerPatch), nameof(CO2CoreManagerPatch.UpdateCO2Postfix));
                Log.Debug("CO2CoreManager Patch Load");
                _harmony.Patch(orginal, null, new HarmonyMethod(patch));
            }
            orginal = AccessTools.Method("HeadDistanceTravelled.HeadDistanceTravelledController:OnDestroy");
            if (orginal != null)
            {
                patch = AccessTools.Method(typeof(HeadDistanceTravelledControllerPatch), nameof(HeadDistanceTravelledControllerPatch.OnDestroyPrefix));
                Log.Debug("HeadDistanceTravelledController Patch Load");
                _harmony.Patch(orginal, new HarmonyMethod(patch));
                HeadDistanceTravelledControllerPatch.Enable = true;
            }
            var type = AccessTools.TypeByName("BeatLeader.API.Methods.UploadReplayRequest");
            if (type != null)
                type = AccessTools.Inner(type, "UploadWithCookieRequestDescriptor");
            if (type != null)
            {
                orginal = AccessTools.Method(type, "ParseResponse");
                patch = AccessTools.Method(typeof(UploadReplayRequestPatch), nameof(UploadReplayRequestPatch.ParseResponsePostfix));
                if (orginal != null)
                {
                    Log.Debug("BeatLeader UploadReplayRequest Patch Load");
                    _harmony.Patch(orginal, null, new HarmonyMethod(patch));
                }
            }
            type = AccessTools.TypeByName("BeatLeader.API.Methods.UploadPlayRequest");
            if (type != null)
                type = AccessTools.Inner(type, "UploadWithCookieRequestDescriptor");
            if (type != null)
            {
                orginal = AccessTools.Method(type, "ParseResponse");
                patch = AccessTools.Method(typeof(UploadPlayRequestPatch), nameof(UploadPlayRequestPatch.ParseResponsePostfix));
                if (orginal != null)
                {
                    Log.Debug("BeatLeader UploadPlayRequest Patch Load");
                    _harmony.Patch(orginal, null, new HarmonyMethod(patch));
                }
            }
            leaderboardCore = PluginManager.GetPluginFromId("LeaderboardCore");
            if (leaderboardCore == null)
                leaderboardCore = PluginManager.GetDisabledPluginFromId("LeaderboardCore");
            if (leaderboardCore != null)
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
            _harmony?.UnpatchSelf();
        }
    }
}
