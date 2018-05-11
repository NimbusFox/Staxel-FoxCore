using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plukit.Base;

namespace NimbusFox.FoxCore.Classes {
    public static class Extensions {

        public static void ObjectToBlob(this Blob blob, string key, object data) {
            var blobEntry = key == null ? blob : blob.FetchBlob(key);

            if (data is IList list) {

                if (list.Count > 0) {
                    for (var i = 0; i < list.Count; i++) {
                        if (!GetProcessProperty(blobEntry, i.ToString(), list[i])) {
                            blobEntry.ObjectToBlob(i.ToString(), list[i]);
                        }
                    }
                }

                return;
            }

            if (data is IDictionary dictionary) {
                var dic = dictionary.ToObjectDictionary();

                if (dictionary.Count > 0) {
                    for (var i = 0; i < dictionary.Count; i++) {
                        var blobEntry3 = blobEntry.FetchBlob(i.ToString());
                        if (!GetProcessProperty(blobEntry3, "key", dic.Keys.ToArray()[i])) {
                            blobEntry3.ObjectToBlob("key", dic.Keys.ToArray()[i]);
                        }

                        if (!GetProcessProperty(blobEntry3, "value", dic.Values.ToArray()[i])) {
                            blobEntry3.ObjectToBlob("value", dic.Values.ToArray()[i]);
                        }
                    }
                }

                return;
            }

            foreach (var property in data.GetType().GetProperties()) {
                var item = property.GetValue(data);

                if (GetProcessProperty(blobEntry, property.Name, item)) {
                    continue;
                }

                blobEntry.ObjectToBlob(property.Name, property.GetValue(data));
            }

            foreach (var field in data.GetType().GetFields()) {
                var item = field.GetValue(data);

                if (item.GetType() == data.GetType()) {
                    continue;
                }

                if (GetProcessProperty(blobEntry, field.Name, item)) {
                    continue;
                }

                blobEntry.ObjectToBlob(field.Name, field.GetValue(data));
            }
        }

        private static bool GetProcessProperty(Blob blob, string key, object value) {
            if (value == null) {
                blob.FetchBlob(key);
                return true;
            }

            if (value is string s) {
                blob.SetString(key, s);
                return true;
            }

            if (value is Guid guid) {
                blob.SetString(key, guid.ToString());
                return true;
            }

            if (value is bool b) {
                blob.SetBool(key, b);
                return true;
            }

            if (value is DateTime date) {
                blob.SetLong(key, date.Ticks);
                return true;
            }

            if (value.GetType().IsNumber()) {
                blob.SetDouble(key, Convert.ToDouble(value));
                return true;
            }

            return false;
        }

        public static T BlobToObject<T>(this Blob blob, string key, T defaultValue) {
            var value = defaultValue;

            Blob blobEntry;

            if (key == null) {
                blobEntry = blob;
            } else {
                if (blob.Contains(key)) {
                    blobEntry = blob.FetchBlob(key);
                    value = (T)Activator.CreateInstance(defaultValue.GetType(), true);
                } else {
                    return value;
                }
            }


            if (value is IList) {
                var list = (IList) value;
                var i = 0;
                while (blobEntry.Contains(i.ToString())) {
                    var currentValue = SetProcessProperty(blobEntry, i.ToString(),
                        value.GetType().GetGenericArguments().Single(), null);

                    if (currentValue == null) {
                        currentValue = blobEntry.BlobToObject(i.ToString(),
                            Activator.CreateInstance(value.GetType().GetGenericArguments().Single(), true));
                    }

                    list.Add(currentValue);
                    i++;
                }

                return value;
            }

            if (value is IDictionary) {
                var dictionary = (IDictionary) value;
                var i = 0;
                var keyType = dictionary.GetType().GetGenericArguments()[0];
                var valueType = dictionary.GetType().GetGenericArguments()[1];
                while (blobEntry.Contains(i.ToString())) {
                    object currentKey;
                    object currentValue;

                    var currentBlob = blobEntry.FetchBlob(i.ToString());

                    currentKey = SetProcessProperty(currentBlob, "key", keyType, null);
                    if (currentKey == null) {
                        currentKey = currentBlob.BlobToObject("key", Activator.CreateInstance(keyType, true));
                    }

                    currentValue = SetProcessProperty(currentBlob, "value", valueType, null);
                    if (currentValue == null) {
                        currentValue = currentBlob.BlobToObject("value", Activator.CreateInstance(valueType, true));
                    }

                    dictionary.Add(currentKey, currentValue);
                    i++;
                }

                return value;
            }

            foreach (var property in value.GetType().GetProperties()) {
                if (!blobEntry.Contains(property.Name)) {
                    continue;
                }

                var type = property.PropertyType;

                var setter = property.GetSetMethod(true);

                if (type == typeof(IDictionary) || type == typeof(IList)) {
                    setter.Invoke(value,
                        new [] {blobEntry.BlobToObject(property.Name, Activator.CreateInstance(type, true))});
                    continue;
                }

                var val = SetProcessProperty(blobEntry, property.Name, type, null);

                if (val == null && property.PropertyType != typeof(string)) {
                    val = blobEntry.BlobToObject(property.Name, Activator.CreateInstance(type, true));
                }

                setter.Invoke(value, new[] {val});
            }

            foreach (var field in value.GetType().GetFields()) {
                if (field.FieldType == defaultValue.GetType()) {
                    continue;
                }

                var type = field.FieldType;

                if (type.IsInterface) {
                    continue;
                }

                if (type == typeof(IDictionary) || type == typeof(IList)) {
                    field.SetValue(value, blobEntry.BlobToObject(field.Name, Activator.CreateInstance(type, true)));
                    continue;
                }

                var val = SetProcessProperty(blobEntry, field.Name, type, null);

                if (val == null && field.FieldType != typeof(string)) {
                    val = blobEntry.BlobToObject(field.Name, Activator.CreateInstance(type, true));
                }

                field.SetValue(value, val);
            }

            return value;
        }

        private static object SetProcessProperty(Blob blob, string key, Type type, object defaultValue) {
            if (!blob.Contains(key)) {
                return defaultValue;
            }

            if (type == typeof(string)) {
                return blob.GetString(key);
            }

            if (type == typeof(Guid)) {
                return Guid.Parse(blob.GetString(key));
            }

            if (type == typeof(bool)) {
                return blob.GetBool(key);
            }

            if (type == typeof(DateTime)) {
                return new DateTime(blob.GetLong(key));
            }

            if (type.IsNumber()) {
                return Convert.ChangeType(blob.GetDouble(key), type);
            }

            if (blob.KeyValueIteratable[key].Kind != BlobEntryKind.Blob) {
                try {
                    return blob.GetString(key);
                } catch {
                    return blob.GetBool(key);
                }
            }

            Blob blob2;

            if (blob.TryGetBlob(key, out blob2)) {
                if (!blob2.KeyValueIteratable.Any()) {
                    return null;
                }
            }

            return defaultValue;
        }

        public static bool IsNumber(this Type type) {
            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default: return false;
            }
        }

        private static Dictionary<object, object> ToObjectDictionary(this IDictionary dictionary) {
            var output = new Dictionary<object, object>();
            foreach (DictionaryEntry pair in dictionary) {
                output.Add(pair.Key, pair.Value);
            }

            return output;
        }
    }
}
