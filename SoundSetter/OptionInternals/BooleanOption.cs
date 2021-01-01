using System;
using System.Dynamic;
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
            var toWrite = value ? 1U : 0U;
            SetFunction(BaseAddress, Kind, toWrite);
            NotifyOptionChanged(value);

            // This is a hack to make the native text commands work as expected; do not reuse this
            // or expect it to work elsewhere.
            if (Hack) Marshal.WriteInt32(BaseAddress, Offset - 21504, (int)toWrite);
        }

        public static Func<OptionKind, int, BooleanOption> CreateFactory(IntPtr baseAddress, Action<ExpandoObject> onChange, SetOptionDelegate setFunction)
        {
            return (optionKind, offset) => new BooleanOption
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