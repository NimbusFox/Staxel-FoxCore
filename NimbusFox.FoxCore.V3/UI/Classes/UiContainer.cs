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

        public UiContainer() {
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
            if (Background != null) {
                var size = GetSize();

                Background.Draw(origin, size, spriteBatch);
            }
            base.Draw(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseState);
        }

        protected void DrawChildren(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
            base.Draw(graphics, entity, universe, origin, spriteBatch, mouseState);
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift,
            IReadOnlyList<InterfaceLogicalButton> inputPressed, MouseState mouseState) {
            var offset = Vector2.Zero;
            foreach (var element in Elements) {
                element.Update(universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero) + offset, avatar, input, ctrl, shift, inputPressed, mouseState);
                offset = offset + new Vector2(0, element.GetSize().Y);
            }
        }

        public void SetBackground(string code) {
            Background = FoxUIHook.Instance.GetBackground(code);
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
