using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;
using Color = Microsoft.Xna.Framework.Color;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiColorPicker : UiElement {

        public event Action<Color> ColorHover;
        public event Action<Color> ColorClick;
        private UiTexture2D _colorWheel;

        public UiColorPicker() {
            _colorWheel = FoxUIHook.Instance.GetPicture(Constants.Images.ColorWheel);
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            Vector2 mouseLocation, bool click, bool clickHold) {

            if (!Window.CallUpdates && !Window.Visible) {
                return;
            }

            var location = Helpers.VectorLocation(origin,
                new Vector2(origin.X + _colorWheel.Width, origin.Y + _colorWheel.Height),
                mouseLocation);

            if (location.X == -1 || location.Y == -1) {
                return;
            }

            Color color;

            try {
                color = Helpers.GetColorByCoordinate(_colorWheel.GetTexture(null), location);
            } catch {
                return;
            }

            if (color == Color.Transparent) {
                return;
            }

            ColorHover?.Invoke(color);

            if (click || clickHold) {
                ColorClick?.Invoke(color);
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            Vector2 mouseLocation, Rectangle scissor) {

            if (_colorWheel == null) {
                return;
            }

            _colorWheel.Draw(graphics, entity, universe, origin, spriteBatch, mouseLocation, scissor);
        }

        public override Vector2 GetSize() {
            if (_colorWheel == null) {
                return Vector2.Zero;
            }

            return new Vector2(_colorWheel.Width, _colorWheel.Height);
        }
    }
}
