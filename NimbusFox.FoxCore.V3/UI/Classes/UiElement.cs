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
    public abstract class UiElement : IDisposable {
        public virtual void Dispose() {
            foreach (var element in Elements) {
                element.Dispose();
            }
        }

        public UiWindow Window { get; private set; }
        public UiElement Parent { get; private set; }

        protected List<UiElement> Elements { get; } = new List<UiElement>();

        public virtual void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch) {
            var offset = Vector2.Zero;
            foreach (var element in Elements) {
                if (element.Parent == null) {
                    element.Parent = this;
                }

                if (element.Window == null) {
                    element.Window = Window;
                }
                element.Draw(graphics, entity, universe, origin + offset, spriteBatch);
                offset = offset + new Vector2(0, element.GetElementSize().Y);
            }
        }

        public virtual Vector2 GetSize() {
            var size = new Vector2(0, 0);

            var largestX = 0F;

            foreach (var element in Elements) {
                var eleSize = element.GetSize();

                size.Y += eleSize.Y;
                largestX = largestX < eleSize.Y ? eleSize.Y : largestX;
            }

            size.Y = largestX;

            return size;
        }

        public virtual void Update(Universe universe, AvatarController avatar, ScanCode? input,
            Vector2I mousePosition, IReadOnlyList<InterfaceLogicalButton> inputPressed) {
            foreach (var element in Elements) {
                element.Update(universe, avatar, input, mousePosition, inputPressed);
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

        public abstract Vector2 GetElementSize();
    }
}
