using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiSpacer : UiElement {
        public override Vector2 GetSize() {
            return new Vector2(Width, Height);
        }

        private uint Width = 10;
        private uint Height = 10;

        public void SetHeight(uint height) {
            Height = height;
        }

        public void SetWidth(uint width) {
            Width = width;
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch, Vector2 mouseLocation, Rectangle scissor) {
            // do nothing
        }

        public override void AddChild(UiElement element) {
            // do nothing
        }

        public static UiSpacer GetSpacer(uint width = 10, uint height = 10) {
            var spacer = new UiSpacer();
            spacer.SetWidth(width);
            spacer.SetHeight(height);
            return spacer;
        }
    }
}
