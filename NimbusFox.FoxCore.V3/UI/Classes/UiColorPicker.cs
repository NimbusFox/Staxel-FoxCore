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
        protected string ColorWheelImage;
        private string _currentImage = "none";
        private Texture2D _colorWheel;

        public UiColorPicker() {
            ColorWheelImage = Constants.Images.ColorWheel;
            _currentImage = ColorWheelImage;
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            MouseState mouseState) {
            if (_colorWheel == null) {
                _colorWheel = FoxUIHook.Instance.GetPicture(ColorWheelImage);
            }

            if (ColorWheelImage != _currentImage) {
                _colorWheel = FoxUIHook.Instance.GetPicture(ColorWheelImage);
                _currentImage = ColorWheelImage;
            }

            var location = Helpers.VectorLocation(origin,
                new Vector2(origin.X + _colorWheel.Width, origin.Y + _colorWheel.Height),
                new Vector2(mouseState.X, mouseState.Y));

            if (location.X == -1 || location.Y == -1) {
                return;
            }

            var color = Helpers.GetColorByCoordinate(_colorWheel, location);

            if (color == Color.Transparent) {
                return;
            }

            ColorHover?.Invoke(color);

            if (mouseState.LeftButton == ButtonState.Pressed) {
                ColorClick?.Invoke(color);
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            MouseState mouseState) {

            if (_colorWheel == null) {
                return;
            }

            spriteBatch.Draw(_colorWheel, origin, Color.White);
        }

        public override Vector2 GetSize() {
            if (_colorWheel == null) {
                return Vector2.Zero;
            }

            return new Vector2(_colorWheel.Width, _colorWheel.Height);
        }
    }
}
