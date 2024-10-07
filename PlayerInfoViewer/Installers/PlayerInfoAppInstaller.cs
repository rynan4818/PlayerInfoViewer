using PlayerInfoViewer.Models;
using PlayerInfoViewer.Util;
using Zenject;

namespace PlayerInfoViewer.Installers
{
    public class PlayerInfoAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<PlayerDataManager>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ScoreSaberRanking>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ScoreSaberPlayerInfo>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<BeatLeaderPlayerInfo>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<PlayerHttpStatus>().AsSingle().NonLazy();
        }
    }
}
