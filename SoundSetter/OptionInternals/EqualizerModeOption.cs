using System;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public class EqualizerModeOption(IPluginLog log) : Option<EqualizerMode.Enum>(log)
    {
        public override EqualizerMode.Enum GetValue()
        {
            var optionValue = GetRawValue();
            return (EqualizerMode.Enum)Convert.ToByte(optionValue.Value1);
        }

        public override unsafe void SetValue(EqualizerMode.Enum value)
        {
            base.SetValue(value);

            SetFunction(ConfigModule, Kind, (byte)value, 2, 1, 1);
            NotifyOptionChanged(value);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }
    }
}