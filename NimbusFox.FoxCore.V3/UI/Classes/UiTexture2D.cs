using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiTexture2D : UiElement {

        private Texture2D _texture;
        private Color _color = Color.White;

        public override Vector2 GetSize() {
            return _texture == null ? Vector2.Zero : new Vector2(_texture.Width, _texture.Height);
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            MouseState mouseState) {
            if (_texture == null) {
                return;
            }

            spriteBatch.Draw(_texture, origin, _color);
        }

        public void SetTexture(Texture2D texture) {
            _texture = texture;
        }

        public void SetColor(Color color) {
            _color = color;
        }
    }
}
