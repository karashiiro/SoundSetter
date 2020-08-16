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
            if (ImGui.Button(this.vc.MasterMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.MasterMuted = !this.vc.MasterMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var master = this.vc.Master;
            if (ImGui.SliderInt("Master Volume", ref master, 0, 100))
            {
                this.vc.Master = master;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(this.vc.BgmMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.BgmMuted = !this.vc.BgmMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var bgm = this.vc.Bgm;
            if (ImGui.SliderInt("BGM", ref bgm, 0, 100))
            {
                this.vc.Bgm = bgm;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(this.vc.SoundEffectsMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.SoundEffectsMuted = !this.vc.SoundEffectsMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var soundEffects = this.vc.SoundEffects;
            if (ImGui.SliderInt("Sound Effects", ref soundEffects, 0, 100))
            {
                this.vc.SoundEffects = soundEffects;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(this.vc.VoiceMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.VoiceMuted = !this.vc.VoiceMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var voice = this.vc.Voice;
            if (ImGui.SliderInt("Voice", ref voice, 0, 100))
            {
                this.vc.Voice = voice;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(this.vc.SystemSoundsMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.SystemSoundsMuted = !this.vc.SystemSoundsMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var systemSounds = this.vc.SystemSounds;
            if (ImGui.SliderInt("System Sounds", ref systemSounds, 0, 100))
            {
                this.vc.SystemSounds = systemSounds;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(this.vc.AmbientSoundsMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.AmbientSoundsMuted = !this.vc.AmbientSoundsMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var ambientSounds = this.vc.AmbientSounds;
            if (ImGui.SliderInt("Ambient Sounds", ref ambientSounds, 0, 100))
            {
                this.vc.AmbientSounds = ambientSounds;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(this.vc.PerformanceMuted ? volumeOn : volumeOff, buttonSize))
            {
                this.vc.PerformanceMuted = !this.vc.PerformanceMuted;
            }
            ImGui.PopFont();
            ImGui.SameLine();
            var performance = this.vc.Performance;
            if (ImGui.SliderInt("Performance", ref performance, 0, 100))
            {
                this.vc.Performance = performance;
            }

            var self = this.vc.Self;
            if (ImGui.SliderInt("Self", ref self, 0, 100))
            {
                this.vc.Self = self;
            }

            var party = this.vc.Party;
            if (ImGui.SliderInt("Party", ref party, 0, 100))
            {
                this.vc.Party = party;
            }

            var others = this.vc.OtherPCs;
            if (ImGui.SliderInt("OtherPCs", ref others, 0, 100))
            {
                this.vc.OtherPCs = others;
            }

            var eqMode = (int)this.vc.EqualizerMode;
            if (ImGui.Combo("Equalizer Mode", ref eqMode, EqualizerMode.Names, EqualizerMode.Names.Length))
            {
                this.vc.EqualizerMode = (EqualizerMode.Enum)eqMode;
            }

            ImGui.End();
        }
    }
}
