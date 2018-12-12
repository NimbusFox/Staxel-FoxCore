using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiBackground : IDisposable {
        private Texture2D TopLeft;
        private Texture2D TopMiddle;
        private Texture2D TopRight;
        private Texture2D MiddleLeft;
        private Texture2D MiddleMiddle;
        private Texture2D MiddleRight;
        private Texture2D BottomLeft;
        private Texture2D BottomMiddle;
        private Texture2D BottomRight;

        public UiBackground(GraphicsDevice graphics, Blob images) {
            TopLeft = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("topLeft")));
            TopMiddle = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("topMiddle")));
            TopRight = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("topRight")));
            MiddleLeft = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("middleLeft")));
            MiddleMiddle = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("middleMiddle")));
            MiddleRight = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("middleRight")));
            BottomLeft = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("bottomLeft")));
            BottomMiddle = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("bottomMiddle")));
            BottomRight = Texture2D.FromStream(graphics, GameContext.ContentLoader.ReadStream(images.GetString("bottomRight")));
        }

        public void Draw(Vector2 origin, Vector2 size, SpriteBatch spriteBatch) {
            Draw(origin, size, spriteBatch, Color.White);
        }

        public void Draw(Vector2 origin, Vector2 size, SpriteBatch spriteBatch, Color color) {

            var stretchWidth = 0;
            var stretchHeight = 0;

            var minSize = GetMinSize();

            var originI = new Vector2I((int)Math.Ceiling(origin.X), (int)Math.Ceiling(origin.Y));
            var sizeI = new Vector2I((int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y));
            var minI = new Vector2I((int)Math.Ceiling(minSize.X), (int)Math.Ceiling(minSize.Y));

            if (minI.X != sizeI.X) {
                stretchWidth = sizeI.X - minI.X;
            }

            if (minI.Y != sizeI.Y) {
                stretchHeight = sizeI.Y - minI.Y;
            }

            spriteBatch.Draw(TopLeft, new Vector2(originI.X, originI.Y), color);
            if (stretchWidth != 0) {
                spriteBatch.Draw(TopMiddle,
                    new Rectangle(originI.X + TopLeft.Width, originI.Y, stretchWidth, TopLeft.Height), color);
            }

            spriteBatch.Draw(TopRight, new Vector2(originI.X + TopLeft.Width + stretchWidth, originI.Y), color);
            if (stretchHeight != 0) {
                spriteBatch.Draw(MiddleLeft,
                    new Rectangle(originI.X, originI.Y + TopLeft.Height, TopLeft.Width, stretchHeight), color);
                if (stretchWidth != 0) {
                    spriteBatch.Draw(MiddleMiddle,
                        new Rectangle(originI.X + TopLeft.Width, originI.Y + TopLeft.Height, stretchWidth,
                            stretchHeight), color);
                }

                spriteBatch.Draw(MiddleRight,
                    new Rectangle(originI.X + TopLeft.Width + stretchWidth, originI.Y + TopLeft.Height, TopLeft.Width,
                        stretchHeight), color);
            }

            spriteBatch.Draw(BottomLeft, new Vector2(originI.X, originI.Y + stretchHeight + TopLeft.Height),
                color);
            if (stretchWidth != 0) {
                spriteBatch.Draw(BottomMiddle,
                    new Rectangle(originI.X + TopLeft.Width, originI.Y + stretchHeight + TopLeft.Height, stretchWidth,
                        TopLeft.Height), color);
            }

            spriteBatch.Draw(BottomRight,
                new Vector2(originI.X + stretchWidth + TopLeft.Width, originI.Y + stretchHeight + TopLeft.Height),
                color);
        }

        public Vector2 GetMinSize() {
            return new Vector2(TopLeft.Width + TopRight.Width, TopLeft.Height + BottomLeft.Height);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            TopLeft?.Dispose();
            TopMiddle?.Dispose();
            TopRight?.Dispose();
            MiddleLeft?.Dispose();
            MiddleMiddle?.Dispose();
            MiddleRight?.Dispose();
            BottomLeft?.Dispose();
            BottomMiddle?.Dispose();
            BottomRight?.Dispose();
        }
    }
}
