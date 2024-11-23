using System;
using System.Dynamic;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public abstract class Option<TManagedValue>(IPluginLog log)
        where TManagedValue : struct
    {
        public OptionKind Kind { get; init; }
        public nint BaseAddress { get; init; }
        public int Offset { get; init; }
        public required string CfgSection { get; init; }
        public string? CfgSetting { get; init; }
        public Action<ExpandoObject>? OnChange { get; init; }
        public required SetOptionDelegate SetFunction { get; init; }

        protected IPluginLog Log { get; } = log;

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