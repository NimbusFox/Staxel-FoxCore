﻿using System;
using System.Timers;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using NimbusFox.FoxCore.V3.Classes.FxBlob;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Classes.BlobRecord {
    public class BaseRecord {
        [JsonIgnore]
        private readonly BlobDatabase _database;
        [JsonIgnore]
        protected readonly FoxBlob _blob;
        [JsonIgnore]
        private Timer _timer;

        public Guid ID { get; }

        protected string _type {
            get => _blob.GetString("_type", "");
            set => _blob.SetString("_type", value);
        }

        public BaseRecord(BlobDatabase database, FoxBlob blob, Guid id) {
            _database = database;
            _blob = blob;
            ID = id;
        }

        public void Save() {
            _type = GetType().DeclaringType?.FullName ?? GetType().FullName;
            _database.NeedsStore();
            _database.Save();
        }

        public void Load(Blob blob) {
            _blob.MergeFrom(blob);
            Save();
        }

        public void Load(FoxBlob blob) {
            _blob.MergeFrom(blob);
            Save();
        }

        public FoxBlob CopyBlob() {
            return _blob.Clone();
        }
    }
}
