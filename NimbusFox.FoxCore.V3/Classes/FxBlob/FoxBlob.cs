using System;
using System.Collections.Generic;
using System.Linq;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json.Linq;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Classes.FxBlob {
    [Serializable]
    public class FoxBlob : IDisposable {
        public IReadOnlyDictionary<string, FoxBlobEntry> Entries => _entries;
        private Dictionary<string, FoxBlobEntry> _entries { get; } = new Dictionary<string, FoxBlobEntry>();

        public void Dispose() {
            foreach (var entry in _entries) {
                if (entry.Value.Kind == BlobKind.Blob) {
                    entry.Value.GetBlob().Dispose();
                }
            }

            _entries.Clear();
        }

        private void ValidateKey(string key) {
            if (!_entries.ContainsKey(key)) {
                _entries.Add(key, new FoxBlobEntry());
            }
        }

        public void SetString(string key, string value) {
            ValidateKey(key);
            _entries[key].SetString(value);
        }

        public void SetBool(string key, bool value) {
            ValidateKey(key);
            _entries[key].SetBool(value);
        }

        public void SetNumber(string key, long value) {
            ValidateKey(key);
            _entries[key].SetNumber(value);
        }

        public void SetDouble(string key, double value) {
            ValidateKey(key);
            _entries[key].SetDouble(value);
        }

        public void SetList<T>(string key, List<T> value) {
            ValidateKey(key);
            _entries[key].SetList(value);
        }

        public void SetBlob(string key, FoxBlob value) {
            ValidateKey(key);
            if (_entries[key].GetValue() is FoxBlob blob) {
                blob.Dispose();
            }
            _entries[key].SetBlob(value);
        }

        public void SetGuid(string key, Guid value) {
            ValidateKey(key);
            _entries[key].SetString(value.ToString());
        }

        public FoxBlob FetchBlob(string key) {
            ValidateKey(key);
            if (_entries[key].GetValue() == null) {
                _entries[key].SetBlob(new FoxBlob());
            }
            return _entries[key].GetBlob();
        }

        public string GetString(string key) {
            return _entries[key].GetString();
        }

        public string GetString(string key, string _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetString(_default);
        }

        public bool GetBool(string key) {
            return _entries[key].GetBool();
        }

        public bool GetBool(string key, bool _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetBool(_default);
        }

        public long GetLong(string key) {
            return _entries[key].GetLong();
        }

        public long GetLong(string key, long _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetLong(_default);
        }

        public uint GetUInt(string key) {
            return _entries[key].GetUInt();
        }

        public uint GetUInt(string key, uint _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetUInt(_default);
        }

        public int GetInt(string key) {
            return _entries[key].GetInt();
        }

        public int GetInt(string key, int _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetInt(_default);
        }

        public byte GetByte(string key) {
            return _entries[key].GetByte();
        }

        public byte GetByte(string key, byte _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetByte(_default);
        }

        public decimal GetDecimal(string key) {
            return _entries[key].GetDecimal();
        }

        public decimal GetDecimal(string key, decimal _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetDecimal(_default);
        }

        public float GetFloat(string key) {
            return _entries[key].GetFloat();
        }

        public float GetFloat(string key, float _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetFloat(_default);
        }

        public double GetDouble(string key) {
            return _entries[key].GetDouble();
        }

        public double GetDouble(string key, double _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetDouble(_default);
        }

        public IEnumerable<T> GetList<T>(string key) {
            return _entries[key].GetList<T>();
        }

        public IEnumerable<T> GetList<T>(string key, T[] _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetList<T>(_default);
        }

        public FoxBlob GetBlob(string key) {
            return _entries[key].GetBlob();
        }

        public FoxBlob GetBlob(string key, FoxBlob _default) {
            return !_entries.ContainsKey(key) ? _default : _entries[key].GetBlob(_default);
        }

        public Guid GetGuid(string key) {
            return Guid.Parse(_entries[key].GetString());
        }

        public Guid GetGuid(string key, Guid _default) {
            return Guid.Parse(!_entries.ContainsKey(key) ? _default.ToString()
                : _entries[key].GetString(_default.ToString()));
        }

        public override string ToString() {
            var data = new Dictionary<string, object>();
            GetJsonData(this, data);
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public void Delete(string key) {
            if (_entries.ContainsKey(key)) {
                var entry = _entries[key];
                if (entry.Kind == BlobKind.Blob) {
                    entry.GetBlob().Dispose();
                }

                _entries.Remove(key);
            }
        }

        private static void GetJsonData(FoxBlob level, IDictionary<string, object> current) {
            foreach (var entry in level.Entries.OrderBy(x => x.Key)) {
                if (entry.Value.Kind != BlobKind.Blob) {
                    current.Add(entry.Key, entry.Value.GetValue());
                } else {
                    var data = new Dictionary<string, object>();
                    GetJsonData(entry.Value.GetBlob(), data);
                    current.Add(entry.Key, data);
                }
            }
        }

        public void MergeFrom(Blob blob) {
            ReadJson(blob.ToString());
        }

        public void MergeFrom(FoxBlob blob) {
            ReadJson(blob.ToString());
        }

        public bool Contains(string key) {
            return _entries.ContainsKey(key);
        }

        public FoxBlob Clone() {
            var clone = new FoxBlob();

            clone.ReadJson(ToString());

            return clone;
        }

        public static FoxBlob FromJson(string json) {
            var blob = new FoxBlob();

            blob.ReadJson(json);

            return blob;
        }

        public void ReadJson(string json) {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            Parse(data, this);
        }

        private void Parse(object data, FoxBlob level) {
            if (data is Dictionary<string, object> == false) {
                return;
            }

            foreach (var entry in (Dictionary<string, object>)data) {
                if (entry.Value is JObject obj) {
                    Parse(obj.ToObject<Dictionary<string, object>>(), level.FetchBlob(entry.Key));
                    continue;
                }

                if (entry.Value is JArray list) {
                    var pList = list.Cast<object>().ToList();
                    if (level.Contains(entry.Key)) {
                        var en = level.GetList<object>(entry.Key);
                        pList.AddRange(en);
                        level.SetList(entry.Key, pList);
                    } else {
                        level.SetList(entry.Key, pList);
                    }
                    pList.Clear();
                    continue;
                }

                if (entry.Value == null) {
                    level.SetString(entry.Key, null);
                    continue;
                }

                if (entry.Value is string str) {
                    level.SetString(entry.Key, str);
                    continue;
                }

                if (entry.Value is long lon) {
                    level.SetNumber(entry.Key, lon);
                    continue;
                }

                if (entry.Value is bool bol) {
                    level.SetBool(entry.Key, bol);
                    continue;
                }

                if (entry.Value is double dou) {
                    level.SetDouble(entry.Key, dou);
                    continue;
                }
            }
        }

        public FoxBlob FromObject(string key, object obj) {
            var blob = FetchBlob(key);
            blob.FromObject(obj);
            return blob;
        }

        public void FromObject(object obj) {
            ReadJson(JsonConvert.SerializeObject(obj));
        }

        public T ToObject<T>(string key) {
            return JsonConvert.DeserializeObject<T>(FetchBlob(key).ToString());
        }

        public T ToObject<T>() {
            return JsonConvert.DeserializeObject<T>(ToString());
        }
    }
}
