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

        public ByteOption Master { get; private set; }
        public ByteOption Bgm { get; private set; }
        public ByteOption SoundEffects { get; private set; }
        public ByteOption Voice { get; private set; }
        public ByteOption SystemSounds { get; private set; }
        public ByteOption AmbientSounds { get; private set; }
        public ByteOption Performance { get; private set; }

        public ByteOption Self { get; private set; }
        public ByteOption Party { get; private set; }
        public ByteOption OtherPCs { get; private set; }

        public BooleanOption MasterMuted { get; private set; }
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
                var setConfigurationPtr = scanner.ScanText("89 54 24 ?? 53 55 57 41 54 41 55 41 56 48 83 EC 48 8B C2 45 8B E0 44 8B D2 45 32 F6 44 8B C2 45 32 ED");
                var setOption = Marshal.GetDelegateForFunctionPointer<SetOptionDelegate>(setConfigurationPtr);
                this.setOptionHook = new Hook<SetOptionDelegate>(setConfigurationPtr, new SetOptionDelegate((baseAddress, kind, value, unknown) =>
                {
                    if (Master == null) InitializeOptions(setOption, baseAddress);
                    PluginLog.Log($"{baseAddress}, {kind}, {value}, {unknown}");
                    return this.setOptionHook.Original(baseAddress, kind, value, unknown);
                }));
                this.setOptionHook.Enable();
            }
            catch (Exception e)
            {
                PluginLog.LogError($"Failed to hook configuration set method! Full error:\n{e}");
            }
        }

        private void InitializeOptions(SetOptionDelegate setOption, IntPtr configuration)
        {
            var byteOptionFactory = ByteOption.CreateFactory(configuration, setOption);
            var booleanOptionFactory = BooleanOption.CreateFactory(configuration, setOption);

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
                BaseAddress = configuration,
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
