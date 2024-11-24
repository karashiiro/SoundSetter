using System;

namespace SoundSetter.OptionInternals;

public static class OptionKind
{
    public enum ConfigEnum : ulong
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

    public enum UIEnum : ulong
    {
        PlaySoundsWhileWindowIsNotActive = 17,
        PlayMusicWhenMounted,
        EnableNormalBattleMusic,
        EnableCityStateBGM,
        PlaySystemSounds,

        Master = 23,
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

        PlaySoundsWhileWindowIsNotActiveBGM = 43,
        PlaySoundsWhileWindowIsNotActiveSoundEffects,
        PlaySoundsWhileWindowIsNotActiveVoice,
        PlaySoundsWhileWindowIsNotActiveSystemSounds,
        PlaySoundsWhileWindowIsNotActiveAmbientSounds,
        PlaySoundsWhileWindowIsNotActivePerformance,

        EqualizerMode = 49,
    }

    public static ConfigEnum GetConfigEnum(UIEnum uiEnum)
    {
        var name = Enum.GetName(typeof(UIEnum), uiEnum);
        ArgumentException.ThrowIfNullOrEmpty(name);
        return (ConfigEnum)Enum.Parse(typeof(ConfigEnum), name);
    }
}