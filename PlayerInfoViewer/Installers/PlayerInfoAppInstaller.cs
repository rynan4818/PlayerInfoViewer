using PlayerInfoViewer.Models;
using Zenject;

namespace PlayerInfoViewer.Installers
{
    public class PlayerInfoAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<PlayerDataManager>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<HDTDataJson>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ScoreSaberRanking>().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ScoreSaberPlayerInfo>().AsSingle().NonLazy();
        }
    }
}
