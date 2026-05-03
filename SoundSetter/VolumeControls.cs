using Dalamud.Game.Config;
using Dalamud.Plugin.Services;
using SoundSetter.OptionInternals;
using System;

namespace SoundSetter
{
    public class VolumeControls : IDisposable
    {
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

        public VolumeControls(IGameConfig gameConfig, IPluginLog log)
        {
            InitializeOptions(gameConfig, log);
        }

        private void InitializeOptions(IGameConfig gameConfig, IPluginLog log)
        {
            PlaySoundsWhileWindowIsNotActive = MakeBoolSoundSettings(SystemConfigOption.IsSoundAlways);
            PlaySoundsWhileWindowIsNotActiveBGM = MakeBoolSoundSettings(SystemConfigOption.IsSoundBgmAlways);
            PlaySoundsWhileWindowIsNotActiveSoundEffects = MakeBoolSoundSettings(SystemConfigOption.IsSoundSeAlways);
            PlaySoundsWhileWindowIsNotActiveVoice = MakeBoolSoundSettings(SystemConfigOption.IsSoundVoiceAlways);
            PlaySoundsWhileWindowIsNotActiveSystemSounds = MakeBoolSoundSettings(SystemConfigOption.IsSoundSystemAlways);
            PlaySoundsWhileWindowIsNotActiveAmbientSounds = MakeBoolSoundSettings(SystemConfigOption.IsSoundEnvAlways);
            PlaySoundsWhileWindowIsNotActivePerformance = MakeBoolSoundSettings(SystemConfigOption.IsSoundPerformAlways);

            PlayMusicWhenMounted = MakeBoolSoundPlay(SystemConfigOption.SoundChocobo);
            EnableNormalBattleMusic = MakeBoolSoundPlay(SystemConfigOption.SoundFieldBattle);
            EnableCityStateBGM = MakeBoolSoundPlay(SystemConfigOption.SoundHousing);
            PlaySystemSounds = MakeBoolSoundPlay(SystemConfigOption.SoundCfTimeCount);

            MasterVolume = MakeByte(SystemConfigOption.SoundMaster);
            Bgm = MakeByte(SystemConfigOption.SoundBgm);
            SoundEffects = MakeByte(SystemConfigOption.SoundSe);
            Voice = MakeByte(SystemConfigOption.SoundVoice);
            SystemSounds = MakeByte(SystemConfigOption.SoundSystem);
            AmbientSounds = MakeByte(SystemConfigOption.SoundEnv);
            Performance = MakeByte(SystemConfigOption.SoundPerform);

            Self = MakeByte(SystemConfigOption.SoundPlayer);
            Party = MakeByte(SystemConfigOption.SoundParty);
            OtherPCs = MakeByte(SystemConfigOption.SoundOther);

            MasterVolumeMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndMaster);
            BgmMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndBgm);
            SoundEffectsMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndSe);
            VoiceMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndVoice);
            SystemSoundsMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndSystem);
            AmbientSoundsMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndEnv);
            PerformanceMuted = MakeBoolSoundPlay(SystemConfigOption.IsSndPerform);

            EqualizerMode = new EqualizerModeOption(log)
            {
                GameConfig = gameConfig,
                ConfigOption = SystemConfigOption.SoundEqualizerType,
                CfgSection = "SoundPlay Settings",
            };
            return;

            ByteOption MakeByte(SystemConfigOption opt) => new(log)
            {
                GameConfig = gameConfig,
                ConfigOption = opt,
                CfgSection = "SoundPlay Settings",
            };

            BooleanOption MakeBoolSoundPlay(SystemConfigOption opt) => new(log)
            {
                GameConfig = gameConfig,
                ConfigOption = opt,
                CfgSection = "SoundPlay Settings",
            };

            BooleanOption MakeBoolSoundSettings(SystemConfigOption opt) => new(log)
            {
                GameConfig = gameConfig,
                ConfigOption = opt,
                CfgSection = "Sound Settings",
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
            GC.SuppressFinalize(this);
        }
    }
}