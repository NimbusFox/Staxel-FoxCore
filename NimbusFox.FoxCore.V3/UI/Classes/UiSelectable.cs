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
    public class UiSelectable : UiContainer {
        protected Color _activeColor = new Color(247, 240, 91);
        protected Color _color = new Color(255, 133, 46);
        protected Color _activeTextColor = new Color(18, 26, 32);
        protected Color _textColor = new Color(18, 26, 32);

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState, Rectangle scissor) {
            var size = GetSize();
            var range = origin + size;
            if (Background != null) {

                if (Helpers.VectorContains(origin, range, new Vector2(mouseState.X, mouseState.Y))) {
                    Background.Draw(graphics, origin, size, spriteBatch, _activeColor);

                    foreach (var element in Elements) {
                        if (element.Parent == null) {
                            element.SetParent(this);
                        }

                        if (element.Window == null) {
                            element.SetWindow(Window);
                        }
                        if (!element.Visible) {
                            continue;
                        }
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_activeTextColor);
                        }
                    }
                } else {
                    Background.Draw(graphics, origin, size, spriteBatch, _color);

                    foreach (var element in Elements) {
                        if (element.Parent == null) {
                            element.SetParent(this);
                        }

                        if (element.Window == null) {
                            element.SetWindow(Window);
                        }
                        if (!element.Visible) {
                            continue;
                        }
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_textColor);
                        }
                    }
                }
            }

            DrawChildren(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseState, scissor);
        }

        public void SetActiveBackgroundColor(Color color) {
            _activeColor = color;
        }

        public void SetBackgroundColor(Color color) {
            _color = color;
        }

        public void SetActiveTextColor(Color color) {
            _activeTextColor = color;
        }

        public void SetTextColor(Color color) {
            _textColor = color;
        }
    }
}
