using System;
using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    public class BooleanOption : Option<bool>
    {
        public override bool GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset) != 0;
        }

        public override void SetValue(bool value)
        {
            SetFunction(BaseAddress, Kind, value ? 1UL : 0UL);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = LoadConfig();
            cfg.Settings[CfgSection][CfgSetting] = value ? "1" : "0";
            cfg.Save();
        }

        public static Func<OptionKind, int, string, BooleanOption> CreateFactory(IntPtr baseAddress, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new BooleanOption
            {
                BaseAddress = baseAddress,
                Offset = offset,
                Kind = optionKind,
                CfgSection = cfgSection,
                CfgSetting = cfgSetting,
                SetFunction = setFunction,
            };
        }
    }
}