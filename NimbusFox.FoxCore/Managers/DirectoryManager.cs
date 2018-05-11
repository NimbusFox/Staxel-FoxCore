﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime;
using Microsoft.Xna.Framework;
using Neo.IronLua;
using NimbusFox.FoxCore.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Items;
using Staxel.Tiles;
using Staxel.Voxel;

namespace NimbusFox.FoxCore.Managers {
    public class DirectoryManager {
        private static List<string> FilesInUse = new List<string>();
        private string _localContentLocation;
        private string _root;
        private DirectoryManager _parent { get; set; }
        internal Engine _jsEngine { get; set; }
        internal LuaGlobalPortable _luaEngine { get; set; }
        
        public string Folder { get; private set; }

        public DirectoryManager Parent => _parent ?? this;

        public DirectoryManager TopLevel {
            get {
                if (_parent != null) {
                    return _parent.TopLevel;
                }

                return this;
            }
        }

        internal string GetPath(char seperator) {
            return Regex.Replace(_localContentLocation.Replace(_root, ""), @"\/|\\", seperator.ToString()) + seperator;
        }

        internal DirectoryManager(string author, string mod) {
            var streamLocation = Path.Combine("Mods", author, mod);
            _localContentLocation = Path.Combine(GameContext.ContentLoader.LocalContentDirectory, streamLocation);
            _root = _localContentLocation;
            Folder = mod;

            if (!Directory.Exists(_localContentLocation)) {
                Directory.CreateDirectory(_localContentLocation);
            }
        }

        internal DirectoryManager(string mod) {
            _localContentLocation = Path.Combine(GameContext.ContentLoader.RootDirectory, "mods", mod);
            _root = GameContext.ContentLoader.RootDirectory;
            Folder = mod;
        }

        internal DirectoryManager() {
            _localContentLocation = GameContext.ContentLoader.RootDirectory;
            _root = _localContentLocation;
            Folder = "content";
        }

        public DirectoryManager FetchDirectory(string name) {
            var dir = new DirectoryManager {
                _localContentLocation = Path.Combine(_localContentLocation, name),
                _root = _root,
                _parent = this,
                Folder = name
            };

            CreateDirectory(name);

            return dir;
        }

        private void CreateDirectory(string name) {
            if (!DirectoryExists(name)) {
                Directory.CreateDirectory(Path.Combine(_localContentLocation, name));
            }
        }

        public IReadOnlyList<string> Directories => new DirectoryInfo(_localContentLocation).GetDirectories().Select(dir => dir.Name).ToList();

        public IReadOnlyList<string> Files => new DirectoryInfo(_localContentLocation).GetFiles().Select(file => file.Name).ToList();

        public bool FileExists(string name) {
            return Files.Contains(name);
        }

        public bool DirectoryExists(string name) {
            return Directories.Contains(name);
        }

        public static Blob SerializeObject<T>(T data) {
            if (data is Blob o) {
                return o;
            }
            var blob = BlobAllocator.AcquireAllocator().NewBlob(true);
            blob.ObjectToBlob(null, data);
            return blob;
        }

        public void WriteFileLua(string fileName, LuaTable data, Func<LuaTable> onFinish = null, bool outputAsText = false) {
            var dictionary = data.Cast<object>().ToDictionary(key => key, key => data[key]);

            WriteFile(fileName, dictionary, () => {
                onFinish?.Invoke();
            }, outputAsText);
        }

        public void ReadFileLua(string fileName, Func<LuaTable, LuaTable> onLoad, bool inputIsText) {
            ReadFile<Dictionary<object, object>>(fileName, data => {
                var output = new LuaTable();

                foreach (var item in data) {
                    output[item.Key] = item.Value;
                }

                onLoad(output);
            }, inputIsText);
        }

        public void WriteFileJS(string fileName, JsValue data, Action onFinish = null, bool outputAsText = false) {
            var blob = BlobAllocator.AcquireAllocator().NewBlob(false);

            JSToBlob(blob, data);

            WriteFile(fileName, blob, onFinish, outputAsText);
        }

