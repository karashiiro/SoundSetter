using System;
using Dalamud.Plugin;
using SoundSetter.Attributes;

namespace SoundSetter
{
    public class SoundSetterPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface pluginInterface;
        private PluginCommandManager<SoundSetterPlugin> commandManager;
        //private SoundSetterUi ui;
        private VolumeControls vc;

        public string Name => "SoundSetter";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            
            this.vc = new VolumeControls(this.pluginInterface.TargetModuleScanner);

            //this.ui = new SoundSetterUi(this.vc);
            //this.pluginInterface.UiBuilder.OnBuildUi += this.ui.Draw;

            this.pluginInterface.UiBuilder.OnOpenConfigUi += ToggleConfig;
            
            this.commandManager = new PluginCommandManager<SoundSetterPlugin>(this, this.pluginInterface);
        }

        [Command("/soundsetterconfig")]
        [Aliases("/ssconfig")]
        [HelpMessage("Open the SoundSetter configuration.")]
        public void SoundSetterConfigCommand(string command, string args)
        {
            ToggleConfig();
        }

        private void ToggleConfig(object sender = null, EventArgs args = null)
        {
            //this.ui.IsVisible = !this.ui.IsVisible;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.commandManager.Dispose();

            this.pluginInterface.UiBuilder.OnOpenConfigUi -= ToggleConfig;

            //this.pluginInterface.UiBuilder.OnBuildUi -= this.ui.Draw;

            this.vc.Dispose();

            this.pluginInterface.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
