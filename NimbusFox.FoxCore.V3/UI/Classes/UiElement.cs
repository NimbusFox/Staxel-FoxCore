using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public abstract class UiElement : IDisposable {
        public virtual void Dispose() {
            foreach (var element in Elements) {
                element.Dispose();
            }
        }

        protected UiWindow Window { get; private set; }
        protected UiElement Parent { get; private set; }

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
                offset = offset + new Vector2(0, element.GetSize().Y);
            }
        }

        public virtual Vector2 GetSize() {
            var size = new Vector2(0, 0);

            foreach (var element in Elements) {
                var eleSize = element.GetSize();

                size.X += eleSize.X;
                size.Y += eleSize.Y;
            }

            return size;
        }

        public virtual void Update(Universe universe, Entity entity) {
            foreach (var element in Elements) {
                element.Update(universe, entity);
            }
        }

        public void AddChild(UiElement element) {
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
    }
}
