namespace SoundSetter.OptionInternals
{
    public enum OptionKind : ulong
    {
        PlaySoundsWhileWindowIsNotActive = 18,

        PlayMusicWhenMounted,
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

        PlaySoundsWhileWindowIsNotActiveBGM = 44,
        PlaySoundsWhileWindowIsNotActiveSoundEffects,
        PlaySoundsWhileWindowIsNotActiveVoice,
        PlaySoundsWhileWindowIsNotActiveSystemSounds,
        PlaySoundsWhileWindowIsNotActiveAmbientSounds,
        PlaySoundsWhileWindowIsNotActivePerformance,

        EqualizerMode = 50,
    }
}