﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NimbusFox.FoxCore.VersionCheck {
    public static class VersionCheck {
        private static string[] VersionToRemove => new [] {
            "NimbusFox.FoxCore.V1-20180228",
            "NimbusFox.FoxCore.V1-20180226",
            "NimbusFox.FoxCore.V1.2",
            "NimbusFox.FoxCore.V1.1"
        };

        public static void Check() {
            var assembly = Assembly.GetAssembly(typeof(VersionCheck));
            var dir = assembly.Location.Substring(0, assembly.Location.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal));

            var fileRemoved = false;

            foreach (var file in new DirectoryInfo(dir).GetFiles().Where(x => x.Extension == ".mod" || x.Extension == ".dll")) {
                if (VersionToRemove.Any(x => file.Name.Contains(x))) {
                    try {
                        File.Delete(file.FullName);
                    } catch {
                        // ignore
                    }

                    if (file.Extension == ".mod") {
                        fileRemoved = true;
                    }
                }
            }

            if (fileRemoved) {
                throw new Exception("\n\n\n\nOlder versions of the Fox Core mod have been removed. Please click retry below\n\n\n\n");
            }
        }
    }
}
