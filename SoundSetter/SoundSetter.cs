using Dalamud.Plugin;
using SoundSetter.Attributes;
using System;
using System.Linq;
using System.Text;
using Dalamud.Game.Text;
using Dalamud.Game.ClientState;
using Dalamud.Game.Internal.Gui;
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SoundSetter
{
    public class SoundSetter : IDalamudPlugin
    {
        private DalamudPluginInterface pluginInterface;
        private PluginCommandManager<SoundSetter> commandManager;

        private Configuration config;
        private SoundSetterUi ui;
        private VolumeControls vc;

        public string Name => "SoundSetter";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            this.config = (Configuration)this.pluginInterface.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(this.pluginInterface);

            this.vc = new VolumeControls(this.pluginInterface.TargetModuleScanner, this.pluginInterface.SendMessage);

            this.pluginInterface.UiBuilder.DisableAutomaticUiHide = true;

            this.ui = new SoundSetterUi(this.vc, this.pluginInterface, this.config);
            this.pluginInterface.UiBuilder.OnBuildUi += this.ui.Draw;
            this.pluginInterface.UiBuilder.OnBuildUi += OnTick;

            this.pluginInterface.UiBuilder.OnOpenConfigUi += ToggleConfig;

            this.commandManager = new PluginCommandManager<SoundSetter>(this, this.pluginInterface);
        }

        private bool keysDown;
        private void OnTick()
        {
            // We don't want to open the UI before the player loads, that leaves the options uninitialized.
            if (this.pluginInterface.ClientState.LocalContentId == 0) return;

            var cutsceneActive = this.pluginInterface.ClientState.Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
                                     this.pluginInterface.ClientState.Condition[ConditionFlag.WatchingCutscene] ||
                                     this.pluginInterface.ClientState.Condition[ConditionFlag.WatchingCutscene78];

            if (this.config.OnlyShowInCutscenes && !cutsceneActive) return;

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
            => ToggleConfig();

        private void ToggleConfig(object sender = null, EventArgs args = null)
            => this.ui.IsVisible = !this.ui.IsVisible;

        private const string MasterVolumeAdjustCommand = "/ssmv";
        [Command(MasterVolumeAdjustCommand)]
        [HelpMessage("Adjust the game's master volume by the specified quantity.")]
        public void MasterVolumeAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.MasterVolumeMuted.SetValue(true);
                chat.Print("Master volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.MasterVolumeMuted.SetValue(false);
                chat.Print("Master volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, MasterVolumeAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.MasterVolume, volumeTarget, op);
            chat.Print($"Master volume set to {this.vc.MasterVolume.GetValue()}.");
        }

        private const string BgmAdjustCommand = "/ssbgm";
        [Command(BgmAdjustCommand)]
        [HelpMessage("Adjust the game's BGM volume by the specified quantity.")]
        public void BgmAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.BgmMuted.SetValue(true);
                chat.Print("BGM volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.BgmMuted.SetValue(false);
                chat.Print("BGM volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, BgmAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.Bgm, volumeTarget, op);
            chat.Print($"BGM volume set to {this.vc.Bgm.GetValue()}.");
        }

        private const string SoundEffectsAdjustCommand = "/sssfx";
        [Command(SoundEffectsAdjustCommand)]
        [HelpMessage("Adjust the game's SFX volume by the specified quantity.")]
        public void SoundEffectsAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.SoundEffectsMuted.SetValue(true);
                chat.Print("SFX volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.SoundEffectsMuted.SetValue(false);
                chat.Print("SFX volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, SoundEffectsAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.SoundEffects, volumeTarget, op);
            chat.Print($"SFX volume set to {this.vc.SoundEffects.GetValue()}.");
        }

        private const string VoiceAdjustCommand = "/ssv";
        [Command(VoiceAdjustCommand)]
        [HelpMessage("Adjust the game's voice volume by the specified quantity.")]
        public void VoiceAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.VoiceMuted.SetValue(true);
                chat.Print("Voice volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.VoiceMuted.SetValue(false);
                chat.Print("Voice volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, VoiceAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.Voice, volumeTarget, op);
            chat.Print($"Voice volume set to {this.vc.Voice.GetValue()}.");
        }

        private const string SystemSoundsAdjustCommand = "/sssys";
        [Command(SystemSoundsAdjustCommand)]
        [HelpMessage("Adjust the game's system sound volume by the specified quantity.")]
        public void SystemSoundsAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.SystemSoundsMuted.SetValue(true);
                chat.Print("System sounds muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.SystemSoundsMuted.SetValue(false);
                chat.Print("System sounds unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, SystemSoundsAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.SystemSounds, volumeTarget, op);
            chat.Print($"System sound volume set to {this.vc.SystemSounds.GetValue()}.");
        }

        private const string AmbientSoundsAdjustCommand = "/ssas";
        [Command(AmbientSoundsAdjustCommand)]
        [HelpMessage("Adjust the game's ambient sound volume by the specified quantity.")]
        public void AmbientSoundsAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.AmbientSoundsMuted.SetValue(true);
                chat.Print("Ambient sounds muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.AmbientSoundsMuted.SetValue(false);
                chat.Print("Ambient sounds unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, AmbientSoundsAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.AmbientSounds, volumeTarget, op);
            chat.Print($"Ambient sound volume set to {this.vc.AmbientSounds.GetValue()}.");
        }

        private const string PerformanceAdjustCommand = "/ssp";
        [Command(PerformanceAdjustCommand)]
        [HelpMessage("Adjust the game's performance volume by the specified quantity.")]
        public void PerformanceAdjust(string command, string args)
        {
            var chat = this.pluginInterface.Framework.Gui.Chat;

            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Mute)
            {
                this.vc.PerformanceMuted.SetValue(true);
                chat.Print("Performance volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.PerformanceMuted.SetValue(false);
                chat.Print("Performance volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(chat, string.Format(ErrorMessages.AdjustCommand, PerformanceAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.Performance, volumeTarget, op);
            chat.Print($"Performance volume set to {this.vc.Performance.GetValue()}.");
        }

        private static void ParseAdjustArgs(string args, out OperationKind op, out string volumeTargetStr)
        {
            var argsList = args.Split(' ').Select(a => a.ToLower()).ToList();

            if (argsList[0] == "mute")
            {
                op = OperationKind.Mute;
                volumeTargetStr = "";
                return;
            }

            if (argsList[0] == "unmute")
            {
                op = OperationKind.Unmute;
                volumeTargetStr = "";
                return;
            }

            volumeTargetStr = argsList[0];
            op = volumeTargetStr[0] switch
            {
                '+' => OperationKind.Add,
                '-' => OperationKind.Subtract,
                _ => OperationKind.Set,
            };

            if (op != OperationKind.Set)
                volumeTargetStr = volumeTargetStr.Substring(1);
        }

        private static void PrintChatError(ChatGui chat, string message)
        {
            chat.PrintChat(new XivChatEntry
            {
                MessageBytes = Encoding.UTF8.GetBytes(message),
                Type = XivChatType.ErrorMessage,
            });
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
