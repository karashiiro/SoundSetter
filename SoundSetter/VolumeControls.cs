using Dalamud.Game;
using System;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin;
using static SoundSetter.SetOption;

namespace SoundSetter
{
    public class VolumeControls : IDisposable
    {
        private readonly Hook<SetOptionDelegate> setOptionHook;

        private IntPtr configuration;

        public ByteOption Master { get; }
        public ByteOption Bgm { get; }
        public ByteOption SoundEffects { get; }
        public ByteOption Voice { get; }
        public ByteOption SystemSounds { get; }
        public ByteOption AmbientSounds { get; }
        public ByteOption Performance { get; }

        public ByteOption Self { get; }
        public ByteOption Party { get; }
        public ByteOption OtherPCs { get; }

        public BooleanOption MasterMuted { get; }
        public BooleanOption BgmMuted { get; }
        public BooleanOption SoundEffectsMuted { get; }
        public BooleanOption VoiceMuted { get; }
        public BooleanOption SystemSoundsMuted { get; }
        public BooleanOption AmbientSoundsMuted { get; }
        public BooleanOption PerformanceMuted { get; }

        public EqualizerModeOption EqualizerMode { get; }

        public VolumeControls(SigScanner scanner)
        {
            SetOptionDelegate setOption;
            try
            {
                var setConfigurationPtr = scanner.ScanText("89 54 24 ?? 53 55 57 41 54 41 55 41 56 48 83 EC 48 8B C2 45 8B E0 44 8B D2 45 32 F6 44 8B C2 45 32 ED");
                setOption = Marshal.GetDelegateForFunctionPointer<SetOptionDelegate>(setConfigurationPtr);
                this.setOptionHook = new Hook<SetOptionDelegate>(setConfigurationPtr, new SetOptionDelegate(
                    (baseAddress, kind, value, unknown) =>
                    {
                        this.configuration = baseAddress;
                        PluginLog.Log($"0x{baseAddress.ToString("X")} {kind}, {value}, {unknown}");
                        return this.setOptionHook.Original(baseAddress, kind, value, unknown);
                    }));
                this.setOptionHook.Enable();
            }
            catch (Exception e)
            {
                PluginLog.LogError($"Failed to hook configuration set method! Full error:\n{e}");
                return;
            }

            var byteOptionFactory = ByteOption.CreateFactory(this.configuration, setOption);
            var booleanOptionFactory = BooleanOption.CreateFactory(this.configuration, setOption);

            Master = byteOptionFactory(OptionKind.Master, OptionOffsets.Master);
            Bgm = byteOptionFactory(OptionKind.Bgm, OptionOffsets.Bgm);
            SoundEffects = byteOptionFactory(OptionKind.SoundEffects, OptionOffsets.SoundEffects);
            Voice = byteOptionFactory(OptionKind.Voice, OptionOffsets.Voice);
            SystemSounds = byteOptionFactory(OptionKind.SystemSounds, OptionOffsets.SystemSounds);
            AmbientSounds = byteOptionFactory(OptionKind.AmbientSounds, OptionOffsets.AmbientSounds);
            Performance = byteOptionFactory(OptionKind.Performance, OptionOffsets.Performance);

            Self = byteOptionFactory(OptionKind.Self, OptionOffsets.Self);
            Party = byteOptionFactory(OptionKind.Party, OptionOffsets.Party);
            OtherPCs = byteOptionFactory(OptionKind.OtherPCs, OptionOffsets.OtherPCs);

            MasterMuted = booleanOptionFactory(OptionKind.MasterMuted, OptionOffsets.MasterMuted);
            BgmMuted = booleanOptionFactory(OptionKind.BgmMuted, OptionOffsets.BgmMuted);
            SoundEffectsMuted = booleanOptionFactory(OptionKind.SoundEffectsMuted, OptionOffsets.SoundEffectsMuted);
            VoiceMuted = booleanOptionFactory(OptionKind.VoiceMuted, OptionOffsets.VoiceMuted);
            SystemSoundsMuted = booleanOptionFactory(OptionKind.SystemSoundsMuted, OptionOffsets.SystemSoundsMuted);
            AmbientSoundsMuted = booleanOptionFactory(OptionKind.AmbientSoundsMuted, OptionOffsets.AmbientSoundsMuted);
            PerformanceMuted = booleanOptionFactory(OptionKind.PerformanceMuted, OptionOffsets.PerformanceMuted);

            EqualizerMode = new EqualizerModeOption
            {
                BaseAddress = this.configuration,
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
