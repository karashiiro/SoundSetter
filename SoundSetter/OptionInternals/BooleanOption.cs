using System;
using System.Dynamic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SoundSetter.OptionInternals
{
    public class BooleanOption(IPluginLog log) : Option<bool>(log)
    {
        public bool Hack { get; set; }

        public override unsafe bool GetValue()
        {
            var configModule = ConfigModule.Instance();
            var configEnum = OptionKind.GetConfigEnum(Kind);
            ref var optionValue1 = ref configModule->Values[(int)configEnum];
            ref var optionValue2 = ref OptionValue.FromOptionValue(ref optionValue1);
            return Convert.ToByte(optionValue2.Value1) != 0;
        }

        public override unsafe void SetValue(bool value)
        {
            var toWrite = value ? 1U : 0U;
            SetFunction(ConfigModule.Instance(), Kind, toWrite, 2, 1, 1);
            NotifyOptionChanged(value);

            // This is a hack to make the native text commands work as expected; do not reuse this
            // or expect it to work elsewhere.
            // if (Hack) Marshal.WriteInt32(BaseAddress, Offset - 21504, (int)toWrite);

            if (string.IsNullOrEmpty(CfgSetting)) return;
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            cfg.Settings[CfgSection][CfgSetting] = toWrite.ToString();
            cfg.Save();
        }

        public static Func<OptionKind.UIEnum, int, string?, BooleanOption> CreateFactory(IPluginLog log, Action<ExpandoObject>? onChange, string cfgSection, SetOptionDelegate setFunction)
        {
            return (optionKind, offset, cfgSetting) => new BooleanOption(log)
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