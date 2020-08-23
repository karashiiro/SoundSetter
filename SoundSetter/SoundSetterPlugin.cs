using Dalamud.Plugin;
using SoundSetter.Attributes;
using System;

namespace SoundSetter
{
    public class SoundSetterPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface pluginInterface;
        private PluginCommandManager<SoundSetterPlugin> commandManager;

        private Configuration config;
        private SoundSetterUi ui;
        private VolumeControls vc;

        public string Name => "SoundSetter";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            this.config = (Configuration)this.pluginInterface.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(this.pluginInterface);

            this.vc = new VolumeControls(this.pluginInterface.TargetModuleScanner);

            this.ui = new SoundSetterUi(this.vc, this.config);
            this.pluginInterface.UiBuilder.OnBuildUi += this.ui.Draw;
            this.pluginInterface.UiBuilder.OnBuildUi += OnTick;

            this.pluginInterface.UiBuilder.OnOpenConfigUi += ToggleConfig;

            this.commandManager = new PluginCommandManager<SoundSetterPlugin>(this, this.pluginInterface);
        }

        private bool keysDown;
        private void OnTick()
        {
            // We don't want to open the UI before the player loads, that leaves the options uninitialized.
            if (this.pluginInterface.ClientState.LocalPlayer == null) return;

            if (this.pluginInterface.ClientState.KeyState[(byte)this.config.ModifierKey] &&
                this.pluginInterface.ClientState.KeyState[(byte)this.config.MajorKey])
            {
                if (this.keysDown) return;

                this.keysDown = true;
                ToggleConfig();
                return;
            }

            this.keysDown = false;
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
            this.ui.IsVisible = !this.ui.IsVisible;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.commandManager.Dispose();

            this.pluginInterface.UiBuilder.OnOpenConfigUi -= ToggleConfig;

            this.pluginInterface.UiBuilder.OnBuildUi -= OnTick;
            this.pluginInterface.UiBuilder.OnBuildUi -= this.ui.Draw;

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
