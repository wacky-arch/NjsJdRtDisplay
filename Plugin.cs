using IPA;
using IPA.Config;
using IPA.Config.Stores;
using NjsJdRtDisplay.Configuration;
using NjsJdRtDisplay.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace NjsJdRtDisplay
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }

        [Init]
        public Plugin(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Log = logger;
            PluginConfig.Instance = conf.Generated<PluginConfig>();

            zenjector.UseLogger(logger);
            zenjector.Install<NjsJdRtMenuInstaller>(Location.Menu);
            zenjector.Install<NjsJdRtPlayerInstaller>(Location.Player);
        }
    }
}
