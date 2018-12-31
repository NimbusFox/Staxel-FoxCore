using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiTextBlock : UiElement {
        private string _text = "";
        private Color _color = Color.White;
        private UiTexture2D _caret;
        private SpriteFont _font;
        private uint _caretHeight = 0;
        private bool _drawCaret = false;
        private uint _caretIndex = 0;

        public UiTextBlock() {
            _font = FoxUIHook.Instance.GetFont();
        }

        public void SetFont(string code = Constants.Fonts.MyFirstCrush24) {
            _font = FoxUIHook.Instance.GetFont(code);
            _caretHeight = (uint)Math.Ceiling(_font.MeasureString("W").Y - 4);
        }

        public override void AddChild(UiElement element) {
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe,
            Vector2 origin, SpriteBatch spriteBatch, MouseState mouseState) {
            if (_caret != null) {
                if (_caretIndex == 0) {
                    if (_drawCaret) {
                        spriteBatch.Draw(_caret.GetTexture(graphics), origin, _color);
                    }
                    spriteBatch.DrawString(_font, _text, origin + new Vector2(_caret.Width, 0), _color);
                } else if (_caretIndex == _text.Length) {
                    spriteBatch.DrawString(_font, _text, origin, _color);
                    if (_drawCaret) {
                        spriteBatch.Draw(_caret.GetTexture(graphics), origin + new Vector2(_font.MeasureString(_text).X, 0), _color);
                    }
                } else {
                    spriteBatch.DrawString(_font, _text.Substring(0, (int)_caretIndex), origin, _color);
                    if (_drawCaret) {
                        spriteBatch.Draw(_caret.GetTexture(graphics),
                            origin + new Vector2(_font.MeasureString(_text.Substring(0, (int) _caretIndex)).X, 0),
                            _color);
                    }
                    spriteBatch.DrawString(_font, _text.Substring((int) _caretIndex),
                        origin + new Vector2(
                            _font.MeasureString(_text.Substring(0, (int) _caretIndex)).X + _caret.Width, 0), _color);
                }
            } else {
                spriteBatch.DrawString(_font, _text, origin, _color);
            }
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

        public override string ToString() {
            return _text;
        }

        public void SetCaret(uint width = 1) {
            _caretHeight = (uint)Math.Ceiling(_font.MeasureString("W").Y - 4);
            _caret = new UiTexture2D(context => Helpers.GetTexture(context, width, _caretHeight));
            _caret.TakeRender();
        }

        public void RemoveCaret() {
            if (HasCaret()) {
                _caret.Dispose();
                _caret = null;
            }
        }

        public bool HasCaret() {
            return _caret != null;
        }

        public void ToggleCaret() {
            _drawCaret = !_drawCaret;
        }

        public void HideCaret() {
            _drawCaret = false;
        }

        public void SetCaretIndex(uint index) {
            _caretIndex = index;
        }
    }
}
