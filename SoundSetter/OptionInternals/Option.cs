using System;
using System.Dynamic;

namespace SoundSetter.OptionInternals
{
    public abstract class Option<TManagedValue> where TManagedValue : struct
    {
        public OptionKind Kind { get; set; }
        public IntPtr BaseAddress { get; set; }
        public int Offset { get; set; }
        public string CfgSection { get; set; }
        public string CfgSetting { get; set; }
        public Action<ExpandoObject> OnChange { get; set; }
        public SetOptionDelegate SetFunction { get; set; }

        public abstract TManagedValue GetValue();
        public abstract void SetValue(TManagedValue value);

        protected void NotifyOptionChanged(TManagedValue value)
        {
            dynamic message = new ExpandoObject();
            message.OptionKind = Kind;
            message.Value = value;
            OnChange?.Invoke(message);
        }
    }
}