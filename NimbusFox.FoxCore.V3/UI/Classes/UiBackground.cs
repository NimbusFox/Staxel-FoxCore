using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Draw;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiBackground : IDisposable {
        private UiTexture2D TopLeft;
        private UiTexture2D TopMiddle;
        private UiTexture2D TopRight;
        private UiTexture2D MiddleLeft;
        private UiTexture2D MiddleMiddle;
        private UiTexture2D MiddleRight;
        private UiTexture2D BottomLeft;
        private UiTexture2D BottomMiddle;
        private UiTexture2D BottomRight;
        private Blob _images;
        public Vector2 TopLeftOffset { get; } = Vector2.Zero;
        public Vector2 BottomRightOffset { get; } = Vector2.Zero;
        public Vector2 TopLeftPadding { get; } = Vector2.Zero;
        public Vector2 BottomRightPadding { get; } = Vector2.Zero;

        public UiBackground(GraphicsDevice graphics, Blob images) {
            _images = images;
            TopLeft = new UiTexture2D();
            TopLeft.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("topLeft"))));
            TopLeft.TakeRender();

            TopMiddle = new UiTexture2D();
            TopMiddle.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("topMiddle"))));
            TopMiddle.TakeRender();

            TopRight = new UiTexture2D();
            TopRight.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("topRight"))));
            TopRight.TakeRender();

            MiddleLeft = new UiTexture2D();
            MiddleLeft.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("middleLeft"))));
            MiddleLeft.TakeRender();

            MiddleMiddle = new UiTexture2D();
            MiddleMiddle.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("middleMiddle"))));
            MiddleMiddle.TakeRender();

            MiddleRight = new UiTexture2D();
            MiddleRight.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("middleRight"))));
            MiddleRight.TakeRender();

            BottomLeft = new UiTexture2D();
            BottomLeft.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("bottomLeft"))));
            BottomLeft.TakeRender();

            BottomMiddle = new UiTexture2D();
            BottomMiddle.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("bottomMiddle"))));
            BottomMiddle.TakeRender();

            BottomRight = new UiTexture2D();
            BottomRight.SetTexture(context => Texture2D.FromStream(context.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(_images.GetString("bottomRight"))));
            BottomRight.TakeRender();

            if (images.Contains("topLeftOffset")) {
                TopLeftOffset = images.GetBlob("topLeftOffset").GetVector2F().ToVector2();
            }

            if (images.Contains("bottomRightOffset")) {
                BottomRightOffset = images.GetBlob("bottomRightOffset").GetVector2F().ToVector2();
            }

            if (images.Contains("topLeftPadding")) {
                TopLeftPadding = images.GetBlob("topLeftPadding").GetVector2F().ToVector2();
            }

            if (images.Contains("bottomRightPadding")) {
                BottomRightPadding = images.GetBlob("bottomRightPadding").GetVector2F().ToVector2();
            }
        }

        public void Draw(DeviceContext graphics, Vector2 origin, Vector2 size, SpriteBatch spriteBatch) {
            Draw(graphics, origin, size, spriteBatch, Color.White);
        }

        public void Draw(DeviceContext graphics, Vector2 origin, Vector2 size, SpriteBatch spriteBatch, Color color) {
            var stretchWidth = 0;
            var stretchHeight = 0;

            var minSize = GetMinSize();

            var originI = new Vector2I((int)Math.Ceiling(origin.X), (int)Math.Ceiling(origin.Y));
            var sizeI = new Vector2I((int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y));
            var minI = new Vector2I((int)Math.Ceiling(minSize.X), (int)Math.Ceiling(minSize.Y));

            if (minI.X < sizeI.X) {
                stretchWidth = sizeI.X - minI.X;
            }

            if (minI.Y < sizeI.Y) {
                stretchHeight = sizeI.Y - minI.Y;
            }

            spriteBatch.Draw(TopLeft.GetTexture(graphics), new Vector2(originI.X, originI.Y), color);
            if (stretchWidth != 0) {
                spriteBatch.Draw(TopMiddle.GetTexture(graphics),
                    new Rectangle(originI.X + TopLeft.Width, originI.Y, stretchWidth, TopLeft.Height), color);
            }

            spriteBatch.Draw(TopRight.GetTexture(graphics), new Vector2(originI.X + TopLeft.Width + stretchWidth, originI.Y), color);
            if (stretchHeight != 0) {
                spriteBatch.Draw(MiddleLeft.GetTexture(graphics),
                    new Rectangle(originI.X, originI.Y + TopLeft.Height, TopLeft.Width, stretchHeight), color);
                if (stretchWidth != 0) {
                    spriteBatch.Draw(MiddleMiddle.GetTexture(graphics),
                        new Rectangle(originI.X + TopLeft.Width, originI.Y + TopLeft.Height, stretchWidth,
                            stretchHeight), color);
                }

                spriteBatch.Draw(MiddleRight.GetTexture(graphics),
                    new Rectangle(originI.X + TopLeft.Width + stretchWidth, originI.Y + TopLeft.Height, TopLeft.Width,
                        stretchHeight), color);
            }

            spriteBatch.Draw(BottomLeft.GetTexture(graphics), new Vector2(originI.X, originI.Y + stretchHeight + TopLeft.Height),
                color);
            if (stretchWidth != 0) {
                spriteBatch.Draw(BottomMiddle.GetTexture(graphics),
                    new Rectangle(originI.X + TopLeft.Width, originI.Y + stretchHeight + TopLeft.Height, stretchWidth,
                        TopLeft.Height), color);
            }

            spriteBatch.Draw(BottomRight.GetTexture(graphics),
                new Vector2(originI.X + stretchWidth + BottomLeft.Width, originI.Y + stretchHeight + TopLeft.Height),
                color);
        }

        public Vector2 GetMinSize() {
            return new Vector2(TopLeft.Width + TopRight.Width + TopLeftPadding.X + BottomRightPadding.X,
                TopLeft.Height + BottomLeft.Height + TopLeftPadding.Y + BottomRightPadding.Y);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
        }
    }
}
