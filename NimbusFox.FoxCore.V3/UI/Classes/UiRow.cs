using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiRow : UiElement {

        public override Vector2 GetSize() {
            var size = Vector2.Zero;

            var largestY = 0F;

            foreach (var element in Elements) {
                var eleSize = element.GetSize();
                size.X += eleSize.X;
                largestY = largestY < eleSize.Y ? eleSize.Y : largestY;
            }

            size.Y = largestY;

            return size;
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch, MouseState mouseState) {
            var offset = Vector2.Zero;

            foreach (var element in Elements) {
                if (element.Parent == null) {
                    element.SetParent(this);
                }

                if (element.Window == null) {
                    element.SetWindow(Window);
                }

                element.Draw(graphics, entity, universe, origin + offset, spriteBatch, mouseState);
                offset = offset + new Vector2(element.GetSize().X, 0);
            }
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            MouseState mouseState) {
            var offset = Vector2.Zero;

            foreach (var element in Elements) {
                element.Update(universe, origin + offset, avatar, input, ctrl, shift, inputPressed, mouseState);
                offset = offset + new Vector2(element.GetSize().X, 0);
            }
        }
    }
}
