using System;
using System.IO;

namespace SoundSetter.OptionInternals
{
    public abstract class Option<TManagedValue> where TManagedValue : struct
    {
        public OptionKind Kind { get; set; }
        public IntPtr BaseAddress { get; set; }
        public int Offset { get; set; }
        public string CfgSection { get; set; }
        public string CfgSetting { get; set; }
        public SetOptionDelegate SetFunction { get; set; }

        public abstract TManagedValue GetValue();
        public abstract void SetValue(TManagedValue value);

        protected CFG LoadConfig()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "my games",
                "FINAL FANTASY XIV - A Realm Reborn",
                "FFXIV.cfg");
            return new CFG(path);
        }
    }
}