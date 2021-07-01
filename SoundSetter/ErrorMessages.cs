namespace SoundSetter
{
    public static class ErrorMessages
    {
        public const string AdjustCommand =
            "The provided input is invalid. Refer to the following as examples of valid inputs:\n" +
            "  Set the volume to 10: {0} 10\n" +
            "  Increase the volume by 10: {0} +10\n" +
            "  Decrease the volume by 10: {0} -10\n" +
            "  Mute the volume option: {0} mute\n" +
            "  Unmute the volume option: {0} unmute\n" +
            "  Toggle the volume option: {0}";
    }
}