        public void ReadFileJS(string fileName, Action<JsValue> onLoad, bool inputIsText) {
            var js = new JsValue(new ObjectInstance(_jsEngine));
            var wait = true;
            ReadFile<Blob>(fileName, blob => {
                BlobToJS(blob, js);
                wait = false;
            }, inputIsText);

            while (wait) { }
            onLoad.Invoke(js);
        }

        private void JSToBlob(Blob parent, JsValue data) {
            if (data.IsObject()) {
                foreach (var property in data.AsObject().GetOwnProperties()) {
                    switch (property.Value.Value.Type) {
                        case Types.Boolean:
                            parent.SetBool(property.Key, property.Value.Value.AsBoolean());
                            break;
                        case Types.Null:
                            parent.SetString(property.Key, "");
                            break;
                        case Types.Number:
                            parent.SetDouble(property.Key, property.Value.Value.AsNumber());
                            break;
                        case Types.String:
                            parent.SetString(property.Key, property.Value.Value.AsString());
                            break;
                        case Types.None:
                            break;
                        case Types.Object:
                            JSToBlob(parent.FetchBlob(property.Key), property.Value.Value);
                            break;
                    }
                }
            }
        }

        private void BlobToJS(Blob container, JsValue parent) {
            foreach (var key in container.KeyValueIteratable) {
                switch (key.Value.Kind) {
                    case BlobEntryKind.Bool:
                        parent.AsObject().FastAddProperty(key.Key, new JsValue(container.GetBool(key.Key)), true, false, false);
                        break;
                    case BlobEntryKind.Float:
                        parent.AsObject().FastAddProperty(key.Key, new JsValue(container.GetDouble(key.Key)), true, false, false);
                        break;
                    case BlobEntryKind.String:
                        if (container.GetString(key.Key).IsNullOrEmpty()) {
                            parent.AsObject().FastAddProperty(key.Key, JsValue.Null, true, false, false);
                            break;
                        }
                        parent.AsObject().FastAddProperty(key.Key, new JsValue(container.GetString(key.Key)), true, false, false);
                        break;
                    case BlobEntryKind.Blob:
                        parent.AsObject().FastAddProperty(key.Key, new JsValue(new ObjectInstance(_jsEngine)), true, false, false);
                        BlobToJS(container.FetchBlob(key.Key), parent.AsObject().Get(key.Key));
                        break;
                }
            }
        }

        public void WriteFile<T>(string fileName, T data, Action onFinish = null, bool outputAsText = false) {
            new Thread(() => {
                var target = Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName);
                var collection = new List<string>();
                collection.AddAll(FilesInUse);
                while (collection.Any(x => x == target)) { 
                    collection.Clear();
                    collection.AddAll(FilesInUse);
                }

                FilesInUse.Add(target);

                var stream = new MemoryStream();
                var output = SerializeObject(data);
                stream.Seek(0L, SeekOrigin.Begin);
                if (!outputAsText) {
                    stream.WriteBlob(output);
                } else {
                    output.SaveJsonStream(stream);
                }
                stream.Seek(0L, SeekOrigin.Begin);
                File.WriteAllBytes(Path.Combine(_localContentLocation, fileName), stream.ReadAllBytes());
                onFinish?.Invoke();

                FilesInUse.Remove(target);
            }).Start();
        }

