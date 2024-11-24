using System;
using System.Dynamic;
using System.Runtime.InteropServices;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    public class BooleanOption(IPluginLog log) : Option<bool>(log)
    {
        public bool Hack { get; set; }

        public override bool GetValue()
        {
            var optionValue = GetRawValue();
            return Convert.ToByte(optionValue.Value1) != 0;
        }

        public override unsafe void SetValue(bool value)
        {
            base.SetValue(value);

            var toWrite = value ? 1U : 0U;
            SetFunction(ConfigModule, Kind, toWrite, 2, 1, 1);
            NotifyOptionChanged(value);

            // This is a hack to make the native text commands work as expected; do not reuse this
            // or expect it to work elsewhere.
            // TODO: Determine if this is still necessary (it'll break when offsets change again)
            if (Hack) Marshal.WriteInt32((nint)ConfigModule, Offset - 21504, (int)toWrite);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = toWrite.ToString();
            cfg.Save();
        }

        public static unsafe Func<OptionKind, int, string?, BooleanOption> CreateFactory(IPluginLog log, nint baseAddress, Action<ExpandoObject>? onChange, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new BooleanOption(log)
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