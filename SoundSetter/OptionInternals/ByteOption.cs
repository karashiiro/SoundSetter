using System;
using System.Dynamic;
using System.Runtime.InteropServices;

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
            NotifyOptionChanged(value);
        }

        public static Func<OptionKind, int, ByteOption> CreateFactory(IntPtr baseAddress, Action<ExpandoObject> onChange, SetOptionDelegate setFunction)
        {
            return (optionKind, offset) => new ByteOption
            {
                BaseAddress = baseAddress,
                Offset = offset,
                Kind = optionKind,
                OnChange = onChange,
                SetFunction = setFunction,
            };
        }
    }
}