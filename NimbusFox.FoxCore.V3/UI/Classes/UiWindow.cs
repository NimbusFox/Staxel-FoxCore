using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NimbusFox.FoxCore.V3.UI.Enums;
using Plukit.Base;
using Staxel;
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
        internal Viewport ViewPort;

        public event Action OnClose;
        public event Action OnShow;
        public event Action OnHide;

        private UiAlignment _alignment;

        public UiWindow(UiAlignment alignment = UiAlignment.MiddleCenter) {
            FoxUIHook.Instance.Windows.Add(this);
            Container = new UiContainer();
            _alignment = alignment;
        }

        internal void Draw(DeviceContext graphics, ref Matrix4F matrix, Entity avatar, Universe universe,
            AvatarController avatarController, Vector2I mousePosition) {
            if (_spriteBatch == null) {
                _spriteBatch = new SpriteBatch(graphics.Graphics.GraphicsDevice);
            }

            _spriteBatch.Begin();

            var size = Container.GetSize();

            Vector2 origin;
            ViewPort = graphics.Graphics.GraphicsDevice.Viewport;

            switch (_alignment) {
                case UiAlignment.TopLeft:
                    origin = new Vector2(0, 0);
                    break;
                case UiAlignment.TopCenter:
                    origin = new Vector2((ViewPort.Width / 2) - (size.X / 2), 0);
                    break;
                case UiAlignment.TopRight:
                    origin = new Vector2(ViewPort.Width - size.X, 0);
                    break;
                case UiAlignment.MiddleLeft:
                    origin = new Vector2(0, (ViewPort.Height / 2) - (size.Y / 2));
                    break;
                case UiAlignment.MiddleCenter:
                default:
                    origin = new Vector2((ViewPort.Width / 2) - (size.X / 2), (ViewPort.Height / 2) - (size.Y / 2));
                    break;
                case UiAlignment.MiddleRight:
                    origin = new Vector2(ViewPort.Width - size.X, (ViewPort.Height / 2) - (size.Y / 2));
                    break;
                case UiAlignment.BottomLeft:
                    origin = new Vector2(0, ViewPort.Height - size.Y);
                    break;
                case UiAlignment.BottomCenter:
                    origin = new Vector2((ViewPort.Width / 2) - (size.X / 2), ViewPort.Height - size.Y);
                    break;
                case UiAlignment.BottomRight:
                    origin = new Vector2(ViewPort.Width - size.X, ViewPort.Height - size.Y);
                    break;
            }

            Container.Draw(graphics, avatar, universe, origin, _spriteBatch, mousePosition);

            _spriteBatch.End();
        }

        internal void DrawTop(DeviceContext graphics, ref Matrix4F matrix, Entity avatar,
            EntityPainter avatarPainter, Universe universe, Timestep timestep, Vector2I mousePosition) {
            if (_spriteBatch == null) {
                _spriteBatch = new SpriteBatch(graphics.Graphics.GraphicsDevice);
            }

            _spriteBatch.Begin();

            var size = Container.GetSize();

            var origin = new Vector2((graphics.Graphics.GraphicsDevice.Viewport.Width / 2) - (size.X / 2),
                graphics.Graphics.GraphicsDevice.Viewport.Height - size.Y);

            Container.Draw(graphics, avatar, universe, origin, _spriteBatch, mousePosition);

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
            IReadOnlyList<InterfaceLogicalButton> inputPressed, MouseState mouseState) {
            var size = Container.GetSize();

            Vector2 origin;

            switch (_alignment) {
                case UiAlignment.TopLeft:
                    origin = new Vector2(0, 0);
                    break;
                case UiAlignment.TopCenter:
                    origin = new Vector2((ViewPort.Width / 2) - (size.X / 2), 0);
                    break;
                case UiAlignment.TopRight:
                    origin = new Vector2(ViewPort.Width - size.X, 0);
                    break;
                case UiAlignment.MiddleLeft:
                    origin = new Vector2(0, (ViewPort.Height / 2) - (size.Y / 2));
                    break;
                case UiAlignment.MiddleCenter:
                default:
                    origin = new Vector2((ViewPort.Width / 2) - (size.X / 2), (ViewPort.Height / 2) - (size.Y / 2));
                    break;
                case UiAlignment.MiddleRight:
                    origin = new Vector2(ViewPort.Width - size.X, (ViewPort.Height / 2) - (size.Y / 2));
                    break;
                case UiAlignment.BottomLeft:
                    origin = new Vector2(0, ViewPort.Height - size.Y);
                    break;
                case UiAlignment.BottomCenter:
                    origin = new Vector2((ViewPort.Width / 2) - (size.X / 2), ViewPort.Height - size.Y);
                    break;
                case UiAlignment.BottomRight:
                    origin = new Vector2(ViewPort.Width - size.X, ViewPort.Height - size.Y);
                    break;
            }
            Container.Update(universe, origin, avatar, input, inputPressed, mouseState);
        }

        public void AddChild(UiElement element) {
            Container.AddChild(element);
            element.SetWindow(this);
            element.SetParent(Container);
        }
    }
}
