using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using SoundSetter.OptionInternals;
using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter
{
    public class VolumeControls : IDisposable
    {
        private static class Signatures
        {
            public const string SetOption =
                "89 54 24 10 53 55 57 41 55 41 56 41 57 48 83 EC ?? 32 C0 49 63 E9 45 8B E8";
        }

        private readonly Action<ExpandoObject>? onChange;
        private readonly OptionOffsets offsets;
        private readonly IGameInteropProvider gameInterop;
        private readonly IPluginLog log;

        private Hook<SetOptionDelegate>? setOptionHook;

        public static nint BaseAddress
        {
            get
            {
                unsafe
                {
                    return (nint)ConfigModule.Instance();
                }
            }
        }

        public BooleanOption? PlaySoundsWhileWindowIsNotActive { get; private set; }
        public BooleanOption? PlaySoundsWhileWindowIsNotActiveBGM { get; private set; }
        public BooleanOption? PlaySoundsWhileWindowIsNotActiveSoundEffects { get; private set; }
        public BooleanOption? PlaySoundsWhileWindowIsNotActiveVoice { get; private set; }
        public BooleanOption? PlaySoundsWhileWindowIsNotActiveSystemSounds { get; private set; }
        public BooleanOption? PlaySoundsWhileWindowIsNotActiveAmbientSounds { get; private set; }
        public BooleanOption? PlaySoundsWhileWindowIsNotActivePerformance { get; private set; }

        public BooleanOption? PlayMusicWhenMounted { get; private set; }
        public BooleanOption? EnableNormalBattleMusic { get; private set; }
        public BooleanOption? EnableCityStateBGM { get; private set; }
        public BooleanOption? PlaySystemSounds { get; private set; }

        public ByteOption? MasterVolume { get; private set; }
        public ByteOption? Bgm { get; private set; }
        public ByteOption? SoundEffects { get; private set; }
        public ByteOption? Voice { get; private set; }
        public ByteOption? SystemSounds { get; private set; }
        public ByteOption? AmbientSounds { get; private set; }
        public ByteOption? Performance { get; private set; }

        public ByteOption? Self { get; private set; }
        public ByteOption? Party { get; private set; }
        public ByteOption? OtherPCs { get; private set; }

        public BooleanOption? MasterVolumeMuted { get; private set; }
        public BooleanOption? BgmMuted { get; private set; }
        public BooleanOption? SoundEffectsMuted { get; private set; }
        public BooleanOption? VoiceMuted { get; private set; }
        public BooleanOption? SystemSoundsMuted { get; private set; }
        public BooleanOption? AmbientSoundsMuted { get; private set; }
        public BooleanOption? PerformanceMuted { get; private set; }

        public EqualizerModeOption? EqualizerMode { get; private set; }

        public VolumeControls(
            ISigScanner scanner,
            IGameInteropProvider gameInterop,
            IPluginLog log,
            Action<ExpandoObject>? onChange)
        {
            this.gameInterop = gameInterop;
            this.log = log;
            this.onChange = onChange;
            this.offsets = OptionOffsets.Load(log);

            if (scanner.TryScanText(Signatures.SetOption, out var setOptionPtr))
            {
                var setOption = Marshal.GetDelegateForFunctionPointer<SetOptionDelegate>(setOptionPtr);
                InitializeOptionsHook(setOptionPtr);
                InitializeOptions(setOption);
            }
            else
            {
                log.Error("Failed to hook configuration setter method!");
            }
        }

        private unsafe void InitializeOptionsHook(nint setOptionPtr)
        {
            // TODO: Keep plugin UI in sync with game when config is updated - but not saved - in the game UI
            // This requires keeping a draft of what the pending settings are, which possibly requires a
            // sizable refactor.
            this.setOptionHook = this.gameInterop.HookFromAddress<SetOptionDelegate>(setOptionPtr,
                (configModule, kind, value, unk1, unk2, unk3) =>
                {
#if DEBUG
                    log.Debug($"{(nint)configModule:X8}: {kind}, {value}, {unk1}, {unk2}, {unk3}");
#endif
                    return this.setOptionHook!.Original(configModule, kind, value, unk1, unk2, unk3);
                });
            this.setOptionHook.Enable();
        }

        private unsafe void InitializeOptions(SetOptionDelegate setOption)
        {
            var makeByteOption = ByteOption.CreateFactory(this.log, BaseAddress, this.onChange, "SoundPlay Settings", setOption);
            var makeBooleanOptionSoundPlay =
                BooleanOption.CreateFactory(this.log, BaseAddress, this.onChange, "SoundPlay Settings", setOption);
            var makeBooleanOptionSoundSettings =
                BooleanOption.CreateFactory(this.log, BaseAddress, this.onChange, "Sound Settings", setOption);

            PlaySoundsWhileWindowIsNotActive = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActive, this.offsets.PlaySoundsWhileWindowIsNotActive,
                "IsSoundAlways");
            PlaySoundsWhileWindowIsNotActiveBGM = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActiveBGM, this.offsets.PlaySoundsWhileWindowIsNotActiveBGM,
                "IsSoundBgmAlways");
            PlaySoundsWhileWindowIsNotActiveSoundEffects = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActiveSoundEffects,
                this.offsets.PlaySoundsWhileWindowIsNotActiveSoundEffects, "IsSoundSeAlways");
            PlaySoundsWhileWindowIsNotActiveVoice = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActiveVoice, this.offsets.PlaySoundsWhileWindowIsNotActiveVoice,
                "IsSoundVoiceAlways");
            PlaySoundsWhileWindowIsNotActiveSystemSounds = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActiveSystemSounds,
                this.offsets.PlaySoundsWhileWindowIsNotActiveSystemSounds, "IsSoundSystemAlways");
            PlaySoundsWhileWindowIsNotActiveAmbientSounds = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActiveAmbientSounds,
                this.offsets.PlaySoundsWhileWindowIsNotActiveAmbientSounds, "IsSoundEnvAlways");
            PlaySoundsWhileWindowIsNotActivePerformance = makeBooleanOptionSoundSettings(
                OptionKind.PlaySoundsWhileWindowIsNotActivePerformance,
                this.offsets.PlaySoundsWhileWindowIsNotActivePerformance, "IsSoundPerformAlways");

            PlayMusicWhenMounted =
                makeBooleanOptionSoundPlay(OptionKind.PlayMusicWhenMounted, this.offsets.PlayMusicWhenMounted, null);
            EnableNormalBattleMusic = makeBooleanOptionSoundPlay(OptionKind.EnableNormalBattleMusic,
                this.offsets.EnableNormalBattleMusic, null);
            EnableCityStateBGM =
                makeBooleanOptionSoundPlay(OptionKind.EnableCityStateBGM, this.offsets.EnableCityStateBGM, null);
            PlaySystemSounds =
                makeBooleanOptionSoundPlay(OptionKind.PlaySystemSounds, this.offsets.PlaySystemSounds, null);

            MasterVolume = makeByteOption(OptionKind.Master, this.offsets.MasterVolume, "SoundMaster");
            Bgm = makeByteOption(OptionKind.Bgm, this.offsets.Bgm, "SoundBgm");
            SoundEffects = makeByteOption(OptionKind.SoundEffects, this.offsets.SoundEffects, "SoundSe");
            Voice = makeByteOption(OptionKind.Voice, this.offsets.Voice, "SoundVoice");
            SystemSounds = makeByteOption(OptionKind.SystemSounds, this.offsets.SystemSounds, "SoundSystem");
            AmbientSounds = makeByteOption(OptionKind.AmbientSounds, this.offsets.AmbientSounds, "SoundEnv");
            Performance = makeByteOption(OptionKind.Performance, this.offsets.Performance, "SoundPerform");

            Self = makeByteOption(OptionKind.Self, this.offsets.Self, "SoundPlayer");
            Party = makeByteOption(OptionKind.Party, this.offsets.Party, "SoundParty");
            OtherPCs = makeByteOption(OptionKind.OtherPCs, this.offsets.OtherPCs, "SoundOther");

            MasterVolumeMuted =
                makeBooleanOptionSoundPlay(OptionKind.MasterMuted, this.offsets.MasterVolumeMuted, "IsSndMaster");
            MasterVolumeMuted.Hack = true;
            BgmMuted = makeBooleanOptionSoundPlay(OptionKind.BgmMuted, this.offsets.BgmMuted, "IsSndBgm");
            BgmMuted.Hack = true;
            SoundEffectsMuted =
                makeBooleanOptionSoundPlay(OptionKind.SoundEffectsMuted, this.offsets.SoundEffectsMuted, "IsSndSe");
            SoundEffectsMuted.Hack = true;
            VoiceMuted = makeBooleanOptionSoundPlay(OptionKind.VoiceMuted, this.offsets.VoiceMuted, "IsSndVoice");
            VoiceMuted.Hack = true;
            SystemSoundsMuted = makeBooleanOptionSoundPlay(OptionKind.SystemSoundsMuted, this.offsets.SystemSoundsMuted,
                "IsSndSystem");
            SystemSoundsMuted.Hack = true;
            AmbientSoundsMuted =
                makeBooleanOptionSoundPlay(OptionKind.AmbientSoundsMuted, this.offsets.AmbientSoundsMuted, "IsSndEnv");
            AmbientSoundsMuted.Hack = true;
            PerformanceMuted =
                makeBooleanOptionSoundPlay(OptionKind.PerformanceMuted, this.offsets.PerformanceMuted, "IsSndPerform");
            PerformanceMuted.Hack = true;

            EqualizerMode = new EqualizerModeOption(this.log)
            {
                ConfigModule = (ConfigModule*)BaseAddress,
                Offset = this.offsets.EqualizerMode,
                Kind = OptionKind.EqualizerMode,

                CfgSection = "SoundPlay Settings",
                CfgSetting = "SoundEqualizerType",

                OnChange = this.onChange,

                SetFunction = setOption,
            };
        }

        public static void ToggleVolume(BooleanOption? option, OperationKind interaction)
        {
            if (option == null)
            {
                throw new InvalidOperationException(
                    "Plugin is uninitialized; sound settings must be modified once for options to be changed.");
            }

            var muted = option.GetValue();
            switch (interaction)
            {
                case OperationKind.Unmute:
                    option.SetValue(false);
                    break;
                case OperationKind.Mute:
                    option.SetValue(true);
                    break;
                case OperationKind.Toggle:
                    option.SetValue(!muted);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interaction));
            }
        }

        public static void AdjustVolume(ByteOption? option, int volumeTarget, OperationKind interaction)
        {
            if (option == null)
            {
                throw new InvalidOperationException(
                    "Plugin is uninitialized; sound settings must be modified once for options to be changed.");
            }

            var curVol = option.GetValue();
            switch (interaction)
            {
                case OperationKind.Add:
                    option.SetValue((byte)Math.Min(curVol + volumeTarget, 100));
                    break;
                case OperationKind.Subtract:
                    option.SetValue((byte)Math.Max(curVol - volumeTarget, 0));
                    break;
                case OperationKind.Set:
                    option.SetValue((byte)Math.Min(volumeTarget, 100));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interaction));
            }
        }

        public void Dispose()
        {
            this.setOptionHook?.Disable();
            this.setOptionHook?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}