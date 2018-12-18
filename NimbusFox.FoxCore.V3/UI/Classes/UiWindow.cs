using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public sealed class UiWindow : IDisposable {

        public bool AlwaysOnTop { get; protected set; } = false;
        public bool Visible { get; private set; }
        public UiContainer Container { get; }
        private SpriteBatch _spriteBatch;
        public bool IsDisposed { get; private set; } = false;

        public event Action OnClose;
        public event Action OnShow;
        public event Action OnHide;

        public UiWindow() {
            FoxUIHook.Instance.Windows.Add(this);
            Container = new UiContainer();
        }

        internal void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
            AvatarController avatarController) {
            if (_spriteBatch == null) {
                _spriteBatch = new SpriteBatch(graphics.Graphics.GraphicsDevice);
            }

            _spriteBatch.Begin();

            var size = Container.GetSize();

            var origin = new Vector2((graphics.Graphics.GraphicsDevice.Viewport.Width / 2) - (size.X / 2),
                graphics.Graphics.GraphicsDevice.Viewport.Height - size.Y);

            Container.Draw(graphics, avatar, universe, origin, _spriteBatch);

            _spriteBatch.End();
        }

        internal void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Entity avatar,
            EntityPainter avatarPainter, Universe universe, Timestep timestep) {
            if (_spriteBatch == null) {
                _spriteBatch = new SpriteBatch(graphics.Graphics.GraphicsDevice);
            }

            _spriteBatch.Begin();

            var size = Container.GetSize();

            var origin = new Vector2((graphics.Graphics.GraphicsDevice.Viewport.Width / 2) - (size.X / 2),
                graphics.Graphics.GraphicsDevice.Viewport.Height - size.Y);

            Container.Draw(graphics, avatar, universe, origin, _spriteBatch);

            _spriteBatch.End();
        }

        public void Dispose() {
            Hide();
            OnClose?.Invoke();
            Container.Dispose();

            FoxUIHook.Instance.Windows.Remove(this);

            IsDisposed = true;
        }

        public void Show() {
            Visible = true;
            OnShow?.Invoke();
        }

        public void Hide() {
            Visible = false;
            OnHide?.Invoke();
        }

        internal void Update(Universe universe, AvatarController avatar, ScanCode? input,
            Vector2I mousePosition, IReadOnlyList<InterfaceLogicalButton> inputPressed) {
            Container.Update(universe, avatar, input, mousePosition, inputPressed);
        }

        public void AddChild(UiElement element) {
            Container.AddChild(element);
            element.SetWindow(this);
            element.SetParent(Container);
        }
    }
}
