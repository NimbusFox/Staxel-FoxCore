using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiButton : UiSelectable {
        private bool _isEnabled = true;
        private Color _disabledColor = Color.LightGray;
        private Color _disabledTextColor = Color.Gray;

        public UiButton() {
            SetBackground(Constants.Backgrounds.Button);
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            MouseState mouseState) {
            var size = GetSize();
            var range = origin + size;
            if (_isEnabled) {
                if (Helpers.VectorContains(origin, range, new Vector2(mouseState.X, mouseState.Y))) {
                    if (mouseState.LeftButton == ButtonState.Pressed) {
                        OnClick?.Invoke();
                    }
                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_activeTextColor);
                        }
                    }
                } else {
                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_textColor);
                        }
                    }
                }
            } else {
                foreach (var element in Elements) {
                    if (element is UiTextBlock textElement) {
                        textElement.SetColor(_disabledTextColor);
                    }
                }
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
            var size = GetSize();
            var range = origin + size;
            if (Background != null) {
                if (_isEnabled) {
                    if (Helpers.VectorContains(origin, range, new Vector2(mouseState.X, mouseState.Y))) {
                        Background.Draw(graphics, origin, size, spriteBatch, _activeColor);
                    } else {
                        Background.Draw(graphics, origin, size, spriteBatch, _color);
                    }
                } else {
                    Background.Draw(graphics, origin, size, spriteBatch, _disabledColor);
                }
            }

            DrawChildren(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseState);
        }

        public event Action OnClick;

        public void Enable() {
            _isEnabled = true;
        }

        public void Disable() {
            _isEnabled = false;
        }

        public void SetDisabledColor(Color color) {
            _disabledColor = color;
        }

        public void SetDisabledTextColor(Color color) {
            _disabledTextColor = color;
        }
    }
}
