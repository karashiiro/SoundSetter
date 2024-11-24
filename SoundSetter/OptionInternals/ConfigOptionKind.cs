using System;

namespace SoundSetter.OptionInternals;

public static class ConfigOptionKind
{
    /// <summary>
    /// Config option IDs as understood by ConfigModule.
    /// </summary>
    public enum ConfigEnum
    {
        PlaySoundsWhileWindowIsNotActive = 70,
        PlayMusicWhenMounted,
        EnableNormalBattleMusic,
        EnableCityStateBGM,
        PlaySystemSounds,

        Master = 76,
        Bgm,
        SoundEffects,
        Voice,
        SystemSounds,
        AmbientSounds,
        Performance,

        Self,
        Party,
        OtherPCs,

        MasterMuted,
        BgmMuted,
        SoundEffectsMuted,
        VoiceMuted,
        SystemSoundsMuted,
        AmbientSoundsMuted,
        PerformanceMuted,

        PlaySoundsWhileWindowIsNotActiveBGM = 96,
        PlaySoundsWhileWindowIsNotActiveSoundEffects,
        PlaySoundsWhileWindowIsNotActiveVoice,
        PlaySoundsWhileWindowIsNotActiveSystemSounds,
        PlaySoundsWhileWindowIsNotActiveAmbientSounds,
        PlaySoundsWhileWindowIsNotActivePerformance,

        EqualizerMode = 102,
    }

    public static ConfigEnum GetConfigEnum(OptionKind kind)
    {
        var name = Enum.GetName(typeof(OptionKind), kind);
        ArgumentException.ThrowIfNullOrEmpty(name);
        return (ConfigEnum)Enum.Parse(typeof(ConfigEnum), name);
    }
}