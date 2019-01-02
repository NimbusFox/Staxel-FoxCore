using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private bool _isEnabled = true;
        private Color _disabledColor = Color.LightGray;
        private Color _disabledTextColor = Color.Gray;
        private bool _update = true;
        private bool _pressed = false;

        public event Action OnClick;
        public event Action OnHold;

        private uint _interval = 50;

        public UiButton() {
            SetBackground(Constants.Backgrounds.Button);
        }

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            Vector2 mouseLocation, bool click, bool clickHold) {
            var size = GetSize();
            var range = origin + size;
            if (_isEnabled) {
                if (Helpers.VectorContains(origin, range, mouseLocation)) {
                    if (click && !clickHold && Window?.CallUpdates != false && Window?.Visible != false) {
                        _pressed = true;
                    }

                    if (_pressed) {
                        if (!click) {
                            _pressed = false;
                            OnClick?.Invoke();
                        }
                    }

                    if (clickHold && Window?.CallUpdates != false && Window?.Visible != false) {
                        if (_update) {
                            HoldTimer();
                            OnHold?.Invoke();
                        }
                    }
                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_activeTextColor);
                        }
                    }
                } else {
                    _pressed = false;
                    foreach (var element in Elements) {
                        if (element is UiTextBlock textElement) {
                            textElement.SetColor(_textColor);
                        }
                    }
                }
            } else {
                _pressed = false;
                foreach (var element in Elements) {
                    if (element is UiTextBlock textElement) {
                        textElement.SetColor(_disabledTextColor);
                    }
                }
            }
        }

        private void HoldTimer() {
            _update = false;

            var timer = new Timer { Interval = _interval };
            timer.Elapsed += (sender, args) => {
                _update = true;
                timer.Dispose();
            };
            timer.Start();
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, Vector2 mouseLocation, Rectangle scissor) {
            var size = GetSize();
            var range = origin + size;
            if (Background != null) {
                if (_isEnabled) {
                    if (Helpers.VectorContains(origin, range, mouseLocation)) {
                        Background.Draw(graphics, origin, size, spriteBatch, _activeColor);
                    } else {
                        Background.Draw(graphics, origin, size, spriteBatch, _color);
                    }
                } else {
                    Background.Draw(graphics, origin, size, spriteBatch, _disabledColor);
                }
            }

            DrawChildren(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseLocation, scissor);
        }
        
        public void Enable() {
            _isEnabled = true;
        }

        public void Disable() {
            _isEnabled = false;
        }

        public void SetDisabledColor(Color color) {
            _disabledColor = color;
        }

        public void SetDisabledTextColor(Color color) {
            _disabledTextColor = color;
        }

        public void SetHoldInterval(uint milliseconds) {
            _interval = milliseconds;
        }
    }
}
