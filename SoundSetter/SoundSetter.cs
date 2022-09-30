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
using Dalamud.Logging;

// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SoundSetter
{
    public class SoundSetter : IDalamudPlugin
    {
        private readonly DalamudPluginInterface pluginInterface;
        private readonly ChatGui chatGui;
        private readonly Condition condition;
        private readonly KeyState keyState;
        private readonly ClientState clientState;

        private readonly PluginCommandManager<SoundSetter> commandManager;

        private readonly Configuration config;
        private readonly SoundSetterUI ui;
        private readonly VolumeControls vc;

        public string Name => "SoundSetter";

        public SoundSetter(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ChatGui chatGui,
            [RequiredVersion("1.0")] SigScanner sigScanner,
            [RequiredVersion("1.0")] CommandManager commands,
            [RequiredVersion("1.0")] Condition condition,
            [RequiredVersion("1.0")] ClientState clientState,
            [RequiredVersion("1.0")] KeyState keyState)
        {
            this.pluginInterface = pluginInterface;
            this.chatGui = chatGui;
            this.condition = condition;
            this.clientState = clientState;
            this.keyState = keyState;

            this.config = (Configuration)this.pluginInterface.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(this.pluginInterface);

            this.vc = new VolumeControls(sigScanner, null); // TODO: restore IPC

            this.pluginInterface.UiBuilder.DisableAutomaticUiHide = true;

            this.ui = new SoundSetterUI(this.vc, this.pluginInterface, this.config);
            this.pluginInterface.UiBuilder.Draw += this.ui.Draw;
            this.pluginInterface.UiBuilder.Draw += OnTick;

            this.pluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

            this.commandManager = new PluginCommandManager<SoundSetter>(this, commands);
        }

        private bool keysDown;

        private void OnTick()
        {
            // We don't want to open the UI before the player loads, that leaves the options uninitialized.
            if (this.clientState.LocalContentId == 0) return;

            var cutsceneActive = this.condition[ConditionFlag.OccupiedInCutSceneEvent] ||
                                 this.condition[ConditionFlag.WatchingCutscene] ||
                                 this.condition[ConditionFlag.WatchingCutscene78];

            if (this.config.OnlyShowInCutscenes && !cutsceneActive) return;

            if (this.keyState[(byte)this.config.ModifierKey] &&
                this.keyState[(byte)this.config.MajorKey])
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

        private void DoCommand(string command, string optName, string errorMessage, BooleanOption boolOpt,
            ByteOption varOpt, string args)
        {
            ParseAdjustArgs(args, out var op, out var targetStr);

            try
            {
                if (op == OperationKind.Toggle)
                {
                    var muted = boolOpt?.GetValue();
                    op = muted == true ? OperationKind.Unmute : OperationKind.Mute;
                }

                if (op == OperationKind.Mute)
                {
                    VolumeControls.ToggleVolume(boolOpt, op);
                    this.chatGui.Print($"{optName} volume muted.");
                    return;
                }

                if (op == OperationKind.Unmute)
                {
                    VolumeControls.ToggleVolume(boolOpt, op);
                    this.chatGui.Print($"{optName} volume unmuted.");
                    return;
                }

                if (!int.TryParse(targetStr, out var volumeTarget))
                {
                    PrintChatError(this.chatGui, string.Format(errorMessage, command));
                    return;
                }

                VolumeControls.AdjustVolume(varOpt, volumeTarget, op);
                this.chatGui.Print($"{optName} volume set to {varOpt.GetValue()}.");
            }
            catch (InvalidOperationException e)
            {
                PluginLog.LogError(e, "Command failed.");
                this.chatGui.Print("SoundSetter is uninitialized.");
                this.chatGui.Print("Please manually change a volume setting once in order to initialize the plugin.");
            }
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

            var arg0 = argsList.FirstOrDefault();
            if (string.IsNullOrEmpty(arg0))
            {
                op = OperationKind.Set;
                return;
            }

            if (arg0 == "toggle")
            {
                op = OperationKind.Toggle;
                return;
            }

            if (arg0 == "mute")
            {
                op = OperationKind.Mute;
                return;
            }

            if (arg0 == "unmute")
            {
                op = OperationKind.Unmute;
                return;
            }

            volumeTargetStr = arg0;
            op = volumeTargetStr[0] switch
            {
                '+' => OperationKind.Add,
                '-' => OperationKind.Subtract,
                _ => OperationKind.Set,
            };

            if (op != OperationKind.Set)
                volumeTargetStr = volumeTargetStr[1..];
        }

        private static void PrintChatError(ChatGui chat, string message)
        {
            chat.PrintChat(new XivChatEntry
            {
                Message = SeString.Parse(Encoding.UTF8.GetBytes(message)),
                Type = XivChatType.ErrorMessage,
            });
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.commandManager.Dispose();

            this.pluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;

            this.pluginInterface.UiBuilder.Draw -= OnTick;
            this.pluginInterface.UiBuilder.Draw -= this.ui.Draw;

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