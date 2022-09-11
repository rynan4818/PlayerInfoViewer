using PlayerInfoViewer.Installers;
using PlayerInfoViewer.Configuration;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System;
using IPALogger = IPA.Logging.Logger;

namespace PlayerInfoViewer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

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
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            zenjector.Install<PlayerInfoAppInstaller>(Location.App);
            zenjector.Install<PlayerInfoMenuInstaller>(Location.Menu);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Info("OnApplicationStart");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            PluginConfig.Instance.LastPlayTime = DateTime.Now.ToString();
            Log.Debug("OnApplicationQuit");
        }
    }
}
