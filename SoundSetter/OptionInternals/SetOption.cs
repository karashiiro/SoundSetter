using System;
using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate IntPtr SetOptionDelegate(IntPtr baseAddress, OptionKind kind, ulong value, ulong unk1, ulong unk2, ulong unk3);
}