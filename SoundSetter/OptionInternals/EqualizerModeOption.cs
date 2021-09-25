﻿using System.Runtime.InteropServices;

namespace SoundSetter.OptionInternals
{
    public class EqualizerModeOption : Option<EqualizerMode.Enum>
    {
        public override EqualizerMode.Enum GetValue()
        {
            return (EqualizerMode.Enum)Marshal.ReadByte(BaseAddress, Offset);
        }

        public override void SetValue(EqualizerMode.Enum value)
        {
            SetFunction(BaseAddress, Kind, (byte)value, 2, 1, 1);
            NotifyOptionChanged(value);
            
            var cfg = LoadConfig();
            cfg.Settings[CfgSection][CfgSetting] = value.ToString();
            cfg.Save();
        }
    }
}