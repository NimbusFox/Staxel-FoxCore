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

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, ScanCode? input, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            MouseState mouseState) {
            var size = GetSize();
            var range = origin + size;
            if (Helpers.VectorContains(origin, range, new Vector2(mouseState.X, mouseState.Y))) {
                if (mouseState.LeftButton == ButtonState.Pressed) {
                    OnClick?.Invoke();
                }
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState) {
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

                if (Helpers.VectorContains(origin, range, new Vector2(mouseState.X, mouseState.Y))) {
                    Background.Draw(origin, size, spriteBatch, _activeColor);

                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor( _activeTextColor);
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

            DrawChildren(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseState);
        }

        public event Action OnClick;
    }
}
