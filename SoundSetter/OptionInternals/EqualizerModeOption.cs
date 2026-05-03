using Dalamud.Game.Config;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public class EqualizerModeOption(IPluginLog log) : Option<EqualizerMode.Enum>(log)
    {
        public override EqualizerMode.Enum GetValue()
        {
            return GameConfig.TryGet(ConfigOption, out uint value)
                ? (EqualizerMode.Enum)value
                : EqualizerMode.Enum.Standard;
        }

        public override void SetValue(EqualizerMode.Enum value)
        {
            var v = (uint)value;
            GameConfig.Set(ConfigOption, v);
            PersistToCfg(v.ToString());
        }
    }
}