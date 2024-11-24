using Dalamud.Plugin.Services;
using SoundSetter.OptionInternals;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter
{
    public class VolumeControls(
        ISigScanner sigScanner,
        IGameInteropProvider gameInterop,
        IPluginLog log,
        Action<ExpandoObject>? onChange) : IDisposable
    {
        private static class Signatures
        {
            public const string SetOption =
                "89 54 24 10 53 55 57 41 55 41 56 41 57 48 83 EC ?? 32 C0 49 63 E9 45 8B E8";
        }

        private readonly OptionOffsets offsets = OptionOffsets.Load(log);

        private Hook<SetOptionDelegate>? setOptionHook;
        private SetOptionDelegate? setOptionActual;

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

        private bool initialized;

        public bool IsInitialized() => initialized;

        public unsafe void OnTick(IFramework f)
        {
            var configModule = ConfigModule.Instance();
            if (!initialized && configModule != null)
            {
                initialized = true;
                InitializeFramework(configModule);
            }
        }

        private unsafe void InitializeFramework(ConfigModule* configModule)
        {
            // Grab the configuration option setter used by the UI
            if (sigScanner.TryScanText(Signatures.SetOption, out var setOptionPtr))
            {
                this.setOptionActual = Marshal.GetDelegateForFunctionPointer<SetOptionDelegate>(setOptionPtr);
                LogUIChanges(setOptionPtr);
            }
            else
            {
                log.Error("Failed to hook configuration setter method!");
            }

            // InitializeOptions(this.setOptionActual);
            InitializeOptions((_, kind, value, _, _, _) =>
            {
                var configEnum = OptionKind.GetConfigEnum(kind);
                ref var optionValue1 = ref configModule->Values[(int)configEnum];
                ref var optionValue2 = ref OptionValue.FromOptionValue(ref optionValue1);
                optionValue2.Value1 = value;
                return nint.Zero;
            });

            LogCurrentSettings();
        }

        [Conditional("DEBUG")]
        private unsafe void LogUIChanges(nint setOptionPtr)
        {
            this.setOptionHook = gameInterop.HookFromAddress<SetOptionDelegate>(setOptionPtr,
                (address, kind, value, unk1, unk2, unk3) =>
                {
                    log.Debug($"{(nint)address:X8}: {kind}, {value}, {unk1}, {unk2}, {unk3}");
                    return this.setOptionHook!.Original(address, kind, value, unk1, unk2, unk3);
                });
            this.setOptionHook.Enable();
        }

        [Conditional("DEBUG")]
        private unsafe void LogCurrentSettings()
        {
            var configModule = ConfigModule.Instance();
            for (var i = 0; i < configModule->Values.Length; i++)
            {
                var optionValue = configModule->Values[i];
                ref var rawValue = ref OptionValue.FromOptionValue(ref optionValue);
                log.Info($"{(nint)configModule:X8}: {(OptionKind.ConfigEnum)i}, {rawValue.Value1}, {rawValue.Value2}");
            }
        }

        private void InitializeOptions(SetOptionDelegate setOption)
        {
            var makeByteOption = ByteOption.CreateFactory(log, onChange, "SoundPlay Settings", setOption);
            var makeBooleanOptionSoundPlay =
                BooleanOption.CreateFactory(log, onChange, "SoundPlay Settings", setOption);
            var makeBooleanOptionSoundSettings =
                BooleanOption.CreateFactory(log, onChange, "Sound Settings", setOption);

            PlaySoundsWhileWindowIsNotActive = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActive, this.offsets.PlaySoundsWhileWindowIsNotActive,
                "IsSoundAlways");
            PlaySoundsWhileWindowIsNotActiveBGM = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActiveBGM, this.offsets.PlaySoundsWhileWindowIsNotActiveBGM,
                "IsSoundBgmAlways");
            PlaySoundsWhileWindowIsNotActiveSoundEffects = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActiveSoundEffects,
                this.offsets.PlaySoundsWhileWindowIsNotActiveSoundEffects, "IsSoundSeAlways");
            PlaySoundsWhileWindowIsNotActiveVoice = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActiveVoice, this.offsets.PlaySoundsWhileWindowIsNotActiveVoice,
                "IsSoundVoiceAlways");
            PlaySoundsWhileWindowIsNotActiveSystemSounds = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActiveSystemSounds,
                this.offsets.PlaySoundsWhileWindowIsNotActiveSystemSounds, "IsSoundSystemAlways");
            PlaySoundsWhileWindowIsNotActiveAmbientSounds = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActiveAmbientSounds,
                this.offsets.PlaySoundsWhileWindowIsNotActiveAmbientSounds, "IsSoundEnvAlways");
            PlaySoundsWhileWindowIsNotActivePerformance = makeBooleanOptionSoundSettings(
                OptionKind.UIEnum.PlaySoundsWhileWindowIsNotActivePerformance,
                this.offsets.PlaySoundsWhileWindowIsNotActivePerformance, "IsSoundPerformAlways");

            PlayMusicWhenMounted =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.PlayMusicWhenMounted, this.offsets.PlayMusicWhenMounted, null);
            EnableNormalBattleMusic = makeBooleanOptionSoundPlay(OptionKind.UIEnum.EnableNormalBattleMusic,
                this.offsets.EnableNormalBattleMusic, null);
            EnableCityStateBGM =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.EnableCityStateBGM, this.offsets.EnableCityStateBGM, null);
            PlaySystemSounds =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.PlaySystemSounds, this.offsets.PlaySystemSounds, null);

            MasterVolume = makeByteOption(OptionKind.UIEnum.Master, this.offsets.MasterVolume, "SoundMaster");
            Bgm = makeByteOption(OptionKind.UIEnum.Bgm, this.offsets.Bgm, "SoundBgm");
            SoundEffects = makeByteOption(OptionKind.UIEnum.SoundEffects, this.offsets.SoundEffects, "SoundSe");
            Voice = makeByteOption(OptionKind.UIEnum.Voice, this.offsets.Voice, "SoundVoice");
            SystemSounds = makeByteOption(OptionKind.UIEnum.SystemSounds, this.offsets.SystemSounds, "SoundSystem");
            AmbientSounds = makeByteOption(OptionKind.UIEnum.AmbientSounds, this.offsets.AmbientSounds, "SoundEnv");
            Performance = makeByteOption(OptionKind.UIEnum.Performance, this.offsets.Performance, "SoundPerform");

            Self = makeByteOption(OptionKind.UIEnum.Self, this.offsets.Self, "SoundPlayer");
            Party = makeByteOption(OptionKind.UIEnum.Party, this.offsets.Party, "SoundParty");
            OtherPCs = makeByteOption(OptionKind.UIEnum.OtherPCs, this.offsets.OtherPCs, "SoundOther");

            MasterVolumeMuted =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.MasterMuted, this.offsets.MasterVolumeMuted, "IsSndMaster");
            MasterVolumeMuted.Hack = true;
            BgmMuted = makeBooleanOptionSoundPlay(OptionKind.UIEnum.BgmMuted, this.offsets.BgmMuted, "IsSndBgm");
            BgmMuted.Hack = true;
            SoundEffectsMuted =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.SoundEffectsMuted, this.offsets.SoundEffectsMuted, "IsSndSe");
            SoundEffectsMuted.Hack = true;
            VoiceMuted = makeBooleanOptionSoundPlay(OptionKind.UIEnum.VoiceMuted, this.offsets.VoiceMuted, "IsSndVoice");
            VoiceMuted.Hack = true;
            SystemSoundsMuted = makeBooleanOptionSoundPlay(OptionKind.UIEnum.SystemSoundsMuted, this.offsets.SystemSoundsMuted,
                "IsSndSystem");
            SystemSoundsMuted.Hack = true;
            AmbientSoundsMuted =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.AmbientSoundsMuted, this.offsets.AmbientSoundsMuted, "IsSndEnv");
            AmbientSoundsMuted.Hack = true;
            PerformanceMuted =
                makeBooleanOptionSoundPlay(OptionKind.UIEnum.PerformanceMuted, this.offsets.PerformanceMuted, "IsSndPerform");
            PerformanceMuted.Hack = true;

            EqualizerMode = new EqualizerModeOption(log)
            {
                Offset = this.offsets.EqualizerMode,
                Kind = OptionKind.UIEnum.EqualizerMode,

                CfgSection = "SoundPlay Settings",
                CfgSetting = "SoundEqualizerType",

                OnChange = onChange,

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