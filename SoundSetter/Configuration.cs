using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace SoundSetter
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }

        public VirtualKey.Enum[] Keybind { get; }

        public Configuration()
        {
            Keybind = new[] { VirtualKey.Enum.VkControl, VirtualKey.Enum.VkK };
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