using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public unsafe delegate nint SetOptionDelegate(ConfigModule* configModule, OptionKind kind, ulong value, ulong unk1, ulong unk2, ulong unk3);
}