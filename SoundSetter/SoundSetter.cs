using Dalamud.Plugin;
using SoundSetter.Attributes;
using System;
using System.Linq;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;

// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SoundSetter
{
    public class SoundSetter : IDalamudPlugin
    {
        [PluginService]
        [RequiredVersion("1.0")]
        private DalamudPluginInterface PluginInterface { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private SigScanner SigScanner { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private SeStringManager SeStringManager { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private CommandManager Commands { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private Condition Condition { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private KeyState KeyState { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private ClientState ClientState { get; init; }

        [PluginService]
        [RequiredVersion("1.0")]
        private ChatGui ChatGui { get; init; }

        private readonly PluginCommandManager<SoundSetter> commandManager;

        private readonly Configuration config;
        private readonly SoundSetterUI ui;
        private readonly VolumeControls vc;

        public string Name => "SoundSetter";

        public SoundSetter()
        {
            this.config = (Configuration)PluginInterface.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(PluginInterface);

            this.vc = new VolumeControls(SigScanner, null); // TODO: restore IPC

            PluginInterface.UiBuilder.DisableAutomaticUiHide = true;

            this.ui = new SoundSetterUI(this.vc, PluginInterface, this.config);
            PluginInterface.UiBuilder.Draw += this.ui.Draw;
            PluginInterface.UiBuilder.Draw += OnTick;

            PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

            this.commandManager = new PluginCommandManager<SoundSetter>(this, Commands);
        }

        private bool keysDown;
        private void OnTick()
        {
            // We don't want to open the UI before the player loads, that leaves the options uninitialized.
            if (ClientState.LocalContentId == 0) return;

            var cutsceneActive = Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
                                     Condition[ConditionFlag.WatchingCutscene] ||
                                     Condition[ConditionFlag.WatchingCutscene78];

            if (this.config.OnlyShowInCutscenes && !cutsceneActive) return;

            if (KeyState[(byte)this.config.ModifierKey] &&
                KeyState[(byte)this.config.MajorKey])
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

        private void ToggleConfig()
            => this.ui.IsVisible = !this.ui.IsVisible;

        private void OpenConfig()
            => this.ui.IsVisible = true;

        private const string MasterVolumeAdjustCommand = "/ssmv";
        [Command(MasterVolumeAdjustCommand)]
        [HelpMessage("Adjust the game's master volume by the specified quantity.")]
        public void MasterVolumeAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.MasterVolumeMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.MasterVolumeMuted.SetValue(true);
                ChatGui.Print("Master volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.MasterVolumeMuted.SetValue(false);
                ChatGui.Print("Master volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, MasterVolumeAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.MasterVolume, volumeTarget, op);
            ChatGui.Print($"Master volume set to {this.vc.MasterVolume.GetValue()}.");
        }

        private const string BgmAdjustCommand = "/ssbgm";
        [Command(BgmAdjustCommand)]
        [HelpMessage("Adjust the game's BGM volume by the specified quantity.")]
        public void BgmAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.BgmMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.BgmMuted.SetValue(true);
                ChatGui.Print("BGM volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.BgmMuted.SetValue(false);
                ChatGui.Print("BGM volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, BgmAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.Bgm, volumeTarget, op);
            ChatGui.Print($"BGM volume set to {this.vc.Bgm.GetValue()}.");
        }

        private const string SoundEffectsAdjustCommand = "/sssfx";
        [Command(SoundEffectsAdjustCommand)]
        [HelpMessage("Adjust the game's SFX volume by the specified quantity.")]
        public void SoundEffectsAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.SoundEffectsMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.SoundEffectsMuted.SetValue(true);
                ChatGui.Print("SFX volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.SoundEffectsMuted.SetValue(false);
                ChatGui.Print("SFX volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, SoundEffectsAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.SoundEffects, volumeTarget, op);
            ChatGui.Print($"SFX volume set to {this.vc.SoundEffects.GetValue()}.");
        }

        private const string VoiceAdjustCommand = "/ssv";
        [Command(VoiceAdjustCommand)]
        [HelpMessage("Adjust the game's voice volume by the specified quantity.")]
        public void VoiceAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.VoiceMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.VoiceMuted.SetValue(true);
                ChatGui.Print("Voice volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.VoiceMuted.SetValue(false);
                ChatGui.Print("Voice volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, VoiceAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.Voice, volumeTarget, op);
            ChatGui.Print($"Voice volume set to {this.vc.Voice.GetValue()}.");
        }

        private const string SystemSoundsAdjustCommand = "/sssys";
        [Command(SystemSoundsAdjustCommand)]
        [HelpMessage("Adjust the game's system sound volume by the specified quantity.")]
        public void SystemSoundsAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.SystemSoundsMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.SystemSoundsMuted.SetValue(true);
                ChatGui.Print("System sounds muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.SystemSoundsMuted.SetValue(false);
                ChatGui.Print("System sounds unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, SystemSoundsAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.SystemSounds, volumeTarget, op);
            ChatGui.Print($"System sound volume set to {this.vc.SystemSounds.GetValue()}.");
        }

        private const string AmbientSoundsAdjustCommand = "/ssas";
        [Command(AmbientSoundsAdjustCommand)]
        [HelpMessage("Adjust the game's ambient sound volume by the specified quantity.")]
        public void AmbientSoundsAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.AmbientSoundsMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.AmbientSoundsMuted.SetValue(true);
                ChatGui.Print("Ambient sounds muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.AmbientSoundsMuted.SetValue(false);
                ChatGui.Print("Ambient sounds unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, AmbientSoundsAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.AmbientSounds, volumeTarget, op);
            ChatGui.Print($"Ambient sound volume set to {this.vc.AmbientSounds.GetValue()}.");
        }

        private const string PerformanceAdjustCommand = "/ssp";
        [Command(PerformanceAdjustCommand)]
        [HelpMessage("Adjust the game's performance volume by the specified quantity.")]
        public void PerformanceAdjust(string command, string args)
        {
            ParseAdjustArgs(args, out var op, out var volumeTargetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = this.vc.PerformanceMuted.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                this.vc.PerformanceMuted.SetValue(true);
                ChatGui.Print("Performance volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                this.vc.PerformanceMuted.SetValue(false);
                ChatGui.Print("Performance volume unmuted.");
                return;
            }

            if (!int.TryParse(volumeTargetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(ErrorMessages.AdjustCommand, PerformanceAdjustCommand));
                return;
            }

            VolumeControls.AdjustVolume(this.vc.Performance, volumeTarget, op);
            ChatGui.Print($"Performance volume set to {this.vc.Performance.GetValue()}.");
        }

        private static void ParseAdjustArgs(string args, out OperationKind op, out string volumeTargetStr)
        {
            volumeTargetStr = "";

            if (string.IsNullOrEmpty(args))
            {
                op = OperationKind.Toggle;
                return;
            }

            var argsList = args.Split(' ').Select(a => a.ToLower()).ToList();

            if (argsList[0] == "mute")
            {
                op = OperationKind.Mute;
                return;
            }

            if (argsList[0] == "unmute")
            {
                op = OperationKind.Unmute;
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
                volumeTargetStr = volumeTargetStr[1..];
        }

        private void PrintChatError(ChatGui chat, string message)
        {
            chat.PrintChat(new XivChatEntry
            {
                Message = SeStringManager.Parse(Encoding.UTF8.GetBytes(message)),
                Type = XivChatType.ErrorMessage,
            });
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.commandManager.Dispose();

            PluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;

            PluginInterface.UiBuilder.Draw -= OnTick;
            PluginInterface.UiBuilder.Draw -= this.ui.Draw;

            this.vc.Dispose();

            PluginInterface.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
