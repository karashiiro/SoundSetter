using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SoundSetter
{
    public class VolumeControls : IDisposable
    {
        private const int MasterVolumeOffset = 34048;
        private const int BgmOffset = 34104;
        private const int SoundEffectsOffset = 34160;
        private const int VoiceOffset = 34216;
        private const int SystemSoundsOffset = 34328;
        private const int AmbientSoundsOffset = 34272;
        private const int PerformanceOffset = 34384;

        private const int SelfOffset = 34440;
        private const int PartyOffset = 34496;
        private const int OtherPCsOffset = 34552;

        private const int MasterVolumeMutedOffset = 34608;
        private const int BgmMutedOffset = 34664;
        private const int SoundEffectsMutedOffset = 34720;
        private const int VoiceMutedOffset = 34776;
        private const int SystemSoundsMutedOffset = 34888;
        private const int AmbientSoundsMutedOffset = 34832;
        private const int PerformanceMutedOffset = 34944;

        private const int EqualizerModeOffset = 35336;

        private delegate IntPtr SetSoundOptionDelegate(IntPtr address, ulong value, IntPtr p3, float p4);
        private Hook<SetSoundOptionDelegate> setSoundOptionHook;

        public int Master { get; set; }
        public int Bgm { get; set; }
        public int SoundEffects { get; set; }
        public int Voice { get; set; }
        public int SystemSounds { get; set; }
        public int AmbientSounds { get; set; }
        public int Performance { get; set; }

        public int Self { get; set; }
        public int Party { get; set; }
        public int OtherPCs { get; set; }

        public bool MasterMuted { get; set; }
        public bool BgmMuted { get; set; }
        public bool SoundEffectsMuted { get; set; }
        public bool VoiceMuted { get; set; }
        public bool SystemSoundsMuted { get; set; }
        public bool AmbientSoundsMuted { get; set; }
        public bool PerformanceMuted { get; set; }

        public EqualizerMode.Enum EqualizerMode { get; set; }

        public VolumeControls(SigScanner scanner)
        {
            HookVolume(scanner);
        }

        private void HookVolume(SigScanner scanner)
        {
            try
            {
                var setSoundOption = scanner.ScanText("48 83 EC 28 44 8B 49 ?? 44 8B 51 ?? 41 3B D1 72 28 3B 51 ?? 77 23");
                this.setSoundOptionHook = new Hook<SetSoundOptionDelegate>(setSoundOption, (SetSoundOptionDelegate)OnSetSoundOption);
                this.setSoundOptionHook.Enable();
                PluginLog.Log($"SetSoundOption found at 0x{setSoundOption.ToString("X")}");
            }
            catch (Exception e)
            {
                PluginLog.LogError($"Failed to hook volume method! Full message:\n{e}");
            }
        }

        private bool readAllOnce;
        private IntPtr OnSetSoundOption(IntPtr address, ulong value, IntPtr p3, float p4)
        {
            PluginLog.Log($"Hit! Args: {address}, {value}, {p3}, {p4}");
            LoadBaseConfigAddress(address);
            if (!this.readAllOnce)
            {
                // Initializes the plugin state
                ReadAllOptions();
                this.readAllOnce = true;
            }
            ReadNewOption(address, value);
            return this.setSoundOptionHook.Original(address, value, p3, p4);
        }

        private IntPtr BaseConfigAddress { get; set; }
        private readonly List<IntPtr> addresses = new List<IntPtr>();
        private void LoadBaseConfigAddress(IntPtr next)
        {
            if (this.addresses.Count == 20) return;
            this.addresses.Add(next);
            BaseConfigAddress = this.addresses // Get the most frequent address in the list
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .First();
        }

        private void ReadAllOptions()
        {
            Master = Marshal.ReadByte(BaseConfigAddress + MasterVolumeOffset);
            Bgm = Marshal.ReadByte(BaseConfigAddress + BgmOffset);
            SoundEffects = Marshal.ReadByte(BaseConfigAddress + SoundEffectsOffset);
            Voice = Marshal.ReadByte(BaseConfigAddress + VoiceOffset);
            SystemSounds = Marshal.ReadByte(BaseConfigAddress + SystemSoundsOffset);
            AmbientSounds = Marshal.ReadByte(BaseConfigAddress + AmbientSoundsOffset);
            Performance = Marshal.ReadByte(BaseConfigAddress + PerformanceOffset);
            Self = Marshal.ReadByte(BaseConfigAddress + SelfOffset);
            Party = Marshal.ReadByte(BaseConfigAddress + PartyOffset);
            OtherPCs = Marshal.ReadByte(BaseConfigAddress + OtherPCsOffset);

            MasterMuted = Marshal.ReadByte(BaseConfigAddress + MasterVolumeMutedOffset) == 0;
            BgmMuted = Marshal.ReadByte(BaseConfigAddress + BgmMutedOffset) == 0;
            SoundEffectsMuted = Marshal.ReadByte(BaseConfigAddress + SoundEffectsMutedOffset) == 0;
            VoiceMuted = Marshal.ReadByte(BaseConfigAddress + VoiceMutedOffset) == 0;
            SystemSoundsMuted = Marshal.ReadByte(BaseConfigAddress + SystemSoundsMutedOffset) == 0;
            AmbientSoundsMuted = Marshal.ReadByte(BaseConfigAddress + AmbientSoundsMutedOffset) == 0;
            PerformanceMuted = Marshal.ReadByte(BaseConfigAddress + PerformanceMutedOffset) == 0;

            EqualizerMode = (EqualizerMode.Enum)Marshal.ReadByte(BaseConfigAddress + EqualizerModeOffset);
        }

        private void ReadNewOption(IntPtr address, ulong value)
        {
            if (address == BaseConfigAddress + MasterVolumeOffset)
                Master = (int)value;
            else if (address == BaseConfigAddress + BgmOffset)
                Bgm = (int)value;
            else if (address == BaseConfigAddress + SoundEffectsOffset)
                SoundEffects = (int)value;
            else if (address == BaseConfigAddress + VoiceOffset)
                Voice = (int)value;
            else if (address == BaseConfigAddress + SystemSoundsOffset)
                SystemSounds = (int)value;
            else if (address == BaseConfigAddress + AmbientSoundsOffset)
                AmbientSounds = (int)value;
            else if (address == BaseConfigAddress + PerformanceOffset)
                Performance = (int)value;
            else if (address == BaseConfigAddress + SelfOffset)
                Self = (int)value;
            else if (address == BaseConfigAddress + PartyOffset)
                Party = (int)value;
            else if (address == BaseConfigAddress + OtherPCsOffset)
                OtherPCs = (int)value;
            else if (address == BaseConfigAddress + MasterVolumeMutedOffset)
                MasterMuted = value == 0;
            else if (address == BaseConfigAddress + BgmMutedOffset)
                BgmMuted = value == 0;
            else if (address == BaseConfigAddress + SoundEffectsMutedOffset)
                SoundEffectsMuted = value == 0;
            else if (address == BaseConfigAddress + VoiceMutedOffset)
                VoiceMuted = value == 0;
            else if (address == BaseConfigAddress + SystemSoundsMutedOffset)
                SystemSoundsMuted = value == 0;
            else if (address == BaseConfigAddress + AmbientSoundsMutedOffset)
                AmbientSoundsMuted = value == 0;
            else if (address == BaseConfigAddress + PerformanceMutedOffset)
                PerformanceMuted = value == 0;
            else if (address == BaseConfigAddress + EqualizerModeOffset)
                EqualizerMode = (EqualizerMode.Enum)value;
        }

        public void Dispose()
        {
            this.setSoundOptionHook?.Disable();
            this.setSoundOptionHook?.Dispose();
        }
    }
}
