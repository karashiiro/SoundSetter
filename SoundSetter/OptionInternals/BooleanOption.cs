using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    public class BooleanOption : Option<bool>
    {
        public bool Hack { get; set; } = true;

        public override bool GetValue()
        {
            return Marshal.ReadByte(BaseAddress, Offset) != 0;
        }

        public override void SetValue(bool value)
        {
            var toWrite = value ? 1U : 0U;
            SetFunction(BaseAddress, Kind, toWrite, 2, 1, 1);
            NotifyOptionChanged(value);

            // This is a hack to make the native text commands work as expected; do not reuse this
            // or expect it to work elsewhere.
            if (Hack) Marshal.WriteInt32(BaseAddress, Offset - 21504, (int)toWrite);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load();
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = toWrite.ToString();
            cfg.Save();
        }

        public static Func<OptionKind, int, string, BooleanOption> CreateFactory(IntPtr baseAddress, Action<ExpandoObject> onChange, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new BooleanOption
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