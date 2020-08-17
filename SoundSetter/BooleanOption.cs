using System;
using System.Runtime.InteropServices;
using static SoundSetter.SetOption;

namespace SoundSetter
{
    public class BooleanOption : Option<bool>
    {
        public override bool GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset) != 0;
        }

        public override void SetValue(bool value)
        {
            SetFunction(BaseAddress, Kind, value ? 1UL : 0UL);
        }

        public static Func<OptionKind, int, BooleanOption> CreateFactory(IntPtr baseAddress, SetOptionDelegate setFunction)
        {
            return (optionKind, offset) => new BooleanOption
            {
                BaseAddress = baseAddress,
                Offset = offset,
                Kind = optionKind,
                SetFunction = setFunction,
            };
        }
    }
}