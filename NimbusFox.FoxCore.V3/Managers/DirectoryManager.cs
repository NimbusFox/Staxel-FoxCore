using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Items;
using Staxel.Tiles;
using Staxel.Voxel;

namespace NimbusFox.FoxCore.V3.Managers {
    public class DirectoryManager {
        private static BinaryFormatter _bf = new BinaryFormatter();
        private string _localContentLocation;
        private string _root;
        internal bool ContentFolder = false;
        private DirectoryManager _parent { get; set; }

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

        public string GetPath(char seperator) {
            return Regex.Replace(_localContentLocation, @"\/|\\", seperator.ToString()) + seperator;
        }

        internal DirectoryManager(string author, string mod) {
            var streamLocation = Path.Combine("Mods", author, mod);
            _localContentLocation = Path.Combine(GameContext.ContentLoader.LocalContentDirectory, streamLocation);
            _root = "~";
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
            _localContentLocation = new DirectoryInfo(GameContext.ContentLoader.RootDirectory).Parent.FullName;
            _root = _localContentLocation;
            Folder = new DirectoryInfo(GameContext.ContentLoader.RootDirectory).Parent.Name;
        }

        public DirectoryManager FetchDirectory(string name) {
            var dir = new DirectoryManager {
                _localContentLocation = Path.Combine(_localContentLocation, name),
                _root = _root,
                _parent = this,
                Folder = name,
                ContentFolder = ContentFolder
            };

            CreateDirectory(name);

            return dir;
        }

        internal DirectoryManager FetchDirectoryNoParent(string name) {
            var dir = new DirectoryManager {
                _localContentLocation = Path.Combine(_localContentLocation, name),
                _root = _root,
                _parent = null,
                Folder = name,
                ContentFolder = ContentFolder
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
            blob.ReadJson(JsonConvert.SerializeObject(data));
            return blob;
        }

        public void WriteFile<T>(string fileName, T data, bool outputAsText = false, bool wait = false) {
            var fwait = true;

            WriteFile(fileName, data, () => { fwait = false; }, outputAsText);

            while (wait && fwait) { }
        }

        public void WriteFile<T>(string fileName, T data, Action onFinish = null, bool outputAsText = false) {
            if (ContentFolder) {
                throw new IOException("Unable to edit files in the content folder");
            }
            new Thread(() => {
                var stream = new MemoryStream();
                if (data is string text) {
                    stream.WriteString(text);
                    stream.Seek(0L, SeekOrigin.Begin);
                    File.WriteAllBytes(Path.Combine(_localContentLocation, fileName), stream.ReadAllBytes());
                    onFinish?.Invoke();
                    return;
                }
                var output = SerializeObject(data);
                stream.Seek(0L, SeekOrigin.Begin);
                if (!outputAsText) {
                    output.Write(stream);
                } else {
                    output.SaveJsonStream(stream);
                }
                stream.Seek(0L, SeekOrigin.Begin);
                File.WriteAllBytes(Path.Combine(_localContentLocation, fileName), stream.ReadAllBytes());
                onFinish?.Invoke();
            }).Start();
        }

        public void WriteFileStream(string fileName, Stream stream, bool wait = false) {
            var fwait = true;

            WriteFileStream(fileName, stream, () => { fwait = false; });

            while (wait && fwait) { }
        }

        public void WriteFileStream(string fileName, Stream stream, Action onWrite = null) {
            if (ContentFolder) {
                throw new IOException("Unable to edit files in the content folder");
            }
            new Thread(() => {
                stream.Seek(0L, SeekOrigin.Begin);
                File.WriteAllBytes(Path.Combine(_localContentLocation, fileName), stream.ReadAllBytes());
                onWrite?.Invoke();
            }).Start();
        }

        public T ReadFile<T>(string fileName, bool inputIsText = false) {
            T data = default;

            var wait = true;

            ReadFile<T>(fileName, (fileData) => {
                data = fileData;
                wait = false;
            }, inputIsText);

            while (wait) { }

            return data;
        }

        public void ReadFile<T>(string fileName, Action<T> onLoad, bool inputIsText = false) {
            new Thread(() => {
                if (FileExists(fileName)) {
                    var data = File.ReadAllBytes(Path.Combine(_localContentLocation, fileName));
                    var stream = new MemoryStream(data, 0, data.Length, false, true);
                    stream.Seek(0, SeekOrigin.Begin);
                    Blob input;
                    if (!inputIsText) {
                        input = BlobAllocator.Blob(false);
                        input.Read(stream);
                    } else {
                        if (typeof(T) == typeof(string)) {
                            onLoad((T)(object)stream.ReadAllText());
                            return;
                        }
                        input = BlobAllocator.AcquireAllocator().NewBlob(false);
                        input.LoadJsonStream(stream);
                    }

                    stream.Close();
                    stream.Dispose();

                    if (typeof(T) == input.GetType()) {
                        onLoad((T)(object)input);
                        return;
                    }

                    using (var json = new MemoryStream()) {
                        input.SaveJsonStream(json);

                        json.Seek(0L, SeekOrigin.Begin);

                        onLoad(JsonConvert.DeserializeObject<T>(json.ReadAllText()));

                        Blob.Deallocate(ref input);
                    }

                    return;
                }

                onLoad(default);
            }).Start();
        }

        public Stream ReadFileStream(string fileName) {
            Stream output = null;
            var wait = true;

            ReadFileStream(fileName, (stream) => {
                output = stream;
                wait = false;
            });

            while (wait) { }

            return output;
        }

        public void ReadFileStream(string fileName, Action<Stream> onLoad) {
            new Thread(() => {
                var stream = new MemoryStream(File.ReadAllBytes(Path.Combine(_localContentLocation, fileName)));
                stream.Seek(0L, SeekOrigin.Begin);
                onLoad?.Invoke(stream);
            }).Start();
        }

        public FileStream ObtainFileStream(string fileName, FileMode mode) {
            return new FileStream(Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName), mode);
        }

