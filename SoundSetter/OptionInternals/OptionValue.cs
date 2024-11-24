using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals;

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public struct OptionValue
{
    public ulong Value1 { get; set; }
    public ulong Value2 { get; set; }

    public static unsafe ref OptionValue FromOptionValue(ref ConfigModule.OptionValue optionValue)
    {
        // Cast from an opaque struct to this type so we can access the values
        Debug.Assert(sizeof(ConfigModule.OptionValue) == sizeof(OptionValue));
        return ref Unsafe.As<ConfigModule.OptionValue, OptionValue>(ref optionValue);
    }
}