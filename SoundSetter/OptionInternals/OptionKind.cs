namespace SoundSetter.OptionInternals
{
    public enum OptionKind : ulong
    {
        PlayMusicWhenMounted = 19,
        EnableNormalBattleMusic,
        EnableCityStateBGM,
        PlaySystemSounds,

        Master = 24,
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

        EqualizerMode = 50,
    }
}