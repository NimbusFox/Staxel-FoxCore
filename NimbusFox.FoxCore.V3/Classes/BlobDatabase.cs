﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json.Bson;
using NimbusFox.FoxCore.V3.Classes.BlobRecord;
using NimbusFox.FoxCore.V3.Classes.FxBlob;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Classes {
    public class BlobDatabase : IDisposable {
        private FoxBlob _database;
        private bool _needsStore;
        private readonly FileStream _databaseFile;
        private BinaryFormatter _bf = new BinaryFormatter();

        public void Dispose() {
            NeedsStore();
            Save();
            _database.Dispose();
            _databaseFile.Dispose();
        }

        public BlobDatabase(FileStream stream, Action<string> errorLogger) {
            _databaseFile = stream;
            _database = new FoxBlob();

            _databaseFile.Seek(0L, SeekOrigin.Begin);

            try {
                _database = (FoxBlob)_bf.Deserialize(_databaseFile);

                _databaseFile.Seek(0, SeekOrigin.Begin);

                File.WriteAllBytes(_databaseFile.Name + ".bak", _databaseFile.ReadAllBytes());
            } catch {
                if (File.Exists(_databaseFile.Name + ".bak")) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    errorLogger($"{_databaseFile.Name} was corrupt. Will revert to backup file");
                    Console.ResetColor();

                    using (var ms = new MemoryStream(File.ReadAllBytes(_databaseFile.Name + ".bak"))) {
                        _database = (FoxBlob)_bf.Deserialize(ms);
                    }
                }
            }

            NeedsStore();
            Save();
        }

        public bool RecordExists(Guid guid) {
            return guid == Guid.Empty || _database.Contains(guid.ToString());
        }

        public T CreateRecord<T>(Guid guid) where T : BaseRecord {
            if (!typeof(BaseRecord).IsAssignableFrom(typeof(T))) {
                throw new BlobDatabaseRecordTypeException("The type given for T does not inherit BaseRecord");
            }

            if (RecordExists(guid)) {
                throw new BlobDatabaseRecordException("A record already exists with this guid");
            }
            NeedsStore();
            return (T)Activator.CreateInstance(typeof(T), this, _database.FetchBlob(guid.ToString()));
        }

        public T GetRecord<T>(Guid guid) where T : BaseRecord {
            if (!typeof(BaseRecord).IsAssignableFrom(typeof(T))) {
                throw new BlobDatabaseRecordTypeException("The type given for T does not inherit BaseRecord");
            }

            if (!RecordExists(guid)) {
                throw new BlobDatabaseRecordException("No record with this guid exists");
            }

            return (T)Activator.CreateInstance(typeof(T), this, _database.FetchBlob(guid.ToString()), guid);
        }

        public void RemoveRecord(Guid guid) {
            if (RecordExists(guid)) {
                _database.Delete(guid.ToString());
                NeedsStore();
            }
        }

        public T OverwriteRecord<T>(Guid guid, BaseRecord newRecord) where T : BaseRecord {
            if (!typeof(BaseRecord).IsAssignableFrom(typeof(T))) {
                throw new BlobDatabaseRecordTypeException("The type given for T does not inherit BaseRecord");
            }

            var record = RecordExists(guid) ? CreateRecord<T>(guid) : GetRecord<T>(guid);

            record.Load(newRecord.CopyBlob());

            NeedsStore();
            Save();

            return record;
        }

        public void NeedsStore() {
            _needsStore = true;
        }

        public List<T> SearchRecords<T>(Expression<Func<T, bool>> expression) where T : BaseRecord {
            var output = new List<T>();
            var func = expression.Compile();
            foreach (var key in _database.Entries.Keys) {
                var current = _database.GetBlob(key);

                if (current.Contains("_type")) {
                    if (current.GetString("_type") == typeof(T).FullName) {
                        var record = GetRecord<T>(Guid.Parse(key));

                        if (func(record)) {
                            output.Add(record);
                        }
                    }
                }
            }

            return output;
        }

        public void Save() {
            if (_needsStore) {
                _databaseFile.SetLength(0);
                _databaseFile.Position = 0;
                
                _bf.Serialize(_databaseFile, _database.ToString());

                _databaseFile.Flush(true);
                _needsStore = false;
            }
        }
    }
}