        public FileStream ObtainFileStream(string fileName, FileMode mode, FileAccess access) {
            return new FileStream(Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName), mode, access);
        }

        public FileStream ObtainFileStream(string fileName, FileMode mode, FileAccess access, FileShare share) {
            return new FileStream(Path.Combine(GetPath(Path.DirectorySeparatorChar), fileName), mode, access, share);
        }

        public void DeleteFile(string name) {
            if (ContentFolder) {
                throw new IOException("Unable to edit files in the content folder");
            }
            if (FileExists(name)) {
                File.Delete(Path.Combine(_localContentLocation, name));
            }
        }

        public void DeleteDirectory(string name, bool recursive) {
            if (ContentFolder) {
                throw new IOException("Unable to edit files in the content folder");
            }
            if (DirectoryExists(name)) {
                Directory.Delete(Path.Combine(_localContentLocation, name), recursive);
            }
        }

        //public void ExportCube(Vector3I range, Vector3I baseVector, string name) {
        //    if (ContentFolder) {
        //        throw new IOException("Unable to edit files in the content folder");
        //    }
        //    var data = new Dictionary<int, Color>();
        //    var colorSet = new HashSet<Color>
        //    {
        //        Color.Transparent
        //    };

        //    var dictionary = new Dictionary<CodeEntry, Color> {
        //        {
        //            new CodeEntry(Constants.SkyCode + "+0", 0U),
        //            Color.Transparent
        //        }, {
        //            new CodeEntry(Constants.CompoundCode + "+0", 0U),
        //            Color.Transparent
        //        }, {
        //            new CodeEntry(Constants.CompoundCollisionCode + "+0", 0U),
        //            Color.Transparent
        //        }
        //    };

        //    var fileData = new VectorFileData { Size = range };

        //    fileData.Mappings.Add("00000000", new VectorData {
        //        Code = Constants.SkyCode,
        //        Rotation = 0
        //    });

        //    fileData.Mappings.Add("FF000000", new VectorData {
        //        Code = Constants.SkyCode,
        //        Rotation = 0
        //    });

        //    var randomSource = GameContext.RandomSource;

        //    Helpers.VectorLoop(new Vector3I(0, 0, 0), range, (x, y, z) => {
        //        var index = x + z * range.X + y * range.X * range.Z;
        //        Tile tile;
        //        if (!CoreHook.Universe.ReadTile(baseVector + new Vector3I(x, y, z),
        //            TileAccessFlags.SynchronousWait, out tile)) {
        //            throw new Exception("Failed to read tile at: " + (baseVector + new Vector3I(x, y, z)) +
        //                                ". Tileflags: " + TileAccessFlags.SynchronousWait);
        //        }

        //        var rotation = tile.Configuration.Rotation(tile.Variant());
        //        if (tile.Configuration.CompoundFiller) {
        //            rotation = 0U;
        //        }

        //        var key = new CodeEntry(tile.Configuration.Code + '+' + rotation, rotation);

        //        Color color;
        //        if (!dictionary.TryGetValue(key, out color)) {
        //            color = tile.Configuration.ExportColor;
        //            var loop = true;
        //            while (loop) {
        //                while (colorSet.Contains(color)) {
        //                    color = new Color(randomSource.Next(0, 256), randomSource.Next(0, 256), randomSource.Next(0, 256));
        //                }
        //                colorSet.Add(color);
        //                dictionary.Add(key, color);

