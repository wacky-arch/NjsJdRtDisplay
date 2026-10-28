using NjsJdRtDisplay.Controllers;
using Zenject;

namespace NjsJdRtDisplay.Installers
{
    internal class NjsJdRtPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NjsJdRtGameplayController>().AsSingle();
        }
    }
}
