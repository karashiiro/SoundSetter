using Dalamud.Game;
using System;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin;
using SoundSetter.OptionInternals;
using static SoundSetter.OptionInternals.SetOption;

namespace SoundSetter
{
    public class VolumeControls : IDisposable
    {
        private readonly Hook<SetOptionDelegate> setOptionHook;

        public IntPtr BaseAddress { get; private set; }

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

        public VolumeControls(SigScanner scanner)
        {
            try
            {
                // I thought I'd need the user to change the settings manually once to get the the base address,
                // but the function is automatically called once when the player is initialized, so I'll settle for that.
                var setConfigurationPtr = scanner.ScanText("89 54 24 10 53 55 57 41 54 41 55 41 56 48 83 EC 48 8B C2 45 8B E0 44 8B D2 45 32 F6 44 8B C2 45 32 ED");
                var setOption = Marshal.GetDelegateForFunctionPointer<SetOptionDelegate>(setConfigurationPtr);
                this.setOptionHook = new Hook<SetOptionDelegate>(setConfigurationPtr, new SetOptionDelegate((baseAddress, kind, value, unknown) =>
                {
                    BaseAddress = baseAddress;

                    if (MasterVolume == null) InitializeOptions(setOption);
#if DEBUG
                    PluginLog.Log($"{baseAddress}, {kind}, {value}, {unknown}");
#endif
                    return this.setOptionHook.Original(baseAddress, kind, value, unknown);
                }));
                this.setOptionHook.Enable();
            }
            catch (Exception e)
            {
                PluginLog.LogError($"Failed to hook configuration set method! Full error:\n{e}");
            }
        }

        private void InitializeOptions(SetOptionDelegate setOption)
        {
            var byteOptionFactory = ByteOption.CreateFactory(BaseAddress, setOption);
            var booleanOptionFactory = BooleanOption.CreateFactory(BaseAddress, setOption);

            MasterVolume = byteOptionFactory(OptionKind.Master, OptionOffsets.MasterVolume);
            Bgm = byteOptionFactory(OptionKind.Bgm, OptionOffsets.Bgm);
            SoundEffects = byteOptionFactory(OptionKind.SoundEffects, OptionOffsets.SoundEffects);
            Voice = byteOptionFactory(OptionKind.Voice, OptionOffsets.Voice);
            SystemSounds = byteOptionFactory(OptionKind.SystemSounds, OptionOffsets.SystemSounds);
            AmbientSounds = byteOptionFactory(OptionKind.AmbientSounds, OptionOffsets.AmbientSounds);
            Performance = byteOptionFactory(OptionKind.Performance, OptionOffsets.Performance);

            Self = byteOptionFactory(OptionKind.Self, OptionOffsets.Self);
            Party = byteOptionFactory(OptionKind.Party, OptionOffsets.Party);
            OtherPCs = byteOptionFactory(OptionKind.OtherPCs, OptionOffsets.OtherPCs);

            MasterVolumeMuted = booleanOptionFactory(OptionKind.MasterMuted, OptionOffsets.MasterVolumeMuted);
            BgmMuted = booleanOptionFactory(OptionKind.BgmMuted, OptionOffsets.BgmMuted);
            SoundEffectsMuted = booleanOptionFactory(OptionKind.SoundEffectsMuted, OptionOffsets.SoundEffectsMuted);
            VoiceMuted = booleanOptionFactory(OptionKind.VoiceMuted, OptionOffsets.VoiceMuted);
            SystemSoundsMuted = booleanOptionFactory(OptionKind.SystemSoundsMuted, OptionOffsets.SystemSoundsMuted);
            AmbientSoundsMuted = booleanOptionFactory(OptionKind.AmbientSoundsMuted, OptionOffsets.AmbientSoundsMuted);
            PerformanceMuted = booleanOptionFactory(OptionKind.PerformanceMuted, OptionOffsets.PerformanceMuted);

            EqualizerMode = new EqualizerModeOption
            {
                BaseAddress = BaseAddress,
                Offset = OptionOffsets.EqualizerMode,
                Kind = OptionKind.EqualizerMode,
                SetFunction = setOption,
            };
        }

        public void Dispose()
        {
            this.setOptionHook?.Disable();
            this.setOptionHook?.Dispose();
        }
    }
}
