using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiTexture2D : UiElement {

        private Texture2D _texture;
        private Color _color = Color.White;
        private Func<DeviceContext, Texture2D> _textureInit;
        private bool _selfRender = true;
        public int Height => _texture?.Height ?? 0;
        public int Width => _texture?.Width ?? 0;

        public UiTexture2D(Func<DeviceContext, Texture2D> textureInit) {
            _textureInit = textureInit;
        }

        public override Vector2 GetSize() {
            return _texture == null ? Vector2.Zero : new Vector2(_texture.Width, _texture.Height);
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            Vector2 mouseLocation, Rectangle scissor) {
            TextureCheck(graphics);

            if (_selfRender) {
                spriteBatch.Draw(_texture, origin, _color);
            }
        }

        public void TextureCheck(DeviceContext graphics) {
            if (_texture == null) {
                if (_textureInit != null) {
                    _texture = _textureInit(graphics);

                    graphics.Graphics.DeviceResetting += (sender, args) => { _texture.Dispose(); };
                }
            } else {
                if (_texture.IsDisposed) {
                    _texture = _textureInit(graphics);
                }
            }
        }

        public Texture2D GetTexture(DeviceContext graphics) {
            TextureCheck(graphics);
            return _texture;
        }

        public void TakeRender() {
            _selfRender = false;
        }

        public void SetColor(Color color) {
            _color = color;
        }

        public override void AddChild(UiElement element) {

        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift,
            IReadOnlyList<InterfaceLogicalButton> inputPressed, Vector2 mouseLocation, bool click, bool clickHold) {
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose() {
            _texture?.Dispose();
            base.Dispose();
        }
    }
}
