using NjsJdRtDisplay.UI;
using Zenject;

namespace NjsJdRtDisplay.Installers
{
    internal class NjsJdRtMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NjsJdRtSettingsController>().AsSingle();
        }
    }
}
