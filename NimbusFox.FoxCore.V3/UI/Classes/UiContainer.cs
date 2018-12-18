using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiContainer : UiElement {
        private UiBackground _background;
        private string _backgroundLocation = "";
        private string _currentBackground = "None";
        public Vector2 TopLeftOffset = Vector2.Zero;
        public Vector2 BottomRightOffset = Vector2.Zero;

        public UiContainer() {
        }

        public override void Dispose() {
            _background?.Dispose();
            base.Dispose();
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch) {
            if (!_backgroundLocation.IsNullOrEmpty()) {
                if (_backgroundLocation != _currentBackground) {
                    var stream = GameContext.ContentLoader.ReadStream(_backgroundLocation);
                    stream.Seek(0L, SeekOrigin.Begin);
                    var blob = BlobAllocator.Blob(false);
                    blob.ReadJson(stream.ReadAllText());
                    _background?.Dispose();
                    _background = new UiBackground(graphics.Graphics.GraphicsDevice, blob);
                    _currentBackground = _backgroundLocation;
                }
                var size = GetSize();

                _background.Draw(origin, size, spriteBatch);
            }
            base.Draw(graphics, entity, universe, origin + TopLeftOffset, spriteBatch);
        }

        public void SetBackground(string location) {
            _backgroundLocation = location;
        }

        public override Vector2 GetSize() {
            var size = Vector2.Zero;

            foreach (var element in Elements) {
                size += element.GetElementSize();
            }

            var calcSize = size + TopLeftOffset + BottomRightOffset;

            if (_background != null) {
                var min = _background.GetMinSize();
                if (min.X > calcSize.X) {
                    calcSize.X = min.X;
                }

                if (min.Y > calcSize.Y) {
                    calcSize.Y = min.Y;
                }
            }

            return calcSize;
        }

        public override Vector2 GetElementSize() {
            return GetSize();
        }
    }
}
