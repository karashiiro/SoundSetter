using System;
using System.Dynamic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    public abstract class Option<TManagedValue>(IPluginLog log)
        where TManagedValue : struct
    {
        public OptionKind Kind { get; init; }
        public unsafe ConfigModule* ConfigModule { get; init; }
        public int Offset { get; init; }
        public required string CfgSection { get; init; }
        public string? CfgSetting { get; init; }
        public Action<ExpandoObject>? OnChange { get; init; }
        public required SetOptionDelegate SetFunction { get; init; }

        protected IPluginLog Log { get; } = log;

        protected unsafe OptionValue GetRawValue()
        {
            var configEnum = ConfigOptionKind.GetConfigEnum(Kind);
            ref var optionValue = ref ConfigModule->Values[(int)configEnum];
            return OptionValue.FromOptionValue(ref optionValue);
        }

        public abstract TManagedValue GetValue();

        public virtual unsafe void SetValue(TManagedValue value)
        {
            var configEnum = ConfigOptionKind.GetConfigEnum(Kind);
            ref var optionValue1 = ref ConfigModule->Values[(int)configEnum];
            ref var optionValue2 = ref OptionValue.FromOptionValue(ref optionValue1);
            optionValue2.Value1 = Convert.ToUInt64(value);
        }

        protected void NotifyOptionChanged(TManagedValue value)
        {
            dynamic message = new ExpandoObject();
            message.OptionKind = Kind;
            message.Value = value;
            OnChange?.Invoke(message);
        }
    }
}