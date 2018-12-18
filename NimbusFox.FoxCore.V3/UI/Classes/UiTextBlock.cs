using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiTextBlock : UiElement {
        private string _text = "";
        private Color _color = Color.White;
        public override void Draw(DeviceContext graphics, Entity entity, Universe universe,
            Vector2 origin, SpriteBatch spriteBatch) {
            var font = FoxUIHook.Instance.GetFont();

            spriteBatch.DrawString(font, _text, origin, _color);
        }

        public override Vector2 GetSize() {
            var size = base.GetSize();

            var font = FoxUIHook.Instance.GetFont();

            var fontSize = font.MeasureString(_text);

            return size + fontSize;
        }

        public void SetString(string text) {
            _text = text;
        }

        public void SetColor(Color color) {
            _color = color;
        }

        public override Vector2 GetElementSize() {
            return GetSize();
        }
    }
}
