﻿using System;
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
        readonly RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };
        private bool _shiftHeld = false;

        private UiButton _scrollUpButton;
        private UiButton _scrollDownButton;
        private UiButton _scrollLeftButton;
        private UiButton _scrollRightButton;

        private uint _verticalScrollAmount = 20;
        private uint _horizontalScrollAmount = 20;

        public UiScrollableContainer() {
            _scrollUpButton = new UiButton();
            _scrollUpButton.SetBackground(Constants.Backgrounds.Button);
            _scrollUpButton.AddChild(FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.upArrow"));
            _scrollUpButton.Hide();
            _scrollUpButton.OnHold += () => { _scrollOffset.X -= _horizontalScrollAmount; };
            _scrollDownButton = new UiButton();
            _scrollDownButton.SetBackground(Constants.Backgrounds.Button);
            _scrollDownButton.AddChild(FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.downArrow"));
            _scrollDownButton.Hide();
            _scrollDownButton.OnHold += () => { _scrollOffset.X += _horizontalScrollAmount; };
            _scrollLeftButton = new UiButton();
            _scrollLeftButton.SetBackground(Constants.Backgrounds.Button);
            _scrollLeftButton.AddChild(FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.leftArrow"));
            _scrollLeftButton.Hide();
            _scrollLeftButton.OnHold += () => { _scrollOffset.Y-= _verticalScrollAmount; };
            _scrollRightButton = new UiButton();
            _scrollRightButton.SetBackground(Constants.Backgrounds.Button);
            _scrollRightButton.AddChild(FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.rightArrow"));
            _scrollRightButton.Hide();
            _scrollRightButton.OnHold += () => { _scrollOffset.Y += _verticalScrollAmount; };
            ClientContext.InputSource.RegisterScrollHandler((change, state) => {
                var innerSize = InternalGetSize();
                var maxSize = GetSize() - innerSize;
                if (maxSize.X > 0) {
                    maxSize.X = 0;
                }

                if (maxSize.Y > 0) {
                    maxSize.Y = 0;
                }

                if (_shiftHeld) {
                    _scrollOffset.X += change;
                } else {
                    _scrollOffset.Y += change;
                }
            });
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift,
            IReadOnlyList<InterfaceLogicalButton> inputPressed, Vector2 mouseLocation, bool click, bool clickHold) {
            _shiftHeld = shift;
            var scrollOffset = origin + _scrollOffset;

            var innerSize = InternalGetSize();
            var maxSize = new Vector2(Width, Height) - innerSize;
            if (maxSize.X > 0) {
                _scrollLeftButton.Hide();
                _scrollRightButton.Hide();
                maxSize.X = 0;
            } else {
                _scrollLeftButton.Show();
                _scrollRightButton.Show();
            }

            if (maxSize.Y > 0) {
                _scrollUpButton.Hide();
                _scrollDownButton.Hide();
                maxSize.Y = 0;
            } else {
                _scrollUpButton.Show();
                _scrollDownButton.Show();
            }

            if (_scrollOffset.X > 0) {
                _scrollOffset.X = 0;
            }

            if (_scrollOffset.X < maxSize.X) {
                _scrollOffset.X = maxSize.X;
            }

            if (_scrollOffset.Y > 0) {
                _scrollOffset.Y = 0;
            }

            if (_scrollOffset.Y < maxSize.Y) {
                _scrollOffset.Y = maxSize.Y;
            }

            _scrollLeftButton.Enable();
            _scrollRightButton.Enable();
            _scrollDownButton.Enable();
            _scrollUpButton.Enable();

            if (_scrollOffset.Y > 0) {
                _scrollUpButton.Disable();
            }

            if (_scrollOffset.Y < maxSize.Y) {
                _scrollDownButton.Disable();
            }

            if (_scrollOffset.X > 0) {
                _scrollLeftButton.Disable();
            }

            if (_scrollOffset.X < maxSize.X) {
                _scrollRightButton.Disable();
            }

            foreach (var element in Elements) {
                if (element.Parent == null) {
                    element.SetParent(this);
                }

                if (element.Window == null) {
                    element.SetWindow(Window);
                }
                if (!element.Visible) {
                    continue;
                }

                element.Update(universe, scrollOffset, avatar, input, ctrl, shift, inputPressed, mouseLocation, click, clickHold);

                scrollOffset = scrollOffset + new Vector2(0, element.GetSize().Y);
            }

            if (_scrollDownButton.Visible) {
                var size = _scrollDownButton.GetSize();
                _scrollDownButton.Update(universe,
                    ((origin + new Vector2(Width, Height) + new Vector2(5, 0)) - size) - new Vector2(0, size.Y), avatar,
                    input, ctrl, shift, inputPressed, mouseLocation, click, clickHold);
            }

            if (_scrollUpButton.Visible) {
                _scrollUpButton.Update(universe, origin + new Vector2(Width + 5, 0), avatar, input, ctrl, shift, inputPressed, mouseLocation, click, clickHold);
            }

            if (_scrollLeftButton.Visible) {
                _scrollLeftButton.Update(universe, origin + new Vector2(0, Height + 5), avatar, input, ctrl, shift, inputPressed, mouseLocation, click, clickHold);
            }

            if (_scrollRightButton.Visible) {
                var size = _scrollRightButton.GetSize();
                _scrollRightButton.Update(universe,
                    ((origin + new Vector2(Width, Height) + new Vector2(0, 5)) - size) - new Vector2(size.X, 0), avatar, input, ctrl,
                    shift, inputPressed, mouseLocation, click, clickHold);
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin, SpriteBatch spriteBatch,
            Vector2 mouseLocation, Rectangle scissor) {

            try {
                spriteBatch.End();
            } catch {
                // ignore
            }

            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle((int)Math.Floor(origin.X), (int)Math.Floor(origin.Y), (int)Width, (int)Height);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, null, _rasterizerState);

            var scrollOffset = origin + _scrollOffset;

            foreach (var element in Elements) {
                if (!element.Visible) {
                    continue;
                }

                element.Draw(graphics, entity, universe, scrollOffset, spriteBatch, mouseLocation, spriteBatch.GraphicsDevice.ScissorRectangle);

                scrollOffset = scrollOffset + new Vector2(0, element.GetSize().Y);
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissor;
            spriteBatch.Begin();

            if (_scrollDownButton.Visible) {
                var size = _scrollDownButton.GetSize();
                _scrollDownButton.Draw(graphics, entity, universe,
                    ((origin + new Vector2(Width, Height) + new Vector2(5, 0)) - size) - new Vector2(0, size.Y),
                    spriteBatch, mouseLocation, scissor);
            }

            if (_scrollUpButton.Visible) {
                _scrollUpButton.Draw(graphics, entity, universe,
                    origin + new Vector2(Width + 5, 0),
                    spriteBatch, mouseLocation, scissor);
            }

            if (_scrollLeftButton.Visible) {
                _scrollLeftButton.Draw(graphics, entity, universe, origin + new Vector2(0, Height + 5), spriteBatch,
                    mouseLocation, scissor);
            }

            if (_scrollRightButton.Visible) {
                var size = _scrollRightButton.GetSize();
                _scrollRightButton.Draw(graphics, entity, universe,
                    ((origin + new Vector2(Width, Height) + new Vector2(0, 5)) - size) - new Vector2(size.X + 8, 0), spriteBatch,
                    mouseLocation, scissor);
            }
        }

        public void SetDimensions(uint width, uint height) {
            Width = width;
            Height = height;
        }

        public override Vector2 GetSize() {
            var current = new Vector2(Width, Height);

            var iSize = InternalGetSize();

            if (iSize.X < current.X || iSize.Y < current.Y) {
                if (iSize.X < current.X) {
                    current.X = iSize.X;
                }

                if (iSize.Y < current.Y) {
                    current.Y = iSize.Y;
                }
            } else {
                if (_scrollDownButton.Visible) {
                    current.X += _scrollDownButton.GetSize().X + 5;
                }

                if (_scrollLeftButton.Visible) {
                    current.Y += _scrollLeftButton.GetSize().Y + 5;
                }
            }

            return current;
        }

        private Vector2 InternalGetSize() {
            return base.GetSize();
        }

        public void SetVerticalScrollAmount(uint amount) {
            _verticalScrollAmount = amount;
        }

        public void SetHorizontalScrollAmount(uint amount) {
            _horizontalScrollAmount = amount;
        }
    }
}
