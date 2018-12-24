using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiPicture : UiElement {
        protected Texture2D Picture;
        protected string PictureLocation = "";
        protected string CurrentPictureLocation = "none";
        protected bool OverrideSize = false;
        protected Vector2 Size = Vector2.Zero;
        protected Color Color = Color.White;
        public override Vector2 GetElementSize() {
            return GetSize();
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            Vector2I mousePosition) {
            if (!PictureLocation.IsNullOrEmpty()) {
                if (PictureLocation != CurrentPictureLocation) {
                    Picture?.Dispose();
                    Picture = Texture2D.FromStream(graphics.Graphics.GraphicsDevice, GameContext.ContentLoader.ReadStream(PictureLocation));
                    CurrentPictureLocation = PictureLocation;
                }

                var size = GetSize();

                spriteBatch.Draw(Picture,
                    new Rectangle((int) Math.Floor(origin.X), (int) Math.Floor(origin.Y), (int) Math.Floor(size.X),
                        (int) Math.Floor(size.Y)), Color);
            }
        }

        public override Vector2 GetSize() {
            return OverrideSize ? Size : new Vector2(Picture.Width, Picture.Height);
        }

        public void SetPicture(string location) {
            PictureLocation = location;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose() {
            Picture?.Dispose();
            base.Dispose();
        }

        public void ResetSize() {
            Size = Vector2.Zero;
            OverrideSize = false;
        }

        public void SetSize(Vector2 size) {
            Size = size;
            OverrideSize = true;
        }

        public void SetColor(Color color) {
            Color = color;
        }
    }
}
