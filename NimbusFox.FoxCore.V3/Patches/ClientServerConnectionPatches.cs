using NimbusFox.FoxCore.V3.Classes;
using Staxel.Data;
using Staxel.Server;

namespace NimbusFox.FoxCore.V3.Patches {
    internal static class ClientServerConnectionPatches {
        public static void InitPatches() {
            CoreHook.FxCore.PatchController.Add(typeof(ClientServerConnection), "ConsoleMessage",
                typeof(ClientServerConnectionPatches), nameof(ConsoleMessage));
        }

        private static bool ConsoleMessage(ClientServerConnection __instance, DataPacket packet) {
            var blob = packet.Blob;

            if (!blob.Contains("message")) {
                return true;
            }

            try {
                var message = blob.FetchBlob("message");
                var storeBlob = message.FetchBlob("store");
                CoreHook.StartRestore(storeBlob);
                return false;
            } catch {
                //ignore
            }

            return true;
        }
    }
}
