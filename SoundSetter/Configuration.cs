using System;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace SoundSetter
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }

        [Obsolete]
        public VirtualKey.Enum[] Keybind { get; }

        public VirtualKey.Enum ModifierKey { get; set; }
        public VirtualKey.Enum MajorKey { get; set; }

        public Configuration()
        {
            ModifierKey = VirtualKey.Enum.VkControl;
            MajorKey = VirtualKey.Enum.VkK;
        }

        [JsonIgnore] private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            // v1.0.2 compat
            if (ModifierKey != default) return;
            ModifierKey = Keybind[0];
            MajorKey = Keybind[1];
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}