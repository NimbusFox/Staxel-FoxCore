using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiContainer : UiElement {
        protected UiBackground Background;
        protected string BackgroundLocation = "";
        protected string CurrentBackground = "None";
        public Vector2 TopLeftOffset = Vector2.Zero;
        public Vector2 BottomRightOffset = Vector2.Zero;

        public UiContainer() {
        }

        public override void Dispose() {
            Background?.Dispose();
            base.Dispose();
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, Vector2I mousePosition) {
            if (!BackgroundLocation.IsNullOrEmpty()) {
                if (BackgroundLocation != CurrentBackground) {
                    var stream = GameContext.ContentLoader.ReadStream(BackgroundLocation);
                    stream.Seek(0L, SeekOrigin.Begin);
                    var blob = BlobAllocator.Blob(false);
                    blob.ReadJson(stream.ReadAllText());
                    Background?.Dispose();
                    Background = new UiBackground(graphics.Graphics.GraphicsDevice, blob);
                    CurrentBackground = BackgroundLocation;
                }
                var size = GetSize();

                Background.Draw(origin, size, spriteBatch);
            }
            base.Draw(graphics, entity, universe, origin + TopLeftOffset, spriteBatch, mousePosition);
        }

        protected void DrawChildren(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, Vector2I mousePosition) {
            base.Draw(graphics, entity, universe, origin, spriteBatch, mousePosition);
        }

        public void SetBackground(string location) {
            BackgroundLocation = location;
        }

        public override Vector2 GetSize() {
            var size = Vector2.Zero;

            foreach (var element in Elements) {
                size += element.GetElementSize();
            }

            var calcSize = size + TopLeftOffset + BottomRightOffset;

            if (Background != null) {
                var min = Background.GetMinSize();
                if (min.X > calcSize.X) {
                    calcSize.X = min.X;
                }

                if (min.Y > calcSize.Y) {
                    calcSize.Y = min.Y;
                }
            }

            calcSize.X = MinWidth < calcSize.X ? calcSize.X : MinWidth;
            calcSize.Y = MinHeight < calcSize.Y ? calcSize.Y : MinHeight;

            return calcSize;
        }

        public override Vector2 GetElementSize() {
            return GetSize();
        }
    }
}
