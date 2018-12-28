using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiContainer : UiElement {
        protected UiBackground Background;
        protected string BackgroundLocation = "";
        protected string CurrentBackground = "None";

        public UiContainer() {
        }

        public override void Dispose() {
            Background?.Dispose();
            base.Dispose();
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
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
            base.Draw(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseState);
        }

        protected void DrawChildren(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
            base.Draw(graphics, entity, universe, origin, spriteBatch, mouseState);
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, ScanCode? input,
            IReadOnlyList<InterfaceLogicalButton> inputPressed, MouseState mouseState) {
            var offset = Vector2.Zero;
            foreach (var element in Elements) {
                element.Update(universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero) + offset, avatar, input, inputPressed, mouseState);
                offset = offset + new Vector2(0, element.GetSize().Y);
            }
        }

        public void SetBackground(string location) {
            BackgroundLocation = location;
        }

        public override Vector2 GetSize() {
            var size = Vector2.Zero;

            foreach (var element in Elements) {
                var eleSize = element.GetSize();
                size.X = size.X < eleSize.X ? eleSize.X : size.X;
                size.Y += eleSize.Y;
            }

            var calcSize = size + (Background?.TopLeftOffset ?? Vector2.Zero) + (Background?.BottomRightOffset ?? Vector2.Zero);

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
    }
}
