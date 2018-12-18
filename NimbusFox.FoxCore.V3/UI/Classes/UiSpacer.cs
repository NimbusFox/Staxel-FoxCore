using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Staxel.Draw;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiSpacer : UiElement {
        public override Vector2 GetSize() {
            return new Vector2(Width, Height);
        }

        public override Vector2 GetElementSize() {
            return GetSize();
        }

        private uint Width = 10;
        private uint Height = 10;

        public void SetHeight(uint height) {
            Height = height;
        }

        public void SetWidth(uint width) {
            Width = width;
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch) {
            
        }

        public override void AddChild(UiElement element) {
            
        }
    }
}
