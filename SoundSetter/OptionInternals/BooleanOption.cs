using System;
using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    public class BooleanOption : Option<bool>
    {
        public bool Hack { get; set; } = true;

        public override bool GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset) != 0;
        }

        public override void SetValue(bool value)
        {
            var toWrite = value ? 1UL : 0UL;
            SetFunction(BaseAddress, Kind, toWrite);

            // This is a hack to make the native text commands work as expected; do not reuse this
            // or expect it to work elsewhere.
            if (Hack) Marshal.WriteInt64(BaseAddress, Offset - 21504, (long)toWrite);
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