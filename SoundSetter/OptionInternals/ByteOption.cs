using System;
using System.Runtime.InteropServices;
using static SoundSetter.OptionInternals.SetOption;

namespace SoundSetter.OptionInternals
{
    public class ByteOption : Option<byte>
    {
        public override byte GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset);
        }

        public override void SetValue(byte value)
        {
            SetFunction(BaseAddress, Kind, value);
        }

        public static Func<OptionKind, int, ByteOption> CreateFactory(IntPtr baseAddress, SetOptionDelegate setFunction)
        {
            return (optionKind, offset) => new ByteOption
            {
                BaseAddress = baseAddress,
                Offset = offset,
                Kind = optionKind,
                SetFunction = setFunction,
            };
        }
    }
}