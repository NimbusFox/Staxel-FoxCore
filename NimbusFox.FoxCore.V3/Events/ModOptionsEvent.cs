using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore.V3.UI.Classes;
using Plukit.Base;
using Staxel;
using Staxel.Draw;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Rendering;

namespace NimbusFox.FoxCore.V3.Events {
    public class ModOptionsEvent : IEffect {
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            _window?.Dispose();
        }
        public bool Completed() {
            return _completed;
        }

        private UiWindow _window;
        private bool _completed;

        public ModOptionsEvent() {
            _window = new UiWindow();

            _window.Container.SetBackground("mods/Fox Core V3/Staxel/UI/Backgrounds/Dark/DarkBackground.json");

            _window.Container.SetMinWidth(250);
            _window.Container.SetMinHeight(250);

            _window.Container.TopLeftOffset = new Vector2(35, 40);
            _window.Container.BottomRightOffset = new Vector2(35, 20);

            var selectable = new UiSelectable();
            selectable.SetActiveBackgroundColor(Color.Green);
            selectable.SetActiveTextColor(Color.Red);
            selectable.SetBackground("mods/Fox Core V3/Staxel/UI/Backgrounds/Dark/DarkBackground.json");
            selectable.TopLeftOffset = new Vector2(35, 40);
            selectable.BottomRightOffset = new Vector2(35, 35);

            var text = new UiTextBlock();

            text.SetString("Hello World");

            _window.AddChild(selectable);
            selectable.AddChild(text);

            _window.OnShow += () => {
                ClientContext.WebOverlayRenderer.AcquireInputControl();
            };

            _window.OnHide += () => {
                ClientContext.WebOverlayRenderer.ReleaseInputControl(); 
            };

            _window.OnClose += () => {
                ClientContext.WebOverlayRenderer.ReleaseInputControl();
            };

            _window.Show();
        }

        internal void Complete() {
            _completed = true;
            _window.Hide();
        }

        public void Render(Entity entity, EntityPainter painter, Timestep renderTimestep, DeviceContext graphics, ref Matrix4F matrix,
            Vector3D renderOrigin, Vector3D position, RenderMode renderMode) { }

        public void Stop() {
            _window.Hide();
        }
        public void Pause() { }

        public void Resume() {
            _window.Show();
        }
    }
}
