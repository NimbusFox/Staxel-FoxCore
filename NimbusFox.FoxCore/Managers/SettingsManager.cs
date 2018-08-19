using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;

namespace NimbusFox.FoxCore.Managers {
    public class SettingsManager : IDisposable {
        internal static readonly Dictionary<string, Blob> ModsSettings = new Dictionary<string, Blob>();
        internal static readonly List<string> UpdateList = new List<string>();

        private Blob _settings {
            get {
                if (!ModsSettings.ContainsKey(ID)) {
                    ModsSettings.Add(ID, BlobAllocator.Blob(true));
                }

                return ModsSettings[ID];
            }
        }
        public string ID { get; }

        public void Dispose() {
            ModsSettings.Remove(ID);
        }

        internal SettingsManager(string author, string mod, string version) {
            ID = $"{author}.{mod}.{version}";
        }

        public void Update(object values) {
            _settings.SetObject(values);

            if (!UpdateList.Contains(ID)) {
                UpdateList.Add(ID);
            }
        }

        public T ReadSettings<T>() where T : class {
            return _settings.GetObject<T>();
        }

        public Blob GetBlob() {
            return _settings;
        }

        internal static void UpdateSettings(string id, Blob blob) {
            if (!ModsSettings.ContainsKey(id)) {
                ModsSettings.Add(id, BlobAllocator.Blob(true));
            }

            ModsSettings[id].MergeFrom(blob);
        }

        public bool Exists() {
            return ModsSettings.ContainsKey(ID);
        }
    }
}
