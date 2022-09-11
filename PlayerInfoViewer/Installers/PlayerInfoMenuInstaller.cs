using PlayerInfoViewer.Views;
using Zenject;

namespace PlayerInfoViewer.Installers
{
    public class PlayerInfoMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<PlayerInfoView>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
        }
    }
}
