using System;
using System.Dynamic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    public class ByteOption(IPluginLog log) : Option<byte>(log)
    {
        public override unsafe byte GetValue()
        {
            var configModule = ConfigModule.Instance();
            var configEnum = OptionKind.GetConfigEnum(Kind);
            ref var optionValue1 = ref configModule->Values[(int)configEnum];
            ref var optionValue2 = ref OptionValue.FromOptionValue(ref optionValue1);
            return Convert.ToByte(optionValue2.Value1);
        }

        public override unsafe void SetValue(byte value)
        {
            SetFunction(ConfigModule.Instance(), Kind, value, 2, 1, 1);
            NotifyOptionChanged(value);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }

        public static Func<OptionKind.UIEnum, int, string?, ByteOption> CreateFactory(IPluginLog log, Action<ExpandoObject>? onChange, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new ByteOption(log)
            {
                Offset = offset,
                Kind = optionKind,
                
                CfgSection = cfgSection,
                CfgSetting = cfgSetting,
                
                OnChange = onChange,
                SetFunction = setFunction,
            };
        }
    }
}