        public void WriteFileStream(string fileName, Stream stream, Action onWrite = null) {
            new Thread(() => {
                var target = Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName);
                var collection = new List<string>();
                collection.AddAll(FilesInUse);
                while (collection.Any(x => x == target)) {
                    collection.Clear();
                    collection.AddAll(FilesInUse);
                }

                FilesInUse.Add(target);

                stream.Seek(0L, SeekOrigin.Begin);
                File.WriteAllBytes(Path.Combine(_localContentLocation, fileName), stream.ReadAllBytes());
                onWrite?.Invoke();

                FilesInUse.Remove(target);
            }).Start();
        }

        public void ReadFile<T>(string fileName, Action<T> onLoad, bool inputIsText = false) {
            new Thread(() => {
                var target = Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName);
                var collection = new List<string>();
                collection.AddAll(FilesInUse);
                while (collection.Any(x => x == target)) {
                    collection.Clear();
                    collection.AddAll(FilesInUse);
                }

                if (FileExists(fileName)) {
                    var stream =
                        GameContext.ContentLoader.ReadStream(Path.Combine(GetPath('/'), fileName));
                    Blob input;
                    if (!inputIsText) {
                        input = stream.ReadBlob();
                    } else {
                        if (typeof(T) == typeof(string)) {
                            onLoad((T)(object)stream.ReadAllText());
                            return;
                        }
                        input = BlobAllocator.AcquireAllocator().NewBlob(false);
                        var sr = new StreamReader(stream);
                        input.ReadJson(sr.ReadToEnd());
                    }

                    stream.Seek(0L, SeekOrigin.Begin);
                    if (typeof(T) == input.GetType()) {
                        onLoad((T)(object)input);
                        return;
                    }

                    onLoad(input.BlobToObject(null, (T)Activator.CreateInstance(typeof(T))));
                    return;
                }

                onLoad((T)Activator.CreateInstance(typeof(T)));
            }).Start();
        }

        public void ReadFileStream(string fileName, Action<Stream> onLoad, bool required = false) {
            new Thread(() => {
                var target = Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName);
                var collection = new List<string>();
                collection.AddAll(FilesInUse);
                while (collection.Any(x => x == target)) {
                    collection.Clear();
                    collection.AddAll(FilesInUse);
                }

                var stream =
                    GameContext.ContentLoader.ReadStream(Path.Combine(GetPath('/'), fileName));
                stream.Seek(0L, SeekOrigin.Begin);
                onLoad?.Invoke(stream);
            }).Start();
        }

        public void DeleteFile(string name) {
            if (FileExists(name)) {
                var target = Path.Combine(GetPath(Path.DirectorySeparatorChar), name);
                var collection = new List<string>();
                collection.AddAll(FilesInUse);
                while (collection.Any(x => x == target)) {
                    collection.Clear();
                    collection.AddAll(FilesInUse);
                }

                File.Delete(Path.Combine(_localContentLocation, name));
            }
        }

        public void DeleteDirectory(string name, bool recursive) {
            if (DirectoryExists(name)) {
                var target = Path.Combine(GetPath(Path.DirectorySeparatorChar), name);
                var collection = new List<string>();
                collection.AddAll(FilesInUse);
                while (collection.Any(x => x == target)) {
                    collection.Clear();
                    collection.AddAll(FilesInUse);
                }

                Directory.Delete(Path.Combine(_localContentLocation, name), recursive);
            }
        }

        public void ExportCube(Vector3I range, Vector3I baseVector, string name) {
            var data = new Dictionary<int, Color>();
            var colorSet = new HashSet<Color>
            {
                Color.Transparent
            };

            var dictionary = new Dictionary<CodeEntry, Color> {
                {
                    new CodeEntry(Constants.SkyCode + "+0", 0U),
                    Color.Transparent
                }, {
                    new CodeEntry(Constants.CompoundCode + "+0", 0U),
                    Color.Transparent
                }, {
                    new CodeEntry(Constants.CompoundCollisionCode + "+0", 0U),
                    Color.Transparent
                }
            };

            var fileData = new VectorFileData { Size = range };

            fileData.Mappings.Add("00000000", new VectorData {
                Code = Constants.SkyCode,
                Rotation = 0
            });

            fileData.Mappings.Add("FF000000", new VectorData {
                Code = Constants.SkyCode,
                Rotation = 0
            });

            var randomSource = GameContext.RandomSource;

            Fox_Core.VectorLoop(new Vector3I(0, 0, 0), range, (x, y, z) => {
                var index = x + z * range.X + y * range.X * range.Z;
                Tile tile;
                if (!CoreHook.Universe.ReadTile(baseVector + new Vector3I(x, y, z),
                    TileAccessFlags.SynchronousWait, out tile)) {
                    throw new Exception("Failed to read tile at: " + (baseVector + new Vector3I(x, y, z)) +
                                        ". Tileflags: " + TileAccessFlags.SynchronousWait);
                }

                var rotation = tile.Configuration.Rotation(tile.Variant());
                if (tile.Configuration.CompoundFiller) {
                    rotation = 0U;
                }

                var key = new CodeEntry(tile.Configuration.Code + '+' + rotation, rotation);

                Color color;
                if (!dictionary.TryGetValue(key, out color)) {
                    color = tile.Configuration.ExportColor;
                    var loop = true;
                    while (loop) {
                        while (colorSet.Contains(color)) {
                            color = new Color(randomSource.Next(0, 256), randomSource.Next(0, 256), randomSource.Next(0, 256));
                        }
                        colorSet.Add(color);
                        dictionary.Add(key, color);

                        if (!fileData.Mappings.ContainsKey(ColorMath.ToString(color))) {
                            fileData.Mappings.Add(ColorMath.ToString(color), new VectorData {
                                Code = tile.Configuration.Code,
                                Rotation = rotation
                            });
                            loop = false;
                        }
                    }
                }

                if (!data.ContainsKey(index)) {
                    data.Add(index, color);
                }
            });

            WriteFile(name + ".json", fileData, null, true);

            WriteQBFile(range, name, data);
        }

        public VoxelOutput ImportCube(string name) {
            var output = new VoxelOutput();
            var check = false;
            ReadFile<VectorFileData>(name + ".json", json => {
                output.JsonData = json;
                output.Voxels = ReadQBFile(name, Vector3I.Zero, json.Size);
                check = true;
            }, true);

            while (!check) { }

            return output;
        }

        public VoxelObject ReadQBFile(string name, Vector3I offSet, Vector3I sizeLimit) {
            VoxelObject output = null;

            ReadFileStream(name + ".qb", data => {
                output = VoxelLoader.LoadQb(data, name + "qb", offSet, sizeLimit);
            }, true);

            while (output == null) { }

            return output;
        }

        public void WriteQBFile(Vector3I range, string name, Dictionary<int, Color> data) {

            var stream = new MemoryStream();

            var bw = new BinaryWriter(stream);

            bw.Write((byte)1);
            bw.Write((byte)1);
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write(0U);
            bw.Write(0U);
            bw.Write(1U);
            bw.Write(0U);
            bw.Write(1U);
            bw.Write("main");
            bw.Write(range.X);
            bw.Write(range.Y);
            bw.Write(range.Z);
            bw.Write(0);
            bw.Write(0);
            bw.Write(0);

            void Action(Color color, int run) {
                if (run <= 0)
                    return;
                var num = 1;
                if (run > 2) {
                    bw.Write(2U);
                    bw.Write((uint)run);
                } else
                    num = run;

                for (var index = 0; index < num; ++index)
                    bw.Write(color.PackedValue);
            }

            var num1 = 0;
            var color1 = Color.White;
            var num2 = 0;
            for (; num1 < range.Z; ++num1) {
                for (var index = 0; index < range.X * range.Y; ++index) {
                    var num3 = index % range.X;
                    var num4 = index / range.X;
                    var transparent = data[num3 + num1 * range.X + num4 * range.X * range.Z];
                    if (((int)transparent.PackedValue & -16777216) == 0)
                        transparent = Color.Transparent;
                    else
                        transparent.PackedValue |= 4278190080U;
                    if (transparent != color1) {
                        Action(color1, num2);
                        num2 = 0;
                    }
                    color1 = transparent;
                    ++num2;
                }
                Action(color1, num2);
                num2 = 0;
                bw.Write(6U);
            }

            bw.Flush();
            stream.Seek(0L, SeekOrigin.Begin);

            var wait = true;

            WriteFileStream(name + ".qb", stream, () => { wait = false; });

            while (wait) { }
        }

        public void WriteQBFile(string name, VoxelObject voxels) {
            var colors = new Dictionary<int, Color>();

            Fox_Core.VectorLoop(Vector3I.Zero, voxels.Size, (x, y, z) => {
                try {
                    var index = x + z * voxels.Size.X + y * voxels.Size.X * voxels.Size.Z;

                    var color = voxels.Read(x, y, z);
                    if (!colors.ContainsKey(index)) {
                        colors.Add(index, color);
                    } else {
                        colors[index] = color;
                    }
                } catch {

                }
            });

            WriteQBFile(voxels.Size, name, colors);
        }
    }
}
