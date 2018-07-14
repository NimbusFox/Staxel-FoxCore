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
    public abstract class BaseRecord {
        [JsonIgnore]
        private readonly BlobDatabase _database;
        [JsonIgnore]
        private readonly Blob _blob;
        [JsonIgnore]
        private Timer _timer;

        public BaseRecord(BlobDatabase database, Blob blob) {
            _database = database;
            _blob = blob;
        }

        public void Save(object target) {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = new Timer(5000) {AutoReset = false};
            _timer.Start();
            _timer.Elapsed += (sender, args) => {
                _blob.SetObject(target);
                _database.NeedsStore();
            };
        }

        public abstract void Load(Blob blob);

        public Blob CopyBlob() {
            var tempBlob = BlobAllocator.Blob(true);

            tempBlob.AssignFrom(_blob);

            return tempBlob;
        }
    }
}
