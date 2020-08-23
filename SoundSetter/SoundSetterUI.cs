using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using SoundSetter.OptionInternals;

namespace SoundSetter
{
    public class SoundSetterUi
    {
        private readonly Configuration config;
        private readonly VolumeControls vc;

        public bool IsVisible { get; set; }

        public SoundSetterUi(VolumeControls vc, Configuration config)
        {
            this.vc = vc;
            this.config = config;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            var pVisible = IsVisible;
            ImGui.Begin("SoundSetter Configuration", ref pVisible, ImGuiWindowFlags.AlwaysAutoResize);
            IsVisible = pVisible;

            if (this.vc.BaseAddress == IntPtr.Zero)
            {
                Fail();
            }
            else
            {
                Settings();
            }

            ImGui.End();
        }

        private void Settings()
        {
            var buttonSize = new Vector2(23, 23);

            ImGui.Text("Plugin Settings");

            ImGui.PushItemWidth(100f);
            var kItem1 = VirtualKey.EnumToIndex(this.config.ModifierKey);
            if (ImGui.Combo("##SoundSetterKeybind1", ref kItem1, VirtualKey.Names.Take(3).ToArray(), 3))
            {
                this.config.ModifierKey = VirtualKey.IndexToEnum(kItem1);
                this.config.Save();
            }
            ImGui.SameLine();
            var kItem2 = VirtualKey.EnumToIndex(this.config.MajorKey) - 3;
            if (ImGui.Combo("Keybind##SoundSetterKeybind2", ref kItem2, VirtualKey.Names.Skip(3).ToArray(), VirtualKey.Names.Length - 3))
            {
                this.config.MajorKey = VirtualKey.IndexToEnum(kItem2) + 3;
                this.config.Save();
            }
            ImGui.PopItemWidth();

            ImGui.Text("Volume Settings");

            ImGui.PushFont(UiBuilder.IconFont);
            var masterVolumeMuted = this.vc.MasterVolumeMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(masterVolumeMuted, nameof(masterVolumeMuted)), buttonSize))
            {
                this.vc.MasterVolumeMuted.SetValue(!masterVolumeMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var masterVolume = (int)this.vc.MasterVolume.GetValue();
            if (ImGui.SliderInt("Master Volume", ref masterVolume, 0, 100))
            {
                this.vc.MasterVolume.SetValue((byte)masterVolume);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var bgmMuted = this.vc.BgmMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(bgmMuted, nameof(bgmMuted)), buttonSize))
            {
                this.vc.BgmMuted.SetValue(!bgmMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var bgm = (int)this.vc.Bgm.GetValue();
            if (ImGui.SliderInt("BGM", ref bgm, 0, 100))
            {
                this.vc.Bgm.SetValue((byte)bgm);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var soundEffectsMuted = this.vc.SoundEffectsMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(soundEffectsMuted, nameof(soundEffectsMuted)), buttonSize))
            {
                this.vc.SoundEffectsMuted.SetValue(!soundEffectsMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var soundEffects = (int)this.vc.SoundEffects.GetValue();
            if (ImGui.SliderInt("Sound Effects", ref soundEffects, 0, 100))
            {
                this.vc.SoundEffects.SetValue((byte)soundEffects);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var voiceMuted = this.vc.VoiceMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(voiceMuted, nameof(voiceMuted)), buttonSize))
            {
                this.vc.VoiceMuted.SetValue(!voiceMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var voice = (int)this.vc.Voice.GetValue();
            if (ImGui.SliderInt("Voice", ref voice, 0, 100))
            {
                this.vc.Voice.SetValue((byte)voice);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var systemSoundsMuted = this.vc.SystemSoundsMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(systemSoundsMuted, nameof(systemSoundsMuted)), buttonSize))
            {
                this.vc.SystemSoundsMuted.SetValue(!systemSoundsMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var systemSounds = (int)this.vc.SystemSounds.GetValue();
            if (ImGui.SliderInt("System Sounds", ref systemSounds, 0, 100))
            {
                this.vc.SystemSounds.SetValue((byte)systemSounds);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var ambientSoundsMuted = this.vc.AmbientSoundsMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(ambientSoundsMuted, nameof(ambientSoundsMuted)), buttonSize))
            {
                this.vc.AmbientSoundsMuted.SetValue(!ambientSoundsMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var ambientSounds = (int)this.vc.AmbientSounds.GetValue();
            if (ImGui.SliderInt("Ambient Sounds", ref ambientSounds, 0, 100))
            {
                this.vc.AmbientSounds.SetValue((byte)ambientSounds);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var performanceMuted = this.vc.PerformanceMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(performanceMuted, nameof(performanceMuted)), buttonSize))
            {
                this.vc.PerformanceMuted.SetValue(!performanceMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var performance = (int)this.vc.Performance.GetValue();
            if (ImGui.SliderInt("Performance", ref performance, 0, 100))
            {
                this.vc.Performance.SetValue((byte)performance);
            }

            ImGui.Text("Player Effects Volume");

            var self = (int)this.vc.Self.GetValue();
            if (ImGui.SliderInt("Self", ref self, 0, 100))
            {
                this.vc.Self.SetValue((byte)self);
            }

            var party = (int)this.vc.Party.GetValue();
            if (ImGui.SliderInt("Party", ref party, 0, 100))
            {
                this.vc.Party.SetValue((byte)party);
            }

            var others = (int)this.vc.OtherPCs.GetValue();
            if (ImGui.SliderInt("Other PCs", ref others, 0, 100))
            {
                this.vc.OtherPCs.SetValue((byte)others);
            }

            ImGui.Text("Equalizer");

            var eqMode = (int)this.vc.EqualizerMode.GetValue();
            if (ImGui.Combo("Mode", ref eqMode, EqualizerMode.Names, EqualizerMode.Names.Length))
            {
                this.vc.EqualizerMode.SetValue((EqualizerMode.Enum)eqMode);
            }
        }

        private static void Fail()
        {
            ImGui.Text("This appears to be your first installation of this plugin.\nPlease manually change a volume setting once in order to initialize the plugin.");
        }

        private static string VolumeButtonName(bool state, string internalName)
        {
            return $"{(state ? FontAwesomeIcon.VolumeOff.ToIconString() : FontAwesomeIcon.VolumeUp.ToIconString())}#SoundSetter{internalName}";
        }
    }
}
