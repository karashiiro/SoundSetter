using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace SoundSetter
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }

        public List<VirtualKey.Enum> Keybind { get; }

        public Configuration()
        {
            Keybind = new List<VirtualKey.Enum> { VirtualKey.Enum.VkControl, VirtualKey.Enum.VkK };
        }

        [JsonIgnore] private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}