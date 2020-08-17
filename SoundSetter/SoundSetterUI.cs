using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;

namespace SoundSetter
{
    public class SoundSetterUi
    {
        private readonly VolumeControls vc;

        public bool IsVisible { get; set; }

        public SoundSetterUi(VolumeControls vc)
        {
            this.vc = vc;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            var buttonSize = new Vector2(23, 23);
            var volumeOff = FontAwesomeIcon.VolumeOff.ToIconString();
            var volumeOn = FontAwesomeIcon.VolumeUp.ToIconString();

            var pVisible = IsVisible;
            ImGui.Begin("SoundSetter Configuration", ref pVisible, ImGuiWindowFlags.AlwaysAutoResize);
            IsVisible = pVisible;

            ImGui.PushFont(UiBuilder.IconFont);
            var masterMuted = this.vc.MasterMuted.GetValue();
            if (ImGui.Button(masterMuted ? volumeOff : volumeOn, buttonSize))
            {
                this.vc.MasterMuted.SetValue(!masterMuted);
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var master = (int)this.vc.Master.GetValue();
            if (ImGui.SliderInt("Master Volume", ref master, 0, 100))
            {
                this.vc.Master.SetValue((byte)master);
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var bgmMuted = this.vc.BgmMuted.GetValue();
            if (ImGui.Button(bgmMuted ? volumeOff : volumeOn, buttonSize))
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
            if (ImGui.Button(soundEffectsMuted ? volumeOff : volumeOn, buttonSize))
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
            if (ImGui.Button(voiceMuted ? volumeOff : volumeOn, buttonSize))
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
            if (ImGui.Button(systemSoundsMuted ? volumeOff : volumeOn, buttonSize))
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
            if (ImGui.Button(ambientSoundsMuted ? volumeOff : volumeOn, buttonSize))
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
            if (ImGui.Button(performanceMuted ? volumeOff : volumeOn, buttonSize))
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

            var eqMode = (int)this.vc.EqualizerMode.GetValue();
            if (ImGui.Combo("Equalizer Mode", ref eqMode, EqualizerMode.Names, EqualizerMode.Names.Length))
            {
                this.vc.EqualizerMode.SetValue((EqualizerMode.Enum)eqMode);
            }

            ImGui.End();
        }
    }
}
