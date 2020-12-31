using System;
using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    public class ByteOption : Option<byte>
    {
        public override byte GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset);
        }

        public override void SetValue(byte value)
        {
            SetFunction(BaseAddress, Kind, value);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = LoadConfig();
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }

        public static Func<OptionKind, int, string, ByteOption> CreateFactory(IntPtr baseAddress, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new ByteOption
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