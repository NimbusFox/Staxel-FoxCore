using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public abstract class UiElement : IDisposable {
        public virtual void Dispose() {
            foreach (var element in Elements) {
                element.Dispose();
            }
        }

        public UiWindow Window { get; private set; }
        public UiElement Parent { get; private set; }

        protected List<UiElement> Elements { get; } = new List<UiElement>();
        protected int MinHeight = 0;
        protected int MinWidth = 0;

        public virtual void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
            var offset = Vector2.Zero;
            foreach (var element in Elements) {
                if (element.Parent == null) {
                    element.Parent = this;
                }

                if (element.Window == null) {
                    element.Window = Window;
                }
                element.Draw(graphics, entity, universe, origin + offset, spriteBatch, mouseState);
                offset = offset + new Vector2(0, element.GetSize().Y);
            }
        }

        public virtual Vector2 GetSize() {
            var size = new Vector2(0, 0);

            var largestX = 0F;

            foreach (var element in Elements) {
                var eleSize = element.GetSize();

                size.Y += eleSize.Y;
                largestX = largestX < eleSize.X ? eleSize.X : largestX;
            }

            size.X = largestX;

            size.X = size.X > MinWidth ? size.X : MinWidth;
            size.Y = size.Y > MinHeight ? size.Y : MinHeight;

            return size;
        }

        public virtual void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift,
            IReadOnlyList<InterfaceLogicalButton> inputPressed, MouseState mouseState) {
            var offset = Vector2.Zero;
            foreach (var element in Elements) {
                element.Update(universe, origin + offset, avatar, input, ctrl, shift, inputPressed, mouseState);
                offset = offset + new Vector2(0, element.GetSize().Y);
            }
        }

        public virtual void AddChild(UiElement element) {
            Elements.Add(element);
            element.SetParent(Parent);
            element.SetWindow(Window);
        }

        internal void SetWindow(UiWindow window) {
            Window = window;
        }

        internal void SetParent(UiElement element) {
            Parent = element;
        }

        public virtual void Deselect() {
            foreach (var element in Elements) {
                element.Deselect();
            }
        }

        public void SetMinHeight(int height) {
            MinHeight = height;
        }

        public void SetMinWidth(int width) {
            MinWidth = width;
        }
    }
}
