﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using NimbusFox.FoxCore.V3.Events;
using NimbusFox.FoxCore.V3.Events.Builders;
using NimbusFox.FoxCore.V3.UI.Classes;
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

        private static bool _isClicked = false;
        private static bool _isHeld = false;

        private static void Update(Universe universe, AvatarController avatarController) {
            var input = new List<ScanCode>();

            if (ClientContext.InputSource.IsAnyScanCodeDownClick(out _)) {
                input = Helpers.GetAllKeysPressed();

                if (input.Any(x => x == ScanCode.O) && !ClientContext.WebOverlayRenderer.HasInputControl()) {
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

            var mainClick = mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed;

            if (_isClicked && mainClick) {
                _isHeld = true;
            } else {
                _isHeld = false;
            }

            _isClicked = mainClick;

            var remove = new List<UiWindow>();

            foreach (var window in new List<UiWindow>(FoxUIHook.Instance.Windows)) {
                window.Update(universe, avatarController, input, ClientContext.InputSource.IsControlKeyDown(),
                    input.Contains(ScanCode.LeftShift) || input.Contains(ScanCode.RightShift), interfacePressed,
                    mouseState.Vector2(), _isClicked, _isHeld);

                if (window.Remove || window.IsDisposed) {
                    remove.Add(window);
                }
            }

            remove.ForEach(x => FoxUIHook.Instance.Windows.Remove(x));
            remove.Clear();
        }

        private static void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
            AvatarController avatarController) {
            InitializeContentManager(graphics);
            var mouseState = ClientContext.InputSource.GetMouseState();
            try {
                foreach (var window in FoxUIHook.Instance.Windows) {
                    if (window.Visible && !window.AlwaysOnTop && !window.IsDisposed) {
                        window.Draw(graphics, ref matrix, avatar, universe, avatarController, mouseState.Vector2());
                    }
                }
            } catch (InvalidOperationException) {
                // stop enumeration bugs
            }
        }

        private static void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Vector3D renderOrigin, Entity avatar,
            EntityPainter avatarPainter, Universe universe, Timestep timestep) {
            InitializeContentManager(graphics);
            var mouseState = ClientContext.InputSource.GetMouseState();
            try {
                foreach (var window in FoxUIHook.Instance.Windows) {
                    if (window.Visible && window.AlwaysOnTop && !window.IsDisposed) {
                        window.DrawTop(graphics, ref matrix, avatar, avatarPainter, universe, timestep, mouseState.Vector2());
                    }
                }
            } catch (InvalidOperationException) {
                // stop enumeration bugs
            }
        }

        private static void InitializeContentManager(DeviceContext graphics) {
            if (FoxUIHook.Instance.ContentManager == null) {
                FoxUIHook.Instance.ContentManager = graphics.GetPrivateFieldValue<ContentManager>("Content");

                FoxUIHook.Instance.LoadContent(graphics.Graphics.GraphicsDevice);

                graphics.Graphics.DeviceReset += (sender, args) => {
                    FoxUIHook.Instance.ContentManager = null;
                };
            }
        }
    }
}
