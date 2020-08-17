using System;
using static SoundSetter.SetOption;

namespace SoundSetter
{
    public abstract class Option<TManagedValue> where TManagedValue : struct
    {
        public OptionKind Kind { get; set; }
        public IntPtr BaseAddress { get; set; }
        public int Offset { get; set; }
        public SetOptionDelegate SetFunction { get; set; }

        public abstract TManagedValue GetValue();
        public abstract void SetValue(TManagedValue value);
    }
}