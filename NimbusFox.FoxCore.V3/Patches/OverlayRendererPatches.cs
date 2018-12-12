using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Logic;
using Staxel.Rendering;

namespace NimbusFox.FoxCore.V3.Patches {
    public static class OverlayRendererPatches {
        internal static void Initialize() {
            FoxUIHook.PatchController.Add(typeof(OverlayRenderer), "Draw", null, null, typeof(OverlayRendererPatches), "Draw");
            FoxUIHook.PatchController.Add(typeof(OverlayRenderer), "DrawTop", typeof(OverlayRendererPatches), "DrawTop");
        }

        private static void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
            AvatarController avatarController) {
            InitializeContentManager(graphics);
            foreach (var window in FoxUIHook.Instance.Windows) {
                window.Update(universe, avatar);
                if (window.Visible && !window.AlwaysOnTop) {
                    window.Draw(graphics, ref matrix, avatar, universe, avatarController);
                }
            }
        }

        private static void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Vector3D renderOrigin, Entity avatar,
            EntityPainter avatarPainter, Universe universe, Timestep timestep) {
            InitializeContentManager(graphics);
            foreach (var window in FoxUIHook.Instance.Windows) {
                window.Update(universe, avatar);
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
