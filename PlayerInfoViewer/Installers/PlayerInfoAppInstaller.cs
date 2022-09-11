using PlayerInfoViewer.Models;
using Zenject;

namespace PlayerInfoViewer.Installers
{
    public class PlayerInfoAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<PlayerDataManager>().AsSingle().NonLazy();
        }
    }
}
