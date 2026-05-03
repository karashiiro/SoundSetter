using Dalamud.Game.Config;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public class ByteOption(IPluginLog log) : Option<byte>(log)
    {
        public override byte GetValue()
        {
            return GameConfig.TryGet(ConfigOption, out uint value) ? (byte)value : (byte)0;
        }

        public override void SetValue(byte value)
        {
            GameConfig.Set(ConfigOption, (uint)value);
            PersistToCfg(value.ToString());
        }
    }
}