using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Core;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiSelectable : UiContainer {
        private Color _activeColor = Color.White;
        private Color _color = Color.White;
        private Color _activeTextColor = Color.White;
        private Color _textColor = Color.White;

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, Vector2I mousePosition) {
            var size = GetSize();
            var range = origin + size;
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

                if (Helpers.VectorContains(origin, range, mousePosition.ToVector2F().ToVector2())) {
                    Background.Draw(origin, size, spriteBatch, _activeColor);

                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_activeTextColor);
                        }
                    }
                } else {
                    Background.Draw(origin, size, spriteBatch, _color);

                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_textColor);
                        }
                    }
                }
            }

            DrawChildren(graphics, entity, universe, origin + TopLeftOffset, spriteBatch, mousePosition);
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
