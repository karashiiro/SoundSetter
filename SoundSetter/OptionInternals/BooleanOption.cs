using Dalamud.Game.Config;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public class BooleanOption(IPluginLog log) : Option<bool>(log)
    {
        public override bool GetValue()
        {
            return GameConfig.TryGet(ConfigOption, out uint value) && value != 0;
        }

        public override void SetValue(bool value)
        {
            var v = value ? 1u : 0u;
            GameConfig.Set(ConfigOption, v);
            PersistToCfg(v.ToString());
        }
    }
}