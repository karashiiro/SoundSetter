using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoundSetter
{
    public class CFG
    {
        private readonly string path;

        public IDictionary<string, IDictionary<string, string>> Settings { get; }

        public CFG(string path)
        {
            this.path = path;
            var file = File.ReadAllText(path);
            Settings = ParseCfg(file);
        }

        public void Save()
        {
            var text = new StringBuilder();
            foreach (var section in Settings)
            {
                var sectionName = section.Key;
                var settings = section.Value;

                text.AppendLine();
                text.Append("<").Append(sectionName).AppendLine(">");
                foreach (var setting in settings)
                {
                    text.Append(setting.Key).Append('\t').AppendLine(setting.Value);
                }
            }
            text.Append(" ");
            File.WriteAllText(this.path, text.ToString());
        }

        private static IDictionary<string, IDictionary<string, string>> ParseCfg(string text)
        {
            var cfg = new Dictionary<string, IDictionary<string, string>>();
            var lines = text.Split('\r', '\n');
            string currentSection = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("<"))
                {
                    currentSection = line.Substring(1, line.Length - 2);
                    cfg.Add(currentSection, new Dictionary<string, string>());
                }
                else if (line.IndexOf('\t') != -1 && currentSection != null)
                {
                    var kvp = line.Split('\t');
                    var key = kvp[0];
                    var value = kvp.Length > 1 ? kvp[1] : "";
                    cfg[currentSection].Add(key, value);
                }
            }
            return cfg;
        }
    }
}