        //                if (!fileData.Mappings.ContainsKey(ColorMath.ToString(color))) {
        //                    fileData.Mappings.Add(ColorMath.ToString(color), new VectorData {
        //                        Code = tile.Configuration.Code,
        //                        Rotation = rotation
        //                    });
        //                    loop = false;
        //                }
        //            }
        //        }

        //        if (!data.ContainsKey(index)) {
        //            data.Add(index, color);
        //        }
        //    });

        //    WriteFile(name + ".json", fileData, null, true);

        //    WriteQBFile(range, name, data);
        //}

        //public VoxelOutput ImportCube(string name) {
        //    var output = new VoxelOutput();
        //    var check = false;
        //    ReadFile<VectorFileData>(name + ".json", json => {
        //        output.JsonData = json;
        //        output.Voxels = ReadQBFile(name, Vector3I.Zero, json.Size);
        //        check = true;
        //    }, true);

        //    while (!check) { }

        //    return output;
        //}

        //public VoxelObject ReadQBFile(string name, Vector3I offSet, Vector3I sizeLimit) {
        //    VoxelObject output = null;

        //    ReadFileStream(name + ".qb", data => {
        //        output = VoxelLoader.LoadQb((MemoryStream)data, name + "qb", offSet, sizeLimit);
        //    });

        //    while (output == null) { }

        //    return output;
        //}

        //public void WriteQBFile(Vector3I range, string name, Dictionary<int, Color> data) {
        //    if (ContentFolder) {
        //        throw new IOException("Unable to edit files in the content folder");
        //    }

        //    var stream = new MemoryStream();

        //    var bw = new BinaryWriter(stream);

        //    bw.Write((byte)1);
        //    bw.Write((byte)1);
        //    bw.Write((byte)0);
        //    bw.Write((byte)0);
        //    bw.Write(0U);
        //    bw.Write(0U);
        //    bw.Write(1U);
        //    bw.Write(0U);
        //    bw.Write(1U);
        //    bw.Write("main");
        //    bw.Write(range.X);
        //    bw.Write(range.Y);
        //    bw.Write(range.Z);
        //    bw.Write(0);
        //    bw.Write(0);
        //    bw.Write(0);

        //    void Action(Color color, int run) {
        //        if (run <= 0)
        //            return;
        //        var num = 1;
        //        if (run > 2) {
        //            bw.Write(2U);
        //            bw.Write((uint)run);
        //        } else
        //            num = run;

        //        for (var index = 0; index < num; ++index)
        //            bw.Write(color.PackedValue);
        //    }

        //    var num1 = 0;
        //    var color1 = Color.White;
        //    var num2 = 0;
        //    for (; num1 < range.Z; ++num1) {
        //        for (var index = 0; index < range.X * range.Y; ++index) {
        //            var num3 = index % range.X;
        //            var num4 = index / range.X;
        //            var transparent = data[num3 + num1 * range.X + num4 * range.X * range.Z];
        //            if (((int)transparent.PackedValue & -16777216) == 0)
        //                transparent = Color.Transparent;
        //            else
        //                transparent.PackedValue |= 4278190080U;
        //            if (transparent != color1) {
        //                Action(color1, num2);
        //                num2 = 0;
        //            }
        //            color1 = transparent;
        //            ++num2;
        //        }
        //        Action(color1, num2);
        //        num2 = 0;
        //        bw.Write(6U);
        //    }

        //    bw.Flush();
        //    stream.Seek(0L, SeekOrigin.Begin);

        //    var wait = true;

        //    WriteFileStream(name + ".qb", stream, () => { wait = false; });

        //    while (wait) { }
        //}

        //public void WriteQBFile(string name, VoxelObject voxels) {
        //    if (ContentFolder) {
        //        throw new IOException("Unable to edit files in the content folder");
        //    }
        //    var colors = new Dictionary<int, Color>();

        //    Helpers.VectorLoop(Vector3I.Zero, voxels.Size, (x, y, z) => {
        //        try {
        //            var index = x + z * voxels.Size.X + y * voxels.Size.X * voxels.Size.Z;

        //            var color = voxels.Read(x, y, z);
        //            if (!colors.ContainsKey(index)) {
        //                colors.Add(index, color);
        //            } else {
        //                colors[index] = color;
        //            }
        //        } catch {

        //        }
        //    });

        //    WriteQBFile(voxels.Size, name, colors);
        //}
    }
}
