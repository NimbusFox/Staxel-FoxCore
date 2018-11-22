using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Dependencies.Harmony;
using NimbusFox.FoxCore.V3.Classes;
using Plukit.Base;
using Staxel.Client;
using Staxel.Core;

namespace NimbusFox.FoxCore.V3.Patches {
    internal static class ChatControllerPatches {
        public static void InitPatches() {
            CoreHook.FxCore.PatchController.Add(typeof(ChatController), "ReceiveConsoleResponse",
                typeof(ChatControllerPatches), nameof(ReceiveConsoleResponse));
        }

        private static bool ReceiveConsoleResponse(ChatController __instance, Blob blob) {
            try {
                var data = BlobAllocator.Blob(true);

                data.ReadJson(blob.GetString("response"));

                var storeBlob = data.FetchBlob("store");
                CoreHook.StartRestore(storeBlob);
                Blob.Deallocate(ref data);
                return false;
            } catch {
                // ignore
            }
            return true;
        }
    }
}
