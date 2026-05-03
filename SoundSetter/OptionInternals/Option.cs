using Dalamud.Game.Config;
using Dalamud.Plugin.Services;

namespace SoundSetter.OptionInternals
{
    public abstract class Option<TManagedValue>(IPluginLog log)
        where TManagedValue : struct
    {
        public required IGameConfig GameConfig { get; init; }
        public required SystemConfigOption ConfigOption { get; init; }
        public required string CfgSection { get; init; }

        protected IPluginLog Log { get; } = log;

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract TManagedValue GetValue();
        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract void SetValue(TManagedValue value);

        protected void PersistToCfg(string serializedValue)
        {
            var cfg = CFG.Load(Log);
            if (cfg == null) return;
            if (!cfg.Settings.TryGetValue(CfgSection, out var section)) return;
            section[ConfigOption.ToString()] = serializedValue;
            cfg.Save();
        }
    }
}