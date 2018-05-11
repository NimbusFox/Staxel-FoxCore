using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Plukit.Base;
using Staxel;
using Staxel.Tiles;
using Staxel.Voxel;
using Voxels;
using Color = Microsoft.Xna.Framework.Color;

namespace NimbusFox.FoxCore.Managers {
    public static class VariantManager {
        public static void CreateTile(Tile baseTile, Dictionary<Color, Color> replaceColorsWith,
            string newVariantTileCode, Fox_Core modFoxCore, Blob overrides) {
            CreateTile(baseTile, replaceColorsWith, newVariantTileCode, modFoxCore.ModDirectory, overrides);
        }

        internal static void CreateTile(Tile baseTile, Dictionary<Color, Color> replaceColorsWith, string newVariantTileCode, DirectoryManager directory, Blob overrides) {
            if (GameContext.TileDatabase.AllMaterials().Any(x =>
                string.Equals(x.Code, newVariantTileCode, StringComparison.CurrentCultureIgnoreCase))) {
                return;
            }

            var file = Regex.Replace(baseTile.Configuration.Source, @"\/|\\", Path.DirectorySeparatorChar.ToString());

            var stream = GameContext.ContentLoader.ReadStream(file).ReadAllText();

            var blob = BlobAllocator.AcquireAllocator().NewBlob(false);

            blob.ReadJson(stream);

            var voxelFile = Regex.Replace(blob.GetString("voxels"), @"\/|\\", Path.DirectorySeparatorChar.ToString());

            var output = ChangeColor(voxelFile, replaceColorsWith);

            directory.WriteQBFile(newVariantTileCode, output);

            blob.SetString("code", newVariantTileCode);
            blob.SetString("voxels", directory.GetPath('/') + newVariantTileCode + ".qb");

            blob.MergeFrom(overrides);

            var wait = true;

            directory.WriteFile(newVariantTileCode + ".tile", blob, () => { wait = false; }, true);

            while (wait) { }
        }

        public static void CreateItem(Dictionary<Color, Color> replaceColorsWith, string kind,
            string newVariantTileCode, string source, Fox_Core modFoxCore, Blob overrides) {
            CreateItem(replaceColorsWith, kind, newVariantTileCode, source, modFoxCore.ModDirectory, overrides);
        }

        internal static void CreateItem(Dictionary<Color, Color> replaceColorsWith, string kind, string newVariantTileCode, string source, DirectoryManager directory, Blob overrides) {
            if (!File.Exists(Path.GetFullPath(Path.Combine(GameContext.ContentLoader.RootDirectory, source)))) {
                return;
            }

            if (GameContext.ItemDatabase.GetConfigsByKind(kind).Any(x =>
                string.Equals(x.Value.Code, newVariantTileCode, StringComparison.CurrentCultureIgnoreCase))) {
                return;
            }

            var file = Regex.Replace(source, @"\/|\\", Path.DirectorySeparatorChar.ToString());

            var stream = GameContext.ContentLoader.ReadStream(file).ReadAllText();

            var blob = BlobAllocator.AcquireAllocator().NewBlob(false);

            blob.ReadJson(stream);

            var voxelIconFile = Regex.Replace(blob.GetString("icon"), @"\/|\\", Path.DirectorySeparatorChar.ToString());
            var voxelInHandFile =
                Regex.Replace(blob.GetString("inHand"), @"\/|\\", Path.DirectorySeparatorChar.ToString());

            var iconOutput = ChangeColor(voxelIconFile, replaceColorsWith);
            var inHandOutput = ChangeColor(voxelInHandFile, replaceColorsWith);

            directory.WriteQBFile(newVariantTileCode + ".icon", iconOutput);
            directory.WriteQBFile(newVariantTileCode + ".inHand", inHandOutput);

            blob.SetString("code", newVariantTileCode);
            blob.SetString("icon", directory.GetPath('/') + newVariantTileCode + ".icon.qb");
            blob.SetString("inHand", directory.GetPath('/') + newVariantTileCode + ".inHand.qb");

            blob.MergeFrom(overrides);

            var wait = true;

            directory.WriteFile(newVariantTileCode + ".item", blob, () => { wait = false;}, true);

            while (wait) {}
        }

        private static VoxelObject ChangeColor(string file, Dictionary<Color, Color> replaceColorsWith) {
            var voxelStream = GameContext.ContentLoader.ReadStream(file);

            voxelStream.Seek(0L, SeekOrigin.Begin);

            var voxels = QbFile.Read(voxelStream);

            voxelStream.Seek(0L, SeekOrigin.Begin);

            var sVoxels = VoxelLoader.LoadQb(voxelStream, file, Vector3I.Zero,
                new Vector3I(voxels.size.X, voxels.size.Y, voxels.size.Z));

            var colors = new List<Color>();

            Fox_Core.VectorLoop(Vector3I.Zero, sVoxels.Size, (x, y, z) => {
                try {
                    var color = sVoxels.Read(x, y, z);

                    foreach (var check in replaceColorsWith) {
                        if (color == check.Key) {
                            color = check.Value;

                            sVoxels.Write(x, y, z, check.Value);
                        }
                    }

                    colors.Add(color);
                } catch { }
            });

            return sVoxels;
        }
    }
}
