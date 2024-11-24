using System;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    public class EqualizerModeOption(IPluginLog log) : Option<EqualizerMode.Enum>(log)
    {
        public override unsafe EqualizerMode.Enum GetValue()
        {
            var configModule = ConfigModule.Instance();
            var configEnum = OptionKind.GetConfigEnum(Kind);
            ref var optionValue1 = ref configModule->Values[(int)configEnum];
            ref var optionValue2 = ref OptionValue.FromOptionValue(ref optionValue1);
            return (EqualizerMode.Enum)Convert.ToByte(optionValue2.Value1);
        }

        public override unsafe void SetValue(EqualizerMode.Enum value)
        {
            SetFunction(ConfigModule.Instance(), Kind, (byte)value, 2, 1, 1);
            NotifyOptionChanged(value);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }
    }
}