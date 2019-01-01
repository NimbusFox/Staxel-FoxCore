using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes {
    public class UiScrollableContainer : UiContainer {
        public uint Height { get; private set; } = 100;
        public uint Width { get; private set; } = 100;
        private Vector2 _scrollOffset = Vector2.Zero;
        private bool _showHScroll = false;
        private bool _showVScroll = false;
        RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };
        private Rectangle _lastRect;
        private bool _shiftHeld = false;

        public UiScrollableContainer() {
            ClientContext.InputSource.RegisterScrollHandler((change, state) => {
                var innerSize = InternalGetSize();
                var maxSize = GetSize() - innerSize;
                if (maxSize.X > 0) {
                    maxSize.X = 0;
                }

                if (maxSize.Y > 0) {
                    maxSize.Y = 0;
                }
                // ReSharper disable once PossibleInvalidOperationException
                if (_shiftHeld) {
                    _scrollOffset.X += change;
                    if (_scrollOffset.X > 0) {
                        _scrollOffset.X = 0;
                    }

                    if (_scrollOffset.X < maxSize.X) {
                        _scrollOffset.X = maxSize.X;
                    }
                } else {
                    _scrollOffset.Y += change;
                    if (_scrollOffset.Y > 0) {
                        _scrollOffset.Y = 0;
                    }

                    if (_scrollOffset.Y < maxSize.Y) {
                        _scrollOffset.Y = maxSize.Y;
                    }
                }
                Logger.WriteLine($"V:{_scrollOffset.X} H: {_scrollOffset.Y}");
            });
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift,
            IReadOnlyList<InterfaceLogicalButton> inputPressed, MouseState mouseState) {
            _shiftHeld = shift;
            var scrollOffset = origin + _scrollOffset;

            foreach (var element in Elements) {
                if (!element.Visible) {
                    continue;
                }

                element.Update(universe, scrollOffset, avatar, input, ctrl, shift, inputPressed, mouseState);

                scrollOffset = scrollOffset + new Vector2(0, element.GetSize().Y);
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            MouseState mouseState) {

            try {
                spriteBatch.End();
            } catch {
                // ignore
            }

            _lastRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle((int)Math.Floor(origin.X), (int)Math.Floor(origin.Y), (int)Width, (int)Height);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, null, _rasterizerState);

            var scrollOffset = origin + _scrollOffset;

            foreach (var element in Elements) {
                if (!element.Visible) {
                    continue;
                }

                element.Draw(graphics, entity, universe, scrollOffset, spriteBatch, mouseState);

                scrollOffset = scrollOffset + new Vector2(0, element.GetSize().Y);
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = _lastRect;
            spriteBatch.Begin();
        }

        public void SetDimensions(uint width, uint height) {
            Width = width;
            Height = height;
        }

        public override Vector2 GetSize() {
            return new Vector2(Width, Height);
        }

        private Vector2 InternalGetSize() {
            return base.GetSize();
        }
    }
}
