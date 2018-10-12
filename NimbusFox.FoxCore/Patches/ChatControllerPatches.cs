using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Dependencies.Harmony;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Rendering;
using Staxel.Translation;

namespace NimbusFox.FoxCore.Patches {
    internal static class ChatControllerPatches {

        public static bool ReceiveConsoleResponse(Blob blob) {
            string str1 = blob.GetString("response");
            var handleMessage = false;
            if (str1.IsNullOrEmpty())
                return handleMessage;


            try {
                var settings = BlobAllocator.Blob(true);
                settings.ReadJson(str1);

                if (settings.Contains("settings")) {
                    foreach (var entry in settings.FetchBlob("settings").KeyValueIteratable) {
                        try {
                            SettingsManager.UpdateSettings(entry.Key, entry.Value.Blob());
                            handleMessage = true;
                        } catch {
                            // ignore
                        }
                    }
                }
            } catch {
                // ignore
            }
            return !handleMessage;
        }
    }
}
