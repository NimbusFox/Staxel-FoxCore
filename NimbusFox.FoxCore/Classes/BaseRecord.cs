using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class BaseRecord {
        [JsonIgnore]
        private readonly BlobDatabase _database;
        [JsonIgnore]
        protected readonly Blob _blob;
        [JsonIgnore]
        private Timer _timer;
        [JsonProperty("type")]
        protected string _type { get; set; }

        public BaseRecord(BlobDatabase database, Blob blob) {
            _database = database;
            _blob = blob;
        }

        public void Save() {
            _type = GetType().DeclaringType?.FullName ?? GetType().FullName;
            _timer?.Stop();
            _timer?.Dispose();
            _timer = new Timer(5000) {AutoReset = false};
            _timer.Start();
            _timer.Elapsed += (sender, args) => {
                _database.NeedsStore();
            };
        }

        public void Load(Blob blob) {
            _blob.AssignFrom(blob);
            Save();
        }

        public Blob CopyBlob() {
            var tempBlob = BlobAllocator.Blob(true);

            tempBlob.AssignFrom(_blob);

            return tempBlob;
        }
    }
}
