using System;
using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    public class SetOption
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr SetOptionDelegate(IntPtr baseAddress, OptionKind kind, ulong value, ulong unknown = 2);
    }
}