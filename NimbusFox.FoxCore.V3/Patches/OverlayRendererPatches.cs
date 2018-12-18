using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;
using Staxel.Rendering;

namespace NimbusFox.FoxCore.V3.Patches {
    public static class OverlayRendererPatches {
        internal static void Initialize() {
            FoxUIHook.PatchController.Add(typeof(OverlayRenderer), "Draw", null, null, typeof(OverlayRendererPatches), "Draw");
            FoxUIHook.PatchController.Add(typeof(OverlayRenderer), "DrawTop", typeof(OverlayRendererPatches), "DrawTop");
            FoxUIHook.PatchController.Add(typeof(OverlayRenderer), "Update", typeof(OverlayRendererPatches), "Update");
        }

        private static void Update(Universe universe, AvatarController avatarController) {
            ScanCode? input = null;
            if (ClientContext.InputSource.IsAnyScanCodeDownClick(out var keyInput)) {
                input = keyInput;
            }

            var mouseState = ClientContext.InputSource.GetMouseState();
            var mousePosition = Vector2I.Zero;

            var interfacePressed = new List<InterfaceLogicalButton>();

            foreach (InterfaceLogicalButton test in Enum.GetValues(typeof(InterfaceLogicalButton))) {
                if (ClientContext.InputSource.IsInterfaceDownClicked(test, false)) {
                    interfacePressed.Add(test);
                }
            }

            mousePosition.Y = mouseState.Y;
            mousePosition.X = mouseState.X;

            foreach (var window in FoxUIHook.Instance.Windows) {
                window.Update(universe, avatarController, input, mousePosition, interfacePressed);
            }
        }

        private static void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
            AvatarController avatarController) {
            InitializeContentManager(graphics);
            foreach (var window in FoxUIHook.Instance.Windows) {
                if (window.Visible && !window.AlwaysOnTop) {
                    window.Draw(graphics, ref matrix, avatar, universe, avatarController);
                }
            }
        }

        private static void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Vector3D renderOrigin, Entity avatar,
            EntityPainter avatarPainter, Universe universe, Timestep timestep) {
            InitializeContentManager(graphics);
            foreach (var window in FoxUIHook.Instance.Windows) {
                if (window.Visible && window.AlwaysOnTop) {
                    window.DrawTop(graphics, ref matrix, avatar, avatarPainter, universe, timestep);
                }
            }
        }

        private static void InitializeContentManager(DeviceContext graphics) {
            if (FoxUIHook.Instance.ContentManager == null) {
                FoxUIHook.Instance.ContentManager = graphics.GetPrivateFieldValue<ContentManager>("Content");

                FoxUIHook.Instance.LoadContent();
            }
        }
    }
}
