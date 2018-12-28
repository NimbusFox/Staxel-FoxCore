using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using NimbusFox.FoxCore.V3.Events;
using NimbusFox.FoxCore.V3.Events.Builders;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Effects;
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

        private static ModOptionsEvent modSettingsWindow;

        private static void Update(Universe universe, AvatarController avatarController) {
            ScanCode? input = null;
            if (ClientContext.InputSource.IsAnyScanCodeDownClick(out var keyInput)) {
                input = keyInput;

                if (keyInput == ScanCode.O && !ClientContext.WebOverlayRenderer.HasInputControl()) {
                    if (modSettingsWindow == null || modSettingsWindow.Completed()) {
                        modSettingsWindow = (ModOptionsEvent)GameContext.EffectDatabase.Instance(ModOptionsEventBuilder.KindCode, EffectMode.Sustain,
                            Timestep.Null, null, null, null, null);
                    } else {
                        modSettingsWindow.Complete();
                        modSettingsWindow = null;
                    }
                }
            }

            if (ClientContext.InputSource.IsCancelDownClicked()) {
                modSettingsWindow?.Complete();
                modSettingsWindow = null;
            }

            var interfacePressed = new List<InterfaceLogicalButton>();

            foreach (InterfaceLogicalButton test in Enum.GetValues(typeof(InterfaceLogicalButton))) {
                if (ClientContext.InputSource.IsInterfaceDownClicked(test, false)) {
                    interfacePressed.Add(test);
                }
            }

            var mouseState = ClientContext.InputSource.GetMouseState();
            foreach (var window in FoxUIHook.Instance.Windows) {
                window.Update(universe, avatarController, input, interfacePressed, mouseState);
            }
        }

        private static void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
            AvatarController avatarController) {
            InitializeContentManager(graphics);
            var mouseState = ClientContext.InputSource.GetMouseState();
            var mousePosition = Vector2I.Zero;

            mousePosition.Y = mouseState.Y;
            mousePosition.X = mouseState.X;
            foreach (var window in FoxUIHook.Instance.Windows) {
                if (window.Visible && !window.AlwaysOnTop) {
                    window.Draw(graphics, ref matrix, avatar, universe, avatarController, mousePosition);
                }
            }
        }

        private static void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Vector3D renderOrigin, Entity avatar,
            EntityPainter avatarPainter, Universe universe, Timestep timestep) {
            InitializeContentManager(graphics);
            var mouseState = ClientContext.InputSource.GetMouseState();
            var mousePosition = Vector2I.Zero;

            mousePosition.Y = mouseState.Y;
            mousePosition.X = mouseState.X;
            foreach (var window in FoxUIHook.Instance.Windows) {
                if (window.Visible && window.AlwaysOnTop) {
                    window.DrawTop(graphics, ref matrix, avatar, avatarPainter, universe, timestep, mousePosition);
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
