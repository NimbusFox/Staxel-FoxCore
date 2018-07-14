using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes.Exceptions;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public class BlobDatabase : IDisposable {
        private Blob _database;
        private bool _needsStore;
        private readonly FileStream _databaseFile;

        public void Dispose() {
            NeedsStore();
            Save();
            Blob.Deallocate(ref _database);
            _databaseFile.Dispose();
        }

        public BlobDatabase(FileStream stream, Action<string> errorLogger) {
            _databaseFile = stream;
            _database = BlobAllocator.Blob(true);

            _databaseFile.Seek(0L, SeekOrigin.Begin);

            if (_databaseFile.Length == 0) {
                return;
            }

            try {
                _database.LoadJsonStream(_databaseFile);
            } catch {
                Console.ForegroundColor = ConsoleColor.Red;
                errorLogger($"{stream.Name} was corrupt. Will reset the database now to recover");
                Console.ResetColor();
            }
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

            return (T)Activator.CreateInstance(typeof(T), this, _database.FetchBlob(guid.ToString()));
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

            return record;
        }

        public void NeedsStore() {
            _needsStore = true;
        }

        public List<T> SearchRecords<T>(Expression<Func<T, bool>> expression) where T : BaseRecord {
            var output = new List<T>();
            var func = expression.Compile();
            foreach (var key in _database.KeyValueIteratable.Keys) {
                var current = _database.GetBlob(key);

                if (current.Contains("type")) {
                    if (current.GetString("type") == typeof(T).FullName) {
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
                _database.SaveJsonStream(_databaseFile);
                _databaseFile.Flush(true);
                _needsStore = false;
            }
        }
    }
}
