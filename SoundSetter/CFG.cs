using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dalamud.Logging;

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
            foreach (var (sectionName, settings) in Settings)
            {
                text.AppendLine();
                text.Append('<').Append(sectionName).AppendLine(">");
                foreach (var (key, value) in settings)
                {
                    text.Append(key).Append('\t').AppendLine(value);
                }
            }
            text.Append(' ');
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
                    currentSection = line[1..^1];
                    if (!cfg.ContainsKey(currentSection))
                    {
                        cfg.Add(currentSection, new Dictionary<string, string>());
                    }
                }
                else if (line.IndexOf('\t') != -1 && currentSection != null)
                {
                    var kvp = line.Split('\t');
                    var key = kvp[0];
                    var value = kvp.Length > 1 ? kvp[1] : "";
                    if (!cfg[currentSection].ContainsKey(key))
                    {
                        cfg[currentSection].Add(key, value);
                    }
                }
            }
            return cfg;
        }

        public static CFG Load()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "my games",
                "FINAL FANTASY XIV - A Realm Reborn",
                "FFXIV.cfg");

            try
            {
                return new CFG(path);
            }
            catch (Exception e)
            {
                PluginLog.LogError(e, "Failed to load configuration object.");
                return null;
            }
        }
    }
}