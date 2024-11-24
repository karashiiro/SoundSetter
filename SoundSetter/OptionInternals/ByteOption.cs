using System;
using System.Dynamic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    public class ByteOption(IPluginLog log) : Option<byte>(log)
    {
        public override byte GetValue()
        {
            var optionValue = GetRawValue();
            return Convert.ToByte(optionValue.Value1);
        }

        public override unsafe void SetValue(byte value)
        {
            base.SetValue(value);

            SetFunction(ConfigModule, Kind, value, 2, 1, 1);
            NotifyOptionChanged(value);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }

        public static unsafe Func<OptionKind, int, string?, ByteOption> CreateFactory(IPluginLog log, nint baseAddress, Action<ExpandoObject>? onChange, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new ByteOption(log)
            {
                ConfigModule = (ConfigModule*)baseAddress,
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