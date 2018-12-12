using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
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

        public void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
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

        public void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Entity avatar,
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
            Container.Dispose();

            FoxUIHook.Instance.Windows.Remove(this);

            IsDisposed = true;
            OnClose?.Invoke();
        }

        public void Show() {
            Visible = true;
            OnShow?.Invoke();
        }

        public void Hide() {
            Visible = false;
            OnHide?.Invoke();
        }

        public void Update(Universe universe, Entity entity) {
            Container.Update(universe, entity);
        }

        public void AddChild(UiElement element) {
            Container.AddChild(element);
            element.SetWindow(this);
            element.SetParent(Container);
        }
    }
}
