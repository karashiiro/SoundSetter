using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using SoundSetter.Attributes;
using SoundSetter.OptionInternals;
using System;
using System.Linq;
using System.Text;

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
            DoCommand(
                MasterVolumeAdjustCommand,
                "Master",
                ErrorMessages.AdjustCommand,
                this.vc.MasterVolumeMuted,
                this.vc.MasterVolume,
                args);
        }

        private const string BgmAdjustCommand = "/ssbgm";
        [Command(BgmAdjustCommand)]
        [HelpMessage("Adjust the game's BGM volume by the specified quantity.")]
        public void BgmAdjust(string command, string args)
        {
            DoCommand(
                BgmAdjustCommand,
                "BGM",
                ErrorMessages.AdjustCommand,
                this.vc.BgmMuted,
                this.vc.Bgm,
                args);
        }

        private const string SoundEffectsAdjustCommand = "/sssfx";
        [Command(SoundEffectsAdjustCommand)]
        [HelpMessage("Adjust the game's SFX volume by the specified quantity.")]
        public void SoundEffectsAdjust(string command, string args)
        {
            DoCommand(
                SoundEffectsAdjustCommand,
                "SFX",
                ErrorMessages.AdjustCommand,
                this.vc.SoundEffectsMuted,
                this.vc.SoundEffects,
                args);
        }

        private const string VoiceAdjustCommand = "/ssv";
        [Command(VoiceAdjustCommand)]
        [HelpMessage("Adjust the game's voice volume by the specified quantity.")]
        public void VoiceAdjust(string command, string args)
        {
            DoCommand(
                VoiceAdjustCommand,
                "Voice",
                ErrorMessages.AdjustCommand,
                this.vc.VoiceMuted,
                this.vc.Voice,
                args);
        }

        private const string SystemSoundsAdjustCommand = "/sssys";
        [Command(SystemSoundsAdjustCommand)]
        [HelpMessage("Adjust the game's system sound volume by the specified quantity.")]
        public void SystemSoundsAdjust(string command, string args)
        {
            DoCommand(
                SystemSoundsAdjustCommand,
                "System sound",
                ErrorMessages.AdjustCommand,
                this.vc.SystemSoundsMuted,
                this.vc.SystemSounds,
                args);
        }

        private const string AmbientSoundsAdjustCommand = "/ssas";
        [Command(AmbientSoundsAdjustCommand)]
        [HelpMessage("Adjust the game's ambient sound volume by the specified quantity.")]
        public void AmbientSoundsAdjust(string command, string args)
        {
            DoCommand(
                AmbientSoundsAdjustCommand,
                "Ambient sound",
                ErrorMessages.AdjustCommand,
                this.vc.AmbientSoundsMuted,
                this.vc.AmbientSounds,
                args);
        }

        private const string PerformanceAdjustCommand = "/ssp";
        [Command(PerformanceAdjustCommand)]
        [HelpMessage("Adjust the game's performance volume by the specified quantity.")]
        public void PerformanceAdjust(string command, string args)
        {
            DoCommand(
                PerformanceAdjustCommand,
                "Performance",
                ErrorMessages.AdjustCommand,
                this.vc.PerformanceMuted,
                this.vc.Performance,
                args);
        }

        private void DoCommand(string command, string optName, string errorMessage, BooleanOption boolOpt, ByteOption varOpt, string args)
        {
            ParseAdjustArgs(args, out var op, out var targetStr);

            if (op == OperationKind.Toggle)
            {
                var muted = boolOpt.GetValue();
                op = muted ? OperationKind.Unmute : OperationKind.Mute;
            }

            if (op == OperationKind.Mute)
            {
                boolOpt.SetValue(true);
                ChatGui.Print($"{optName} volume muted.");
                return;
            }

            if (op == OperationKind.Unmute)
            {
                boolOpt.SetValue(false);
                ChatGui.Print($"{optName} volume unmuted.");
                return;
            }

            if (!int.TryParse(targetStr, out var volumeTarget))
            {
                PrintChatError(ChatGui, string.Format(errorMessage, command));
                return;
            }

            VolumeControls.AdjustVolume(varOpt, volumeTarget, op);
            ChatGui.Print($"{optName} volume set to {varOpt.GetValue()}.");
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
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
