using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public class ByteOption(IPluginLog log) : Option<byte>(log)
    {
        public override byte GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset);
        }

        public override void SetValue(byte value)
        {
            SetFunction(BaseAddress, Kind, value, 2, 1, 1);
            NotifyOptionChanged(value);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }

        public static Func<OptionKind, int, string?, ByteOption> CreateFactory(IPluginLog log, nint baseAddress, Action<ExpandoObject>? onChange, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new ByteOption(log)
            {
                BaseAddress = baseAddress,
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