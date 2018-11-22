using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json.Linq;
using Plukit.Base;

namespace NimbusFox.FoxCore.V3.Classes.FxBlob {
    [Serializable]
    public class FoxBlobEntry : IDisposable {
        public BlobKind Kind { get; private set; }
        private object _value;

        internal object GetValue() {
            return _value;
        }

        public void Dispose() {
            _value = null;
        }

        public void SetString(string value) {
            Kind = BlobKind.String;
            _value = value;
        }

        public void SetBool(bool value) {
            Kind = BlobKind.Boolean;
            _value = value;
        }

        public void SetNumber(long value) {
            Kind = BlobKind.Number;
            _value = value;
        }

        public void SetDouble(double value) {
            Kind = BlobKind.Decimal;
            _value = value;
        }

        public void SetList<T>(List<T> value) {
            Kind = BlobKind.List;
            _value = value.ToArray();
        }

        public void SetBlob(FoxBlob value) {
            Kind = BlobKind.Blob;
            _value = value;
        }

        public string GetString(string _default) {
            return GetString() ?? _default;
        }

        public string GetString() {
            return _value?.ToString();
        }

        private bool GetBool(out bool value) {
            return bool.TryParse(_value?.ToString() ?? "", out value);
        }

        public bool GetBool(bool _default) {
            Validate(BlobKind.Boolean);
            return GetBool(out var result) ? result : _default;
        }

        public bool GetBool() {
            Validate(BlobKind.Boolean);
            if (GetBool(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetLong(out long value) {
            return long.TryParse(_value?.ToString() ?? "", out value);
        }

        public long GetLong(long _default) {
            Validate(BlobKind.Number);
            return GetLong(out var result) ? result : _default;
        }

        public long GetLong() {
            Validate(BlobKind.Number);
            if (GetLong(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetUInt(out uint value) {
            return uint.TryParse(_value?.ToString() ?? "", out value);
        }

        public uint GetUInt(uint _default) {
            return GetUInt(out var result) ? result : _default;
        }

        public uint GetUInt() {
            Validate(BlobKind.Number);
            if (GetUInt(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetInt(out int value) {
            return int.TryParse(_value?.ToString() ?? "", out value);
        }

        public int GetInt(int _default) {
            Validate(BlobKind.Number);
            return GetInt(out var result) ? result : _default;
        }

        public int GetInt() {
            Validate(BlobKind.Number);
            if (GetInt(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetByte(out byte value) {
            return byte.TryParse(_value?.ToString() ?? "", out value);
        }

        public byte GetByte(byte _default) {
            Validate(BlobKind.Number);
            return GetByte(out var result) ? result : _default;
        }

        public byte GetByte() {
            Validate(BlobKind.Number);
            if (GetByte(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetDecimal(out decimal value) {
            return decimal.TryParse(_value?.ToString() ?? "", out value);
        }

        public decimal GetDecimal(decimal _default) {
            Validate(BlobKind.Decimal);
            return GetDecimal(out var result) ? result : _default;
        }

        public decimal GetDecimal() {
            Validate(BlobKind.Decimal);
            if (GetDecimal(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetFloat(out float value) {
            return float.TryParse(_value?.ToString() ?? "", out value);
        }

        public float GetFloat(float _default) {
            Validate(BlobKind.Decimal);
            return GetFloat(out var result) ? result : _default;
        }

        public float GetFloat() {
            Validate(BlobKind.Decimal);
            if (GetFloat(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private bool GetDouble(out double value) {
            return double.TryParse(_value?.ToString() ?? "", out value);
        }

        public double GetDouble(double _default) {
            Validate(BlobKind.Decimal);
            return GetDouble(out var result) ? result : _default;
        }

        public double GetDouble() {
            Validate(BlobKind.Decimal);
            if (GetDouble(out var result)) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        public T[] GetList<T>(T[] _default) {
            Validate(BlobKind.List);
            try {
                return GetList<T>();
            } catch {
                return _default;
            }
        }

        public T[] GetList<T>() {
            Validate(BlobKind.List);
            if (_value is T[] result) {
                return result;
            }

            if (_value is object[] objResult) {
                return objResult.Cast<JValue>().Select(x => (T)x.Value).ToArray();
            }

            throw new BlobValueException("Blob contained an invalid value.");
        }

        public FoxBlob GetBlob(FoxBlob _default) {
            Validate(BlobKind.Blob);
            if (_value is FoxBlob result) {
                return result;
            }

            return _default;
        }

        public FoxBlob GetBlob() {
            Validate(BlobKind.Blob);
            if (_value is FoxBlob result) {
                return result;
            }
            throw new BlobValueException("Blob contained an invalid value.");
        }

        private void Validate(BlobKind expected) {
            if (expected != Kind) {
                throw new BlobKindException(
                    $"Unable to fetch value. Was expecting kind '{Enum.GetName(expected.GetType(), expected)}' but got '{Enum.GetName(Kind.GetType(), Kind)}'");
            }
        }
    }
}
