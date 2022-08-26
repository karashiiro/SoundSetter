using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using SoundSetter.OptionInternals;
using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace SoundSetter
{
    public class VolumeControls : IDisposable
    {
        private readonly Hook<SetOptionDelegate> setOptionHook;
        private readonly Action<ExpandoObject> onChange;

        public IntPtr BaseAddress { get; private set; }

        public BooleanOption PlayMusicWhenMounted { get; private set; }
        public BooleanOption EnableNormalBattleMusic { get; private set; }
        public BooleanOption EnableCityStateBGM { get; private set; }
        public BooleanOption PlaySystemSounds { get; private set; }

        public ByteOption MasterVolume { get; private set; }
        public ByteOption Bgm { get; private set; }
        public ByteOption SoundEffects { get; private set; }
        public ByteOption Voice { get; private set; }
        public ByteOption SystemSounds { get; private set; }
        public ByteOption AmbientSounds { get; private set; }
        public ByteOption Performance { get; private set; }

        public ByteOption Self { get; private set; }
        public ByteOption Party { get; private set; }
        public ByteOption OtherPCs { get; private set; }

        public BooleanOption MasterVolumeMuted { get; private set; }
        public BooleanOption BgmMuted { get; private set; }
        public BooleanOption SoundEffectsMuted { get; private set; }
        public BooleanOption VoiceMuted { get; private set; }
        public BooleanOption SystemSoundsMuted { get; private set; }
        public BooleanOption AmbientSoundsMuted { get; private set; }
        public BooleanOption PerformanceMuted { get; private set; }

        public EqualizerModeOption EqualizerMode { get; private set; }

        public VolumeControls(SigScanner scanner, Action<ExpandoObject> onChange)
        {
            this.onChange = onChange;

            try
            {
                // I thought I'd need the user to change the settings manually once to get the the base address,
                // but the function is automatically called once when the player is initialized, so I'll settle for that.
                // Note to self: Cheat Engine's "Select current function" tool is unreliable, don't waste time with it.
                // This signature is probably stable, but the option struct offsets need to be updated after some patches.
                var setConfigurationPtr = scanner.ScanText("89 54 24 10 53 55 57 41 54 41 55 41 56 48 83 EC 48 8B C2 45 8B E0 44 8B D2 45 32 F6 44 8B C2 45 32 ED");
                var setOption = Marshal.GetDelegateForFunctionPointer<SetOptionDelegate>(setConfigurationPtr);
                this.setOptionHook = Hook<SetOptionDelegate>.FromAddress(setConfigurationPtr, (baseAddress, kind, value, unk1, unk2, unk3) =>
                {
                    if (MasterVolume == null)
                    {
                        BaseAddress = baseAddress;
                        InitializeOptions(setOption);
                    }

                    PluginLog.LogDebug($"{baseAddress}, {kind}, {value}, {unk1}, {unk2}, {unk3}");
                    return this.setOptionHook!.Original(baseAddress, kind, value, unk1, unk2, unk3);
                });
                this.setOptionHook.Enable();
            }
            catch (Exception e)
            {
                PluginLog.LogError($"Failed to hook configuration set method! Full error:\n{e}");
            }
        }

        private void InitializeOptions(SetOptionDelegate setOption)
        {
            var makeByteOption = ByteOption.CreateFactory(BaseAddress, this.onChange, "SoundPlay Settings", setOption);
            var makeBooleanOption = BooleanOption.CreateFactory(BaseAddress, this.onChange, "SoundPlay Settings", setOption);

            PlayMusicWhenMounted = makeBooleanOption(OptionKind.PlayMusicWhenMounted, OptionOffsets.PlayMusicWhenMounted, null);
            PlayMusicWhenMounted.Hack = false;
            EnableNormalBattleMusic = makeBooleanOption(OptionKind.EnableNormalBattleMusic, OptionOffsets.EnableNormalBattleMusic, null);
            EnableNormalBattleMusic.Hack = false;
            EnableCityStateBGM = makeBooleanOption(OptionKind.EnableCityStateBGM, OptionOffsets.EnableCityStateBGM, null);
            EnableCityStateBGM.Hack = false;
            PlaySystemSounds = makeBooleanOption(OptionKind.PlaySystemSounds, OptionOffsets.PlaySystemSounds, null);
            PlaySystemSounds.Hack = false;

            MasterVolume = makeByteOption(OptionKind.Master, OptionOffsets.MasterVolume, "SoundMaster");
            Bgm = makeByteOption(OptionKind.Bgm, OptionOffsets.Bgm, "SoundBgm");
            SoundEffects = makeByteOption(OptionKind.SoundEffects, OptionOffsets.SoundEffects, "SoundSe");
            Voice = makeByteOption(OptionKind.Voice, OptionOffsets.Voice, "SoundVoice");
            SystemSounds = makeByteOption(OptionKind.SystemSounds, OptionOffsets.SystemSounds, "SoundSystem");
            AmbientSounds = makeByteOption(OptionKind.AmbientSounds, OptionOffsets.AmbientSounds, "SoundEnv");
            Performance = makeByteOption(OptionKind.Performance, OptionOffsets.Performance, "SoundPerform");

            Self = makeByteOption(OptionKind.Self, OptionOffsets.Self, "SoundPlayer");
            Party = makeByteOption(OptionKind.Party, OptionOffsets.Party, "SoundParty");
            OtherPCs = makeByteOption(OptionKind.OtherPCs, OptionOffsets.OtherPCs, "SoundOther");

            MasterVolumeMuted = makeBooleanOption(OptionKind.MasterMuted, OptionOffsets.MasterVolumeMuted, "IsSndMaster");
            BgmMuted = makeBooleanOption(OptionKind.BgmMuted, OptionOffsets.BgmMuted, "IsSndBgm");
            SoundEffectsMuted = makeBooleanOption(OptionKind.SoundEffectsMuted, OptionOffsets.SoundEffectsMuted, "IsSndSe");
            VoiceMuted = makeBooleanOption(OptionKind.VoiceMuted, OptionOffsets.VoiceMuted, "IsSndVoice");
            SystemSoundsMuted = makeBooleanOption(OptionKind.SystemSoundsMuted, OptionOffsets.SystemSoundsMuted, "IsSndSystem");
            AmbientSoundsMuted = makeBooleanOption(OptionKind.AmbientSoundsMuted, OptionOffsets.AmbientSoundsMuted, "IsSndEnv");
            PerformanceMuted = makeBooleanOption(OptionKind.PerformanceMuted, OptionOffsets.PerformanceMuted, "IsSndPerform");

            EqualizerMode = new EqualizerModeOption
            {
                BaseAddress = BaseAddress,
                Offset = OptionOffsets.EqualizerMode,
                Kind = OptionKind.EqualizerMode,

                CfgSection = "SoundPlay Settings",
                CfgSetting = "SoundEqualizerType",

                OnChange = this.onChange,

                SetFunction = setOption,
            };
        }

        public static void ToggleVolume(BooleanOption option, OperationKind interaction)
        {
            if (option == null)
            {
                throw new InvalidOperationException("Plugin is uninitialized; sound settings must be modified once for options to be changed.");
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

        public static void AdjustVolume(ByteOption option, int volumeTarget, OperationKind interaction)
        {
            if (option == null)
            {
                throw new InvalidOperationException("Plugin is uninitialized; sound settings must be modified once for options to be changed.");